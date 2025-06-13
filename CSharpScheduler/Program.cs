using log4net;
using log4net.Config;

namespace CSharpScheduler
{
    internal class Program
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(Program));

        private static async Task Main(string[] args)
        {
            XmlConfigurator.Configure(new FileInfo("log4net.config"));

            var scheduledTasks = new SchedulerTasks();
            var scheduledService = new SchedulerService();

            await scheduledService.StartAsync(scheduledTasks);

            logger.Info("Wait for the tasks to kick out ...");
            Console.ReadKey();

            logger.Info("Stopping Scheduler");
            await scheduledService.StopAsync();
        }

        public string CurrentTimeStamp
        {
            get { return DateTime.Now.ToString("yyyyMMddhhmmss"); }
        }
    }
}
