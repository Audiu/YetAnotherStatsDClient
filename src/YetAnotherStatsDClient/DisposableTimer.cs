using System;
using System.Diagnostics;

namespace YetAnotherStatsDClient
{
    internal class DisposableTimer : IDisposableTimer
    {
        private bool _disposed;

        private IStatsClient _statsClient;
        private Stopwatch _stopwatch;

        public string StatName { get; set; }

        public DisposableTimer(IStatsClient statsClient, string statName)
        {
            if (string.IsNullOrEmpty(statName))
            {
                throw new ArgumentNullException(nameof(statName));
            }

            _statsClient = statsClient ?? throw new ArgumentNullException(nameof(statsClient));

            StatName = statName;
            _stopwatch = Stopwatch.StartNew();
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;
                _stopwatch.Stop();

                if (string.IsNullOrEmpty(StatName))
                {
                    throw new InvalidOperationException("StatName must be set");
                }

                _statsClient.Timing(StatName, _stopwatch.ElapsedMilliseconds);

                _stopwatch = null;
                _statsClient = null;
            }
        }
    }
}