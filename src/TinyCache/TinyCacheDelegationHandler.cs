using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace TinyCache
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
}
