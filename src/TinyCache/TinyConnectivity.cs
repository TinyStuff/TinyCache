using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace TinyCacheLib
{
    public class TinyConnectivity
    {
        
        private static TinyConnectivity _instance;
        public static TinyConnectivity Instance
        {
            get
            {
                return _instance ?? (_instance = new TinyConnectivity());
            }
            set {
                _instance = value;
            }
        }

        public static bool HasInternet
        {
            get; set;
        }

        private bool _seemConnected;
        public bool IsConnected
        {
            get
            {
                Task.Run(async () => await Ping());
                return _seemConnected;
            }
            set
            {
                _seemConnected = value;
            }
        }

        public TimeSpan CheckTimeSpan { get; private set; } = TimeSpan.FromSeconds(5);
        public string PingUrl { get; private set; } = "http://www.google.com/";

        private HttpClient _client;
        private DateTime lastCheck;

        private async Task<bool> Ping()
        {
            if ((DateTime.Now - lastCheck) > CheckTimeSpan)
            {
                lastCheck = DateTime.Now;
                if (_client == null)
                {
                    _client = new HttpClient();
                }
                var request = new HttpRequestMessage(HttpMethod.Head, PingUrl);

                var timeout = _seemConnected ? 150 : 9000;
                _seemConnected = false;

                using (var timeoutCancellationTokenSource = new CancellationTokenSource())
                {
                    try
                    {
                        var tsk = _client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, timeoutCancellationTokenSource.Token);
                        var completedTask = await Task.WhenAny(tsk, Task.Delay(timeout, timeoutCancellationTokenSource.Token));
                        if (completedTask == tsk)
                        {
                            _seemConnected = true;
                        }
                        lastCheck = DateTime.Now;
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
            if (_seemConnected)
                TinyConnectivity.HasInternet = true;
            return _seemConnected;
        }
    }
}
