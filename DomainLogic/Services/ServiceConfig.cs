namespace DomainLogic.Services
{
    public class ServiceConfig
    {
        public static readonly object LogLock = new object();
        public int MaxDelaySeconds { get; set; } = 10;
        public int MinDelaySeconds { get; set; } = 3;
    }
}