using System;

namespace TinyCache
{
    public class TinyCachePolicy
    {
        public virtual int ExpirationTimeoutInSeconds { get; set; } = 30;
        public virtual int FetchTimeout { get; set; } = 5000;
        public virtual int UpdateCacheTimeout { get; set; } = 0;
        public virtual bool UseNetworkResponseFirst { get; set; }
        public virtual TinyCacheModeEnum Mode { get; set; } = TinyCacheModeEnum.CacheFirst;
        public int BackgroundFetchTimeout { get; set; } = 20000;
        public bool ReportExceptionsOnBackgroundFetch { get; set; }

        public virtual void HandleError(Exception ex, bool fromBackground)
        {

        }

        public TinyCachePolicy SetMode(TinyCacheModeEnum mode)
        {
            Mode = mode;
            return this;
        }

        public TinyCachePolicy SetFetchTimeout(int timeoutInMilliseconds)
        {
            FetchTimeout = timeoutInMilliseconds;
            return this;
        }

        public TinyCachePolicy SetBackgroundFetchTimeout(int timeoutInMilliseconds)
        {
            BackgroundFetchTimeout = timeoutInMilliseconds;
            return this;
        }
    }
}
