using System;

namespace YetAnotherStatsDClient
{
    public interface ICommunicationProvider : IDisposable
    {
        void SendMetric(string metric);
    }
}
