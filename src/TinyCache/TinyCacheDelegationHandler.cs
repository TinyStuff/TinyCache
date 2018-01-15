using System;
using System.Collections.Generic;
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

    public class TinyRetryDelegationHandler : DelegatingHandler
    {

        public IList<Type> ExceptionForRetry { get; set; } = new List<Type>();
        public int MaxRetrys { get; private set; } = 5;
        public int DelayBetweenRetries { get; set; } = 500;

        public void RetryOn<T>() where T : Exception
        {
            ExceptionForRetry.Add(typeof(T));
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            int retrys = 0;
            var lastException = new Exception("Unknown error in retryhandler");
            while (retrys++ <= MaxRetrys)
            {
                try
                {
                    if (retrys > 1)
                        request.Headers.Add("x-retry", retrys.ToString());
                    var ret = await base.SendAsync(request, cancellationToken);
                    return ret;
                }
                catch (Exception ex)
                {
                    lastException = ex;
                    var doRetry = false;
                    foreach (var t in ExceptionForRetry)
                    {
                        if (ex.GetType() == t)
                            doRetry = true;
                    }
                    if (doRetry)
                        await Task.Delay(DelayBetweenRetries);
                    else
                        throw ex;
                }
            }
            throw lastException;
        }
    }
}