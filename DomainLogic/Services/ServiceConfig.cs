namespace DomainLogic.Services
{
    public class ServiceConfig
    {
        public static readonly object LogLock = new object();
        public int MaxDelaySeconds { get; set; } = 2;
        public int MinDelaySeconds { get; set; } = 0;
    }
}