using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace TinyCacheLib
{
    public class TinyCacheDelegationHandler : DelegatingHandler
    {
        public TinyCachePolicy DefaultPolicy { get; set; }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (request.Method == HttpMethod.Get)
            {
                return await TinyCache.RunAsync<HttpResponseMessage>(request.RequestUri.ToString(), async () =>
                {
                    var ret = await base.SendAsync(request, cancellationToken);
                    return ret;
                }, DefaultPolicy);
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }

    internal class ResponseHandler
    {
        internal ResponseHandler(Task<HttpResponseMessage> originalRequest)
        {
            IsRunning = !originalRequest.IsCompleted;
            HandleRequest(originalRequest);

        }

        private void HandleRequest(Task<HttpResponseMessage> originalRequest)
        {
            var waiter = originalRequest.GetAwaiter();
            waiter.OnCompleted(() =>
            {
                IsRunning = false;
                OriginalRespone = originalRequest.Result;
                OnCompleted?.Invoke(this, new EventArgs());
            });
        }

        public bool IsRunning { get; set; }
        private HttpResponseMessage OriginalRespone { get; set; }

        public EventHandler OnCompleted { get; set; }

        private HttpResponseMessage CloneResponse(HttpResponseMessage response)
        {
            var newResponse = new HttpResponseMessage(response.StatusCode);
            var ms = new MemoryStream();

            foreach (var v in response.Headers) newResponse.Headers.TryAddWithoutValidation(v.Key, v.Value);
            if (response.Content != null)
            {
                response.Content.CopyToAsync(ms).GetAwaiter().GetResult();
                ms.Position = 0;
                newResponse.Content = new StreamContent(ms);

                foreach (var v in response.Content.Headers) newResponse.Content.Headers.TryAddWithoutValidation(v.Key, v.Value);

            }
            return newResponse;
        }

        public Task<HttpResponseMessage> AddRequest()
        {
            var task = new Task<HttpResponseMessage>(() =>
            {
                while (IsRunning)
                {
                    Task.Delay(30);
                }
                return CloneResponse(OriginalRespone);
            });
            task.Start();
            return task;
        }
    }

    public class TinyNetworkDispatcher : DelegatingHandler
    {
        private Dictionary<string, ResponseHandler> currentRequests = new Dictionary<string, ResponseHandler>();

        private object lockObj = true;

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (request.Method == HttpMethod.Get)
            {
                var key = request.RequestUri.PathAndQuery;
                lock (lockObj)
                {
                    var hasHandler = currentRequests.ContainsKey(key);
                    if (hasHandler && currentRequests[key].IsRunning)
                    {
                        return currentRequests[key].AddRequest();
                    }
                    else
                    {
                        var res = base.SendAsync(request, cancellationToken);
                        var respHandler = new ResponseHandler(res);
                        respHandler.OnCompleted += (sender, e) =>
                        {
                            lock (lockObj)
                            {
                                currentRequests.Remove(key);
                            }
                        };
                        currentRequests.Add(key, respHandler);
                        return res;
                    }
                }
            }
            return base.SendAsync(request, cancellationToken);
        }
    }
}