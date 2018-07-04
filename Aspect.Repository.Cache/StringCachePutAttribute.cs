using System;

namespace Aspect.Repository.Cache
{
    /// <inheritdoc />
    /// <summary>
    /// 更新缓存属性
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class StringCachePutAttribute : Attribute
    {
        private readonly IKeyGenerator _keyGenerator;
        private ICacheRepo CacheRepo { get; }
        private string KeyNamespace { get; }
        private TimeSpan Expires { get; }
        private int[] ItemArgsIndex { get; }
        private string KeyPerfix { get; }

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        /// <param name="keyNamespace">缓存命名缓存</param>
        /// <param name="expires">过期时间(秒)</param>
        /// <param name="argsIndex">拦截方法的参数索引</param>
        /// <param name="keyPerfix">缓存子域</param>
        public StringCachePutAttribute(string keyNamespace, string keyPerfix, int expires, params int[] argsIndex)
        {
            KeyNamespace = keyNamespace;
            Expires = TimeSpan.FromSeconds(expires);
            ItemArgsIndex = argsIndex;
            KeyPerfix = keyPerfix;
            CacheRepo = new CacheRepo();
            _keyGenerator = new DefaultKeyGenerator();
        }

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        /// <param name="keyNamespace">缓存命名缓存</param>
        /// <param name="expires">过期时间(秒)</param>
        /// <param name="argsIndex">拦截方法的参数索引</param>
        public StringCachePutAttribute(string keyNamespace, int expires, params int[] argsIndex) : this(keyNamespace, "", expires, argsIndex) { }

        /// <summary>
        /// 更新缓存
        /// </summary>
        /// <param name="arguments">拦截方法的参数</param>
        /// <param name="returnValue">拦截方法的返回值</param>
        /// <param name="returnType">拦截方法的返回类型</param>
        public void UpdateIntercept(object[] arguments, Type returnType, object returnValue)
        {
            var leftKey = _keyGenerator.GetKeyRegion(KeyNamespace,
                string.IsNullOrEmpty(KeyPerfix) ? returnType.Name : KeyPerfix);
            var rightKey = _keyGenerator.GetRightKey(arguments, ItemArgsIndex);
            var cacheKey = _keyGenerator.GetCacheKey(leftKey, rightKey);
            CacheRepo.StringSet(cacheKey, returnValue, returnType, Expires);
        }
    }
}