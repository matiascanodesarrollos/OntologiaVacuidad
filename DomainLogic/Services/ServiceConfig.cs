namespace DomainLogic.Services
{
    public class ServiceConfig
    {
        public static readonly object LogLock = new object();
        public int MaxDelaySeconds { get; set; } = 0;
        public int MinDelaySeconds { get; set; } = 2;
    }
}