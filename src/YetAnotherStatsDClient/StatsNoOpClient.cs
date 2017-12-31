namespace YetAnotherStatsDClient
{
    public class StatsNoOpClient : IStatsClient
    {
        public void Count(string metric, int value = 1)
        {
        }

        public void Gauge(string metric, double value)
        {
        }

        public void Timing(string metric, int value)
        {
        }

        public void Timing(string metric, long value)
        {
        }
    }
}