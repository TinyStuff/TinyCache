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

        public static EventHandler<Exception> OnError;
        public static EventHandler<CacheUpdatedEvt> OnUpdate;
        public static EventHandler<CacheUpdatedEvt> OnRemove;
        public static EventHandler<bool> OnLoadingChange;

        public static async Task<TResult> TimeoutAfter<TResult>(Func<Task<TResult>> task, int timeout)
        {
            OnLoadingChange?.Invoke(task, true);

            using (var timeoutCancellationTokenSource = new CancellationTokenSource())
            {
                var tsk = task();
                var completedTask = await Task.WhenAny(tsk, Task.Delay(timeout, timeoutCancellationTokenSource.Token));
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

        public static async Task<T> UsePolicy<T>(string key, Func<Task<T>> func, TinyCachePolicy policy = null)
        {
            object ret = Storage.Get(key, typeof(T));

            if (policy == null)
            {
                policy = defaultPolicy;
			}
            
            if (policy.Mode == TinyCacheModeEnum.FetchFirst)
            {
                try
                {
                    var realFetch = await TimeoutAfter<T>(func, policy.FetchTimeout);
                    if (ret != null)
                    {
                        ret = realFetch;
                        Store(key, realFetch);
                    }
                }
                catch (Exception ex)
                {
                    OnError?.Invoke(policy, ex);
                }
            }

            StartBackgroundFetch(key, func, policy);

            if (ret == null)
            {
				return default(T);
            }

            if (typeof(T) == typeof(object))
            {
				return (T)ret;
            }

            return (T)Convert.ChangeType(ret, typeof(T));
        }

        private static void StartBackgroundFetch<T>(string key, Func<Task<T>> func, TinyCachePolicy policy)
        {
            if (policy.UpdateCacheTimeout > 0)
            {
                if (ShouldFetch(policy.UpdateCacheTimeout, key))
                {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                    Task.Delay(policy.UpdateCacheTimeout).ContinueWith(async (arg) =>
                    {
                        try
                        {
                            var newvalue = await TimeoutAfter<T>(func, policy.BackgroundFetchTimeout);
                            Store(key, newvalue);
                        }
                        catch (Exception ex)
                        {
                            if (policy.ReportExceptionsOnBackgroundFetch)
                                OnError?.Invoke(policy, ex);
                        }
                    });
                }
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            }
        }

        public static void SetBasePolicy(TinyCachePolicy tinyCachePolicy)
        {
            defaultPolicy = tinyCachePolicy;
        }

        public static void SetCacheStore(ICacheStorage store)
        {
            Storage = store;
        }

        public static void Remove(string key)
        {
            Storage.Remove(key);
            OnRemove?.Invoke(key, new CacheUpdatedEvt()
            {
                Key = key,
                Value = null
            });
        }

        private static void AddLastFetch(string key)
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

        private static bool ShouldFetch(int v, string key)
        {
            if (!lastFetch.ContainsKey(key))
            {
				return true;
            }

            var timeDiff = (DateTime.Now - lastFetch[key]).TotalMilliseconds;
            return (timeDiff > (v * 10));
        }

        private static void Store(string key, object val)
        {
            if (val != null)
            {
                AddLastFetch(key);

                if (Storage.Store(key, val))
                {
					OnUpdate?.Invoke(val, new CacheUpdatedEvt() { Key = key, Value = val });
                }
            }
        }

        private static Dictionary<string, DateTime> lastFetch = new Dictionary<string, DateTime>();

        //        public static bool ReportExceptionsOnBackgroundFetch { get; set; } = true;

        //        private static async Task<T> FetchWithCache<T>(string key, Func<Task<T>> func, int timeout = 4800)
        //        {
        //            T ret;
        //            try
        //            {
        //                ret = await TimeoutAfter<T>(func, timeout);
        //                if (ret != null)
        //                {
        //                    Store(key, ret);
        //                }
        //                else
        //                    ret = await Get<T>(key, func, 1000, timeout * 10);
        //            }
        //            catch (Exception ex)
        //            {
        //                ret = await Get<T>(key, func, 1000, timeout * 10);
        //            }
        //            return ret;
        //        }

        //        private static async Task<T> Get<T>(string key, Func<Task<T>> func, int refreshTime = 0, int timeout = 4800)
        //        {
        //            object ret = storage.Get(key, typeof(T));
        //            if (ret == null)
        //            {
        //                try
        //                {
        //                    ret = await TimeoutAfter<T>(func, timeout);
        //                    Store(key, ret);

        //                }
        //                catch (Exception ex)
        //                {
        //                    OnError?.Invoke(ret, ex);
        //                }
        //            }
        //            else
        //            {
        //                if (refreshTime > 0)
        //                {
        //                    if (ShouldFetch(refreshTime + timeout, key))
        //                    {
        //#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        //                        Task.Delay(refreshTime).ContinueWith(async (arg) =>
        //                        {
        //                            try
        //                            {
        //                                var newvalue = await TimeoutAfter<T>(func, timeout * 2);
        //                                Store(key, newvalue);
        //                            }
        //                            catch (Exception ex)
        //                            {
        //                                if (ReportExceptionsOnBackgroundFetch)
        //                                    OnError?.Invoke(ret, ex);
        //                            }
        //                        });
        //                    }
        //#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        //        }
        //    }
        //    if (typeof(T) == typeof(object))
        //        return (T)ret;
        //    return (T)Convert.ChangeType(ret, typeof(T));
        //}
    }
}
