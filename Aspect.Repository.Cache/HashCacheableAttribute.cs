using System;

namespace Aspect.Repository.Cache
{
    /// <inheritdoc />
    /// <summary>
    /// </summary>
    public class HashCacheableAttribute : Attribute
    {
        private readonly IKeyGenerator _keyGenerator;
        private ICacheRepo CacheRepo { get; }
        private string KeyNamespace { get; }
        private TimeSpan Expires { get; }
        private string KeyPerfix { get; }
        private int[] KeyIndex { get; }
        private int[] FieldIndex { get; }

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        /// <param name="keyNamespace">缓存命名空间</param>
        /// <param name="expires">过期时间(秒)</param>
        /// <param name="keyIndex">拦截方法参数索引(做为hashKey的一部分)</param>
        /// <param name="fieldIndex">拦截方法参数索引(做为filedKey的一部分)</param>
        public HashCacheableAttribute(string keyNamespace, int expires, int[] keyIndex, params int[] fieldIndex) : this(keyNamespace, "", expires, keyIndex, fieldIndex)
        {
        }

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        /// <param name="keyNamespace">缓存命名空间</param>
        /// <param name="keyPerfix">缓存子域</param>
        /// <param name="expires">过期时间(秒)</param>
        public HashCacheableAttribute(string keyNamespace,string keyPerfix, int expires) : this(keyNamespace, keyPerfix, expires, null, null) { }

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        /// <param name="keyNamespace">缓存命名空间</param>
        /// <param name="keyPerfix">缓存子域</param>
        /// <param name="expires">过期时间(秒)</param>
        /// <param name="keyIndex">拦截方法参数索引(做为hashKey的一部分)</param>
        /// <param name="fieldIndex">拦截方法参数索引(做为filedKey的一部分)</param>
        public HashCacheableAttribute(string keyNamespace, string keyPerfix, int expires, int[] keyIndex, params int[] fieldIndex)
        {
            KeyNamespace = keyNamespace;
            Expires = TimeSpan.FromSeconds(expires);
            KeyPerfix = keyPerfix;
            KeyIndex = keyIndex;
            FieldIndex = fieldIndex;
            CacheRepo = new CacheRepo();
            _keyGenerator = new DefaultKeyGenerator();
        }

        /// <summary>
        /// 读取缓存
        /// </summary>
        /// <param name="arguments">拦截方法的参数</param>
        /// <param name="returnType">拦截方法的返回类型</param>
        /// <returns></returns>
        public object LoadIntercept(object[] arguments, Type returnType)
        {
            var filedKey = _keyGenerator.GetRightKey(arguments, FieldIndex);
            var regionKey = _keyGenerator.GetKeyRegion(KeyNamespace, string.IsNullOrEmpty(KeyPerfix) ? returnType.Name : KeyPerfix);
            var itemKey = KeyIndex != null && KeyIndex.Length > 0 ? _keyGenerator.GetRightKey(arguments, KeyIndex) : "";
            var cacheKey = _keyGenerator.GetCacheKey(regionKey, itemKey);
            var cache = CacheRepo.HashGet(cacheKey, filedKey, returnType);
            return cache;
        }


        /// <summary>
        /// 加入缓存
        /// </summary>
        /// <param name="arguments">拦截方法的参数</param>
        /// <param name="returnValue">拦截方法的返回值</param>
        /// <param name="returnType">拦截方法的返回类型</param>
        public void StoreIntercept(object[] arguments, Type returnType, object returnValue)
        {
            var filedKey = _keyGenerator.GetRightKey(arguments, FieldIndex);
            var regionKey = _keyGenerator.GetKeyRegion(KeyNamespace, string.IsNullOrEmpty(KeyPerfix) ? returnType.Name : KeyPerfix);
            var itemKey = KeyIndex != null && KeyIndex.Length > 0 ? _keyGenerator.GetRightKey(arguments, KeyIndex) : "";
            var cacheKey = _keyGenerator.GetCacheKey(regionKey, itemKey);
            CacheRepo.HashSet(cacheKey, filedKey, returnValue, returnType, Expires);
        }
    }
}