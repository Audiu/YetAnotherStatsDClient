using System;

namespace YetAnotherStatsDClient
{
    public class StatsClient : IStatsClient, IDisposable
    {
        private readonly IStatsConfig _statsConfig;

        private readonly ICommunicationProvider _communicationProvider;
        
        public StatsClient(IStatsConfig statsConfig)
        {
            _statsConfig = statsConfig;

            if (string.IsNullOrEmpty(_statsConfig.Host))
            {
                throw new ArgumentException("Host must be provided");
            }

            if (_statsConfig.Port <= 0)
            {
                throw new ArgumentException("Valid port must be provided");
            }

            if (string.IsNullOrEmpty(_statsConfig.Prefix))
            {
                throw new ArgumentException("Prefix must be provided");
            }

            _communicationProvider = new TcpCommunicationProvider(_statsConfig);
        }

        private void SendMetric(string metric)
        {
            _communicationProvider.SendMetric(metric);
        }

        public void Count(string metric, int value = 1)
        {
            SendMetric($"{_statsConfig.Prefix}.{metric}:{value}|c");
        }

        public void Gauge(string metric, double value)
        {
            SendMetric($"{_statsConfig.Prefix}.{metric}:{value}|g");
        }

        public void Timing(string metric, int value)
        {
            SendMetric($"{_statsConfig.Prefix}.{metric}:{value}|ms");
        }

        public void Timing(string metric, long value)
        {
            SendMetric($"{_statsConfig.Prefix}.{metric}:{value}|ms");
        }

        public void Dispose()
        {
            _communicationProvider?.Dispose();
        }
    }
}