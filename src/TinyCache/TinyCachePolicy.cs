using System;

namespace TinyCache
{
    public class TinyCachePolicy
    {
        public virtual double ExpirationTimeoutInSeconds { get; set; } = 30;
        public virtual double FetchTimeout { get; set; } = 5000;
        public virtual double UpdateCacheTimeout { get; set; } = 0;
        public virtual bool UseNetworkResponseFirst { get; set; }
        public virtual TinyCacheModeEnum Mode { get; set; } = TinyCacheModeEnum.CacheFirst;
        public double BackgroundFetchTimeout { get; set; } = 20000;
        public bool ReportExceptionsOnBackgroundFetch { get; set; }

        public Action<Exception, bool> ExceptionHandler { get; set; }
        public Action<string, object> UpdateHandler { get; set; }

        public TinyCachePolicy SetUpdateCacheTimeout(TimeSpan ts)
        {
            UpdateCacheTimeout = ts.TotalMilliseconds;
            return this;
        }

        public TinyCachePolicy SetUpdateCacheTimeout(double milliseconds)
        {
            UpdateCacheTimeout = milliseconds;
            return this;
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

        public TinyCachePolicy SetFetchTimeout(TimeSpan ts)
        {
            FetchTimeout = ts.TotalMilliseconds;
            return this;
        }

        public TinyCachePolicy SetBackgroundFetchTimeout(int timeoutInMilliseconds)
        {
            BackgroundFetchTimeout = timeoutInMilliseconds;
            return this;
        }

        public TinyCachePolicy SetExpirationTime(TimeSpan ts)
        {
            ExpirationTimeoutInSeconds = ts.TotalSeconds;
            return this;
        }

        public TinyCachePolicy SetExpirationTime(double seconds)
        {
            ExpirationTimeoutInSeconds = seconds;
            return this;
        }

        public TinyCachePolicy SetBackgroundFetchTimeout(TimeSpan ts)
        {
            BackgroundFetchTimeout = ts.TotalMilliseconds;
            return this;
        }
    }
}
