using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TinyCache
{
    public static class TinyCache
    {
        private static TinyCachePolicy defaultPolicy = new TinyCachePolicy();

        public static ICacheStorage Storage { get; internal set; } = new MemoryDictionaryCache();
        public static ICacheStorage SecondaryStorage { get; internal set; }

        public static EventHandler<Exception> OnError;
        public static EventHandler<CacheUpdatedEvt> OnUpdate;
        public static EventHandler<CacheUpdatedEvt> OnRemove;
        public static EventHandler<bool> OnLoadingChange;

        /// <summary>
        /// Timeouts function after specified amout of milliseconds.
        /// </summary>
        /// <returns>Result of running function.</returns>
        /// <param name="task">Task to run.</param>
        /// <param name="timeout">Timeout in milliseconds</param>
        /// <typeparam name="TResult">Type of result when running function</typeparam>
        public static async Task<TResult> TimeoutAfter<TResult>(Func<Task<TResult>> task, double timeout)
        {
            OnLoadingChange?.Invoke(task, true);

            using (var timeoutCancellationTokenSource = new CancellationTokenSource())
            {
                var tsk = task();
                var completedTask = await Task.WhenAny(tsk, Task.Delay((int)timeout, timeoutCancellationTokenSource.Token));
                if (completedTask == tsk)
                {
                    OnLoadingChange?.Invoke(task, false);
                    timeoutCancellationTokenSource.Cancel();
                    return await tsk;  // Very important in order to propagate exceptions
                }

                OnLoadingChange?.Invoke(task, false);
                throw new TimeoutException("The operation has timed out.");
            }
        }

        /// <summary>
        /// Gets from cache storage and return null if not found.
        /// </summary>
        /// <returns>The from storage.</returns>
        /// <param name="key">Cache key.</param>
        /// <typeparam name="T">Type of the stored data.</typeparam>
        public static T GetFromStorage<T>(string key)
        {
            object ret = Storage.Get(key, typeof(T));
            if (ret == null && SecondaryStorage != null)
            {
                ret = SecondaryStorage.Get(key, typeof(T));
                if (ret != null)
                    Storage.Store(key, ret);
            }
            if (typeof(T) == typeof(object))
            {
                return (T)ret;
            }

            return (T)ret;
        }

        /// <summary>
        /// Fetch using policy
        /// </summary>
        /// <returns>The data from function or cache depending on policy.</returns>
        /// <param name="key">Cache key.</param>
        /// <param name="func">Function for populating cache</param>
        /// <param name="policy">Policy.</param>
        /// <typeparam name="T">Return type of function and cache object.</typeparam>
        public static async Task<T> RunAsync<T>(string key, Func<Task<T>> func, TinyCachePolicy policy = null)
        {
            var ttype = typeof(T);
            object ret = Storage.Get(key, ttype);
            if (ret == null && SecondaryStorage != null)
            {
                ret = SecondaryStorage.Get(key, ttype);
            }
            if (policy == null)
            {
                policy = defaultPolicy;
            }

            if (policy.Mode == TinyCacheModeEnum.FetchFirst || ret == null)
            {
                try
                {
                    var realFetch = await TimeoutAfter<T>(func, policy.FetchTimeout);
                    if (realFetch != null)
                    {
                        ret = realFetch;
                        Store(key, realFetch, policy);
                    }
                }
                catch (Exception ex)
                {
                    OnError?.Invoke(policy, ex);
                    policy.ExceptionHandler?.Invoke(ex, true);
                }
            }

            StartBackgroundFetch(key, func, policy);

            if (ret == null)
            {
                return default(T);
            }

            return (T)ret;
        }

        /// <summary>
        /// Sets the base policy wich will be used if not specified in each request.
        /// </summary>
        /// <param name="tinyCachePolicy">Tiny cache policy.</param>
        public static void SetBasePolicy(TinyCachePolicy tinyCachePolicy)
        {
            defaultPolicy = tinyCachePolicy;
        }

        /// <summary>
        /// Sets the permanent (secondary) cache storage type.
        /// </summary>
        /// <param name="store">Storage instance.</param>
        public static void SetCacheStore(ICacheStorage store)
        {
            SecondaryStorage = store;
        }

        /// <summary>
        /// Sets the permanent cache storage type.
        /// </summary>
        /// <param name="store">Storage instance.</param>
        public static void SetCacheStore(ICacheStorage primary, ICacheStorage secondary)
        {
            Storage = primary;
            SecondaryStorage = secondary;
        }

        /// <summary>
        /// Remove the specified key from the cache store
        /// </summary>
        /// <returns>The remove.</returns>
        /// <param name="key">Key.</param>
        public static void Remove(string key)
        {
            Storage.Remove(key);
            OnRemove?.Invoke(key, new CacheUpdatedEvt()
            {
                Key = key,
                Value = null
            });
        }

        private static void StartBackgroundFetch<T>(string key, Func<Task<T>> func, TinyCachePolicy policy)
        {
            if (policy.UpdateCacheTimeout > 0)
            {
                if (ShouldFetch(policy.UpdateCacheTimeout, key))
                {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                    Task.Delay((int)policy.UpdateCacheTimeout).ContinueWith(async (arg) =>
                    {
                        try
                        {
                            var newvalue = await TimeoutAfter<T>(func, policy.BackgroundFetchTimeout);
                            Store(key, newvalue, policy);
                        }
                        catch (Exception ex)
                        {
                            if (policy.ReportExceptionsOnBackgroundFetch)
                            {
                                OnError?.Invoke(policy, ex);
                                policy.ExceptionHandler?.Invoke(ex, true);
                            }
                        }
                    });
                }
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            }
        }

        private static Object thisLock = new Object();
        private static void AddLastFetch(string key)
        {
            lock (thisLock)
            {
                if (lastFetch.ContainsKey(key))
                {
                    lastFetch[key] = DateTime.Now;
                }
                else
                {
                    lastFetch.Add(key, DateTime.Now);
                }
            }
        }

        private static bool ShouldFetch(double v, string key)
        {
            if (!lastFetch.ContainsKey(key))
            {
                return true;
            }

            var timeDiff = (DateTime.Now - lastFetch[key]).TotalMilliseconds;
            return (timeDiff > v);
        }

        private static void Store(string key, object val, TinyCachePolicy policy)
        {
            if (val != null)
            {
                AddLastFetch(key);

                if (Storage.Store(key, val))
                {
                    if (SecondaryStorage != null)
                    {
                        Task.Delay(10).ContinueWith((a) =>
                        {
                            SecondaryStorage.Store(key, val);
                        });
                    }
                    policy?.UpdateHandler?.Invoke(key, val);
                    OnUpdate?.Invoke(val, new CacheUpdatedEvt() { Key = key, Value = val });
                }
            }
        }

        private static Dictionary<string, DateTime> lastFetch = new Dictionary<string, DateTime>();


    }
}
