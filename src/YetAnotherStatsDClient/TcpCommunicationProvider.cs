using System;
using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace YetAnotherStatsDClient
{
    public class TcpCommunicationProvider : ICommunicationProvider
    {
        private readonly IStatsConfig _statsConfig;

        private readonly BlockingCollection<string> _statsBuffer;

        private readonly Thread _sendThread;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private bool _isRunning = true;

        private TcpClient _tcpClient;
        private NetworkStream _networkStream;

        public TcpCommunicationProvider(IStatsConfig statsConfig)
        {
            _statsConfig = statsConfig;

            _statsBuffer = new BlockingCollection<string>(new ConcurrentQueue<string>());

            _cancellationTokenSource = new CancellationTokenSource();

            _sendThread = new Thread(Sender)
                {
                    IsBackground = true
                };

            _sendThread.Start(_cancellationTokenSource.Token);
        }

        public bool Connected => (_tcpClient?.Connected ?? false) && (_networkStream?.CanWrite ?? false);

        public void SendMetric(string metric)
        {
            _statsBuffer.Add(metric);
        }

        private void Sender(object obj)
        {
            var cancellationToken = (CancellationToken) obj;

            while (_isRunning)
            {
                string metric;

                try
                {
                    metric = _statsBuffer.Take(cancellationToken);
                }
                catch (Exception ex)
                {
                    _statsConfig.OnLogException?.Invoke(ex);
                    continue;
                }

                while (!Connected)
                {
                    try
                    {
                        Connect();
                    }
                    catch (Exception ex)
                    {
                        _statsConfig.OnLogException?.Invoke(ex);
                        Thread.Sleep(10000);

                        // Drop all the statistics on the floor
                        while (_statsBuffer.TryTake(out var droppedMetric, 10))
                        {
                        }
                    }
                }

                try
                {
                    var payload = Encoding.ASCII.GetBytes(metric + "\n");
                    _networkStream.Write(payload, 0, payload.Length);
                }
                catch (Exception ex)
                {
                    _statsConfig.OnLogException?.Invoke(ex);
                }
            }
        }

        private void Connect()
        {
            if (!Connected)
            {
                _tcpClient?.Dispose();
                _tcpClient = new TcpClient();       // TODO: Would be good to chuck a keep alive on here
                _tcpClient.Connect(_statsConfig.Host, _statsConfig.Port);
                if (_tcpClient.Connected)
                {
                    _networkStream = _tcpClient.GetStream();
                }

                if (!Connected)
                {
                    throw new Exception("Cannot connect, or network stream cannot be written");
                }
            }
        }

        public void Dispose()
        {
            if (_statsConfig.ShutdownOnDispose)
            {
                _cancellationTokenSource.Cancel();
                _isRunning = false;
                _sendThread.Abort();
            }
        }
    }
}