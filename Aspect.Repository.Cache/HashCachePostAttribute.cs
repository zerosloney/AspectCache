using System;
using Aspect.Repository.Cache;

namespace Aspect.Repository.Cache
{
    /// <inheritdoc />
    /// <summary>
    /// </summary>
    public class HashCachePostAttribute : Attribute
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
        public HashCachePostAttribute(string keyNamespace, int expires, int[] keyIndex, params int[] fieldIndex) : this(keyNamespace, "", expires, keyIndex, fieldIndex)
        {
        }

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        /// <param name="keyNamespace">缓存命名空间</param>
        /// <param name="keyPerfix">缓存子域</param>
        /// <param name="expires">过期时间(秒)</param>
        /// <param name="keyIndex">拦截方法参数索引(做为hashKey的一部分)</param>
        /// <param name="fieldIndex">拦截方法参数索引(做为filedKey的一部分)</param>
        public HashCachePostAttribute(string keyNamespace, string keyPerfix, int expires, int[] keyIndex, params int[] fieldIndex)
        {
            _keyGenerator = new DefaultKeyGenerator();
            CacheRepo = new CacheRepo();
            KeyNamespace = keyNamespace;
            Expires = TimeSpan.FromSeconds(expires);
            FieldIndex = fieldIndex;
            KeyIndex = keyIndex;
            KeyPerfix = keyPerfix;
        }

        /// <summary>
        /// 新增缓存
        /// </summary>
        /// <param name="returnValue">拦截方法的返回值</param>
        /// <param name="returnType">拦截方法的返回类型</param>
        /// <param name="arguments">拦截方法的参数</param>
        public void AddIntercept(object[] arguments, Type returnType, object returnValue)
        {
            var regionKey = _keyGenerator.GetKeyRegion(KeyNamespace,
                string.IsNullOrEmpty(KeyPerfix) ? returnType.Name : KeyPerfix);
            var itemKey = KeyIndex != null && KeyIndex.Length > 0 ? _keyGenerator.GetRightKey(arguments, KeyIndex) : "";
            var cacheKey = _keyGenerator.GetCacheKey(regionKey, itemKey);
            var filedKey = _keyGenerator.GetRightKey(arguments, FieldIndex);
            CacheRepo.HashSet(cacheKey, filedKey, returnValue, returnType, Expires);
        }
    }
}
