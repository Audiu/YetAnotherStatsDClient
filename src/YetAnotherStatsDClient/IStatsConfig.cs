using System;

namespace YetAnotherStatsDClient
{
    public interface IStatsConfig
    {
        string Host { get; set; }

        int Port { get; set; }

        string Prefix { get; set; }

        Action<Exception> OnLogException { get; set; }

        /// <summary>
        /// This should be injected as singleton, but if using DI and (for example) we have a singleton
        /// but we want to return it scoped, the DI will try to dispose and will shutdown the producer
        /// 
        /// This is a hack to stop that from happening.
        /// 
        /// As the thread is background, it won't stop the app from shutting down, and data might be lost
        /// </summary>
        bool ShutdownOnDispose { get; set; }
    }
}