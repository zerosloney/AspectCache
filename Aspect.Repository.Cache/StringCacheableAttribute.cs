using System;

namespace Aspect.Repository.Cache
{
    /// <inheritdoc />
    /// <summary>
    /// 读取缓存属性
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class StringCacheableAttribute : Attribute
    {
        private readonly IKeyGenerator _keyGenerator;
        private ICacheRepo CacheRepo { get; }
        private string KeyNamespace { get; }
        private TimeSpan Expires { get; }
        private string KeyPerfix { get; }
        /// <inheritdoc />
        /// <summary>
        /// </summary>
        /// <param name="keyNamespace">缓存命名空间</param>
        /// <param name="expires">过期时间(秒)</param>
        public StringCacheableAttribute(string keyNamespace, int expires) : this(keyNamespace, "", expires)
        {
        }

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        /// <param name="keyNamespace">缓存命名空间</param>
        /// <param name="keyPerfix">缓存子域</param>
        /// <param name="expires">过期时间(秒)</param>
        public StringCacheableAttribute(string keyNamespace, string keyPerfix, int expires)
        {
            KeyNamespace = keyNamespace;
            Expires = TimeSpan.FromSeconds(expires);
            KeyPerfix = keyPerfix;
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
            var rightKey = _keyGenerator.GetRightKey(arguments);
            var leftKey = _keyGenerator.GetKeyRegion(KeyNamespace,
                string.IsNullOrEmpty(KeyPerfix) ? returnType.Name : KeyPerfix);
            var cacheKey = _keyGenerator.GetCacheKey(leftKey, rightKey);
            var cache = CacheRepo.StringGet(cacheKey, returnType);
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
            var rightKey = _keyGenerator.GetRightKey(arguments);
            var leftKey = _keyGenerator.GetKeyRegion(KeyNamespace,
                string.IsNullOrEmpty(KeyPerfix) ? returnType.Name : KeyPerfix);
            var cacheKey = _keyGenerator.GetCacheKey(leftKey, rightKey);
            CacheRepo.StringSet(cacheKey, returnValue, returnType, Expires);
        }
    }
}