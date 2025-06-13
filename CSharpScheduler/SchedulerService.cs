using Quartz;
using Quartz.Impl;
using System.Reflection;

namespace CSharpScheduler
{
    public class SchedulerService
    {
        private readonly IScheduler _scheduler;

        public SchedulerService()
        {
            var factory = new StdSchedulerFactory();
            _scheduler = factory.GetScheduler().Result;
        }

        public async Task StartAsync(object target)
        {
            var methods = target.GetType()
            .GetMethods()
            .Where(m => m.GetCustomAttributes<ScheduledAttribute>().Any());

            foreach (var methodInfo in methods)
            {
                var attribute = methodInfo.GetCustomAttribute<ScheduledAttribute>()??throw new InvalidOperationException($"Scheduler Attribute is required for {methodInfo.Name}.");
                var jobDetail = CreateJobDetail(target, methodInfo);
                var trigger = CreateTrigger(attribute);
                await _scheduler.ScheduleJob(jobDetail, trigger);
            }

            await _scheduler.Start();
        }

        public async Task StopAsync()
        {
            await _scheduler.Shutdown();
        }

        private ITrigger CreateTrigger(ScheduledAttribute attribute)
        {
            var timeZone = TimeZoneInfo.FindSystemTimeZoneById(attribute.TimeZoneId);
            return TriggerBuilder.Create().WithCronSchedule(attribute.CronExpression, x => x.InTimeZone(timeZone)).Build();
        }

        private IJobDetail CreateJobDetail(object target, MethodInfo methodInfo)
        {
            return JobBuilder.Create<DynamicJob>()
                .UsingJobData("TargetType", target.GetType().AssemblyQualifiedName)
                .UsingJobData("MethodName", methodInfo.Name)
                .Build();
        }
    }

    public class DynamicJob : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            var targetType = context.JobDetail.JobDataMap.GetString("TargetType") ?? throw new InvalidOperationException("TargetType is null");
            var methodName = context.JobDetail.JobDataMap.GetString("MethodName") ?? throw new InvalidOperationException("MethodName is null");
            var type = Type.GetType(targetType) ?? throw new InvalidOperationException($"Type {targetType} could not be found.");
            var target = Activator.CreateInstance(type) ?? throw new InvalidOperationException($"Unable to create an instance of type {type.FullName}");
            var method = target.GetType().GetMethod(methodName) ?? throw new InvalidOperationException($"Method {methodName} could not be found");
            method.Invoke(target, null);
            return Task.CompletedTask;
        }
    }
}
