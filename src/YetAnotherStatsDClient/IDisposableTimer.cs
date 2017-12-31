using System;

namespace YetAnotherStatsDClient
{
    public interface IDisposableTimer : IDisposable
    {
        string StatName { get; set; }
    }
}