using System;

namespace YetAnotherStatsDClient
{
    public class StatsConfig : IStatsConfig
    {
        public string Host { get; set; }

        public int Port { get; set; }

        public string Prefix { get; set; }

        public Action<Exception> OnLogException { get; set; }

        public bool ShutdownOnDispose { get; set; }
    }
}
