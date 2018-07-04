
using System;

namespace Aspect.Repository.Cache
{
    public interface ICacheRepo
    {
        /// <summary>
        /// 序列化
        /// </summary>
        ISerializer Serializer { get; }

        #region String操作

        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <param name="redisKey">缓存Key键</param>
        /// <param name="returnType">返回类型</param>
        /// <returns></returns>
        object StringGet(string redisKey, Type returnType);

        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <param name="redisKey">缓存Key键</param>
        /// <param name="redisValue">被缓存的单个对象或者集合对象</param>
        /// <param name="returnType">返回类型</param>
        /// <param name="expires"></param>
        /// <returns></returns>
        bool StringSet(string redisKey, object redisValue, Type returnType, TimeSpan? expires);

        /// <summary>
        /// 删除缓存
        /// </summary>
        /// <param name="redisKey"></param>
        /// <returns></returns>
        bool KeyDelete(string redisKey);

        /// <summary>
        /// 删除模式所有的键
        /// </summary>
        /// <param name="redisKeyPattern"></param>
        /// <returns></returns>
        bool KeyDeletePattern(string redisKeyPattern);

        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="redisKey"></param>
        /// <returns></returns>
        T StringGet<T>(string redisKey);

        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <param name="redisKey">缓存Key键</param>
        /// <param name="redisValue">被缓存的单个对象或者集合对象</param>
        /// <param name="expires"></param>
        /// <returns></returns>
        bool StringSet<T>(string redisKey, T redisValue, TimeSpan? expires);
        #endregion

        #region Hash操作

        /// <summary>
        /// 获取哈希内字段的值
        /// </summary>
        /// <param name="redisKey"></param>
        /// <param name="filedKey"></param>
        /// <param name="returnType"></param>
        /// <returns></returns>
        object HashGet(string redisKey, string filedKey, Type returnType);
        /// <summary>
        /// 设置哈希内字段的值
        /// </summary>
        /// <param name="redisKey"></param>
        /// <param name="filedKey"></param>
        /// <param name="redisValue"></param>
        /// <param name="returnType"></param>
        /// <param name="expires"></param>
        void HashSet(string redisKey, string filedKey, object redisValue, Type returnType, TimeSpan? expires);
        /// <summary>
        /// 删除哈希内的字段
        /// </summary>
        /// <param name="redisKey"></param>
        /// <param name="filedKey"></param>
        /// <returns></returns>
        bool HashDelete(string redisKey, string filedKey);

        #endregion

    }
}
