namespace YetAnotherStatsDClient
{
    public interface IStatsClient
    {
        void Count(string metric, int value = 1);
        void Gauge(string metric, double value);
        void Timing(string metric, int value);
        void Timing(string metric, long value);
    }
}
