

namespace Aspect.Repository.Cache
{
    public class ThrottlingHelper
    {
        private static readonly RedisHelper Helper = new RedisHelper();
        /// <summary>
        /// 是否开启降级
        /// </summary>
        /// <returns></returns>
        public static bool IsOpenDegrade()
        {
            var keyGenerator = new DefaultKeyGenerator();
            var cacheKey = keyGenerator.GetKeyRegion(CacheConstants.ServiceKeyNamespace, CacheConstants.KeyDegradation);
            var redisValue = Helper.StringGet(cacheKey);
            if (redisValue.HasValue)
            {
                return (int)redisValue > 0;
            }
            return false;
        }

        /// <summary>
        /// 设置限流降级脚本(请求数限流)
        /// </summary>
        /// <param name="limit">限制请求数(1s)</param>
        /// <param name="duration">设置降级持续时间(s)</param>
        public static bool ThrottlingDegrade(int limit, int duration)
        {
            var str = @"
                        local limitKey = KEYS[1]
                        local degradeKey = KEYS[2]
                        local limit = tonumber(ARGV[1])
                        local d = tonumber(ARGV[2])
                        
                        local current = tonumber(redis.call('GET',limitKey) or '0')
                        --超过1秒limit数时 开启动态降级
                        if current >= limit then
                            redis.call('SET',degradeKey,1)
                            redis.call('EXPIRE',degradeKey,d)
                        else 
                            redis.call('INCRBY',limitKey,1)
                            redis.call('EXPIRE',limitKey,1)
                        end

                        --判断是否已经动态开启降级
                        local v = tonumber(redis.call('GET',degradeKey) or '0')
                        if v > 0 then
                           return 1;
                        else
                           return 0;
                        end 
                        ";
            var keyGenerator = new DefaultKeyGenerator();
            var d = keyGenerator.GetKeyRegion(CacheConstants.ServiceKeyNamespace, CacheConstants.KeyDegradation);
            var l = keyGenerator.GetKeyRegion(CacheConstants.ServiceKeyNamespace, CacheConstants.KeyLimitSecond);
            var result = Helper.ExecLuaScript(str, new[] { l, d }, new[] { limit.ToString(), duration.ToString() });
            if (!result.IsNull)
            {
                return (int)result > 0;
            }
            return false;
        }
    }
}
