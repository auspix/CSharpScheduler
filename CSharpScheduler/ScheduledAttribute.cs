namespace CSharpScheduler
{
    public class ScheduledAttribute:Attribute
    {
        public string CronExpression { get; }
        public string TimeZoneId { get; }
        public ScheduledAttribute(string cronExpression, string timeZoneId)
        {
            this.CronExpression = cronExpression;
            this.TimeZoneId = timeZoneId;
        }
    }
}
