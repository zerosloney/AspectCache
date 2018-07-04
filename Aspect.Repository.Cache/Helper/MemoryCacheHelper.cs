using System;
using System.Runtime.Caching;

namespace Aspect.Repository.Cache
{
    public class MemoryCacheHelper
    {
        static readonly MemoryCache Cache = MemoryCache.Default;
        const string LockKey = "LAIAI:";
        static object GetCacheObject(string key)
        {
            var lockKey = $"{LockKey}{key}";
            lock (Cache)
            {
                if (Cache[lockKey] == null)
                {
                    Cache.Add(lockKey, new object(), new CacheItemPolicy { SlidingExpiration = new TimeSpan(0, 2, 0) });
                }
            }
            return Cache[lockKey];
        }

        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <param name="key">caceh key</param>
        /// <returns></returns>
        public static object Get(string key)
        {
            lock (GetCacheObject(key))
            {
                var cacheData = Cache[key];
                return cacheData;
            }
        }

        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <param name="key"></param>
        /// <param name="t"></param>
        /// <param name="expiers"></param>
        /// <returns></returns>
        public static bool Set(string key, object t, TimeSpan? expiers)
        {
            lock (GetCacheObject(key))
            {
                var cacheData = Cache[key];
                if (cacheData != null)
                {
                    Cache.Remove(key);
                }
                return Cache.Add(key, t, new CacheItemPolicy { SlidingExpiration = expiers ?? new TimeSpan(0, 2, 0) });
            }
        }

    }
}
