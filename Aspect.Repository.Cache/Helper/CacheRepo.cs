using System;

namespace Aspect.Repository.Cache
{
    public class CacheRepo : ICacheRepo
    {
        public ISerializer Serializer { get; }

        private readonly RedisHelper _helper;

        public CacheRepo()
        {
            Serializer = new MsgPackSerializer();
            _helper = new RedisHelper();
        }

        public CacheRepo(ISerializer serializer)
        {
            Serializer = serializer;
            _helper = new RedisHelper();
        }

        public object StringGet(string redisKey, Type returnType)
        {
            var redisValue = _helper.StringGet(redisKey);
            if (redisValue.IsNullOrEmpty || !redisValue.HasValue)
            {
                return null;
            }
            var bs = (byte[])redisValue;
            return Serializer.Deserialize(returnType, bs);
        }

        public bool StringSet(string redisKey, object redisValue, Type returnType, TimeSpan? expires)
        {
            var bs = Serializer.Serialize(returnType, redisValue);
            return _helper.StringSet(redisKey, bs, expires);
        }

        public bool KeyDelete(string redisKey)
        {
            return _helper.KeyDelete(redisKey);
        }

        public bool KeyDeletePattern(string redisKeyPattern)
        {
            return _helper.KeyDeletePattern(redisKeyPattern);
        }

        public object HashGet(string redisKey, string filedKey, Type returnType)
        {
            var redisValue = _helper.HashGet(redisKey, filedKey);
            if (redisValue.IsNullOrEmpty || !redisValue.HasValue)
            {
                return null;
            }
            var bs = (byte[])redisValue;
            return Serializer.Deserialize(returnType, bs);
        }

        public void HashSet(string redisKey, string filedKey, object redisValue, Type returnType, TimeSpan? expires)
        {
            var bs = Serializer.Serialize(returnType, redisValue);
            _helper.HashSet(redisKey, filedKey, bs);
            _helper.KeyExpire(redisKey, expires);
        }

        public bool HashDelete(string redisKey, string filedKey)
        {
            return _helper.HashDelete(redisKey, filedKey);
        }

        public T StringGet<T>(string redisKey)
        {
            var redisValue = _helper.StringGet(redisKey);
            if (redisValue.IsNullOrEmpty || !redisValue.HasValue)
            {
                return default(T);
            }
            var bs = (byte[])redisValue;
            return Serializer.Deserialize<T>(bs);
        }

        public bool StringSet<T>(string redisKey, T redisValue, TimeSpan? expires)
        {
            var bs = Serializer.Serialize(redisValue);
            return _helper.StringSet(redisKey, bs, expires);
        }
    }
}