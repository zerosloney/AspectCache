using System;

namespace Aspect.Repository.Cache
{
    /// <inheritdoc />
    /// <summary>
    /// 新增缓存属性
    /// </summary>
    public class StringCachePostAttribute : Attribute
    {
        private readonly IKeyGenerator _keyGenerator;
        private ICacheRepo CacheRepo { get; }
        private string[] KeyNamespace { get; }
        private TimeSpan Expires { get; }
        private int[] ItemArgsIndex { get; }
        private string KeyPerfix { get; }

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        /// <param name="keyNamespace">缓存命名空间</param>
        /// <param name="expires">过期时间(秒)</param>
        /// <param name="argsIndex">拦截方法的参数索引</param>
        /// <param name="keyPerfix">缓存子域</param>
        public StringCachePostAttribute(string[] keyNamespace,string keyPerfix, int expires, params int[] argsIndex)
        {
            _keyGenerator = new DefaultKeyGenerator();
            CacheRepo = new CacheRepo();
            KeyNamespace = keyNamespace;
            Expires = TimeSpan.FromSeconds(expires);
            ItemArgsIndex = argsIndex;
            KeyPerfix = keyPerfix;
        }
        /// <inheritdoc />
        /// <summary>
        /// </summary>
        /// <param name="keyNamespace">缓存命名空间</param>
        /// <param name="expires">过期时间(秒)</param>
        /// <param name="argsIndex">拦截方法的参数索引</param>
        public StringCachePostAttribute(string[] keyNamespace, int expires, params int[] argsIndex) : this(keyNamespace, "", expires, argsIndex) { }

        /// <summary>
        /// 新增缓存
        /// </summary>
        /// <param name="returnValue">拦截方法的返回值</param>
        /// <param name="returnType">拦截方法的返回类型</param>
        /// <param name="arguments">拦截方法的参数</param>
        public void AddIntercept(object[] arguments, Type returnType, object returnValue)
        {
            foreach (var keyNamespace in KeyNamespace)
            {
                var left = _keyGenerator.GetKeyRegion(keyNamespace,
                    string.IsNullOrEmpty(KeyPerfix) ? returnType.Name : KeyPerfix);
                var right = _keyGenerator.GetRightKey(arguments, ItemArgsIndex);
                var cacheKey = _keyGenerator.GetCacheKey(left, right);
                CacheRepo.StringSet(cacheKey, returnValue, returnType, Expires);
            }
        }
    }
}