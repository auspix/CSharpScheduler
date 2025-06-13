using log4net;

namespace CSharpScheduler
{
    public class SchedulerTasks
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(SchedulerTasks));

        [Scheduled("0 19 3 ? * *", "Asia/Shanghai")]
        public void MorningTask()
        {
            logger.Info($"MorningTask executed at {DateTime.Now}");
        }

        [Scheduled("0 0 5 ? * MON-FRI", "Asia/Shanghai")]
        public void AfternoonTask()
        {
            logger.Info($"AfternoonTask executed at {DateTime.Now}");
        }

        [Scheduled("0 * * * * ?", "Asia/Shanghai")]
        public void MinutesTask()
        {
            logger.Info($"MinutesTask executed at {DateTime.Now}");
        }
    }
}
