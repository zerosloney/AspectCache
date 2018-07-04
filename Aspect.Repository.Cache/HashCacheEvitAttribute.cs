using System;
using System.Collections.Generic;
using System.Linq;

namespace Aspect.Repository.Cache
{
    /// <inheritdoc />
    /// <summary>
    /// </summary>
    public class HashCacheEvitAttribute : Attribute
    {
        private readonly IKeyGenerator _keyGenerator;
        private ICacheRepo CacheRepo { get; }
        private string KeyNamespace { get; }
        private int[] KeyIndex { get; }
        private int[] FieldIndex { get; }
        private string[] KeysPerfix { get; }

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        /// <param name="keyNamespace">缓存命名空间</param>
        /// <param name="keyPerfix">缓存子域</param>
        /// <param name="keyIndex">拦截方法参数索引(做为hashKey的一部分)</param>
        /// <param name="fieldIndex">拦截方法参数索引(做为filedKey的一部分)</param>
        public HashCacheEvitAttribute(string keyNamespace, string keyPerfix, int[] keyIndex, params int[] fieldIndex)
        {
            CacheRepo = new CacheRepo();
            _keyGenerator = new DefaultKeyGenerator();
            KeyNamespace = keyNamespace;
            KeysPerfix = new[] { keyPerfix };
            KeyIndex = keyIndex;
            FieldIndex = fieldIndex;
        }
        /// <inheritdoc />
        /// <summary>
        /// </summary>
        /// <param name="keyNamespace">缓存命名空间</param>
        /// <param name="keyIndex">拦截方法参数索引(做为hashKey的一部分)</param>
        /// <param name="fieldIndex">拦截方法参数索引(做为filedKey的一部分)</param>
        public HashCacheEvitAttribute(string keyNamespace, int[] keyIndex, params int[] fieldIndex) : this(keyNamespace, "", keyIndex, fieldIndex) { }

        /// <inheritdoc />
        /// <param name="keyNamespace">缓存命名空间</param>
        /// <param name="keysPerfix">缓存子域(多个)</param>
        /// <param name="keyIndex">拦截方法参数索引(做为hashKey的一部分)</param>
        /// <param name="fieldIndex">拦截方法参数索引(做为filedKey的一部分)</param>
        public HashCacheEvitAttribute(string keyNamespace, string[] keysPerfix, int[] keyIndex, params int[] fieldIndex)
        {
            CacheRepo = new CacheRepo();
            _keyGenerator = new DefaultKeyGenerator();
            KeyNamespace = keyNamespace;
            KeysPerfix = keysPerfix;
            KeyIndex = keyIndex;
            FieldIndex = fieldIndex;

        }

        /// <summary>
        /// 移除缓存
        /// </summary>
        /// <param name="arguments">拦截方法的参数</param>
        /// <param name="returnType">拦截方法的返回类型</param>
        /// <param name="returnValue">拦截方法的返回值</param>
        public void RemoveIntercept(object[] arguments, Type returnType, object returnValue)
        {
            foreach (var keyPerfix in KeysPerfix)
            {
                var regionKey = _keyGenerator.GetKeyRegion(KeyNamespace,
                    string.IsNullOrEmpty(keyPerfix) ? returnType.Name : keyPerfix);
                var itemKeys = new List<string>();
                if (KeyIndex != null && KeyIndex.Length > 0)
                {
                    itemKeys.Add(_keyGenerator.GetRightKey(arguments, KeyIndex));
                    foreach (var key in KeyIndex)
                    {
                        itemKeys.Add(_keyGenerator.GetRightKey(arguments, key));
                    }
                }
                var fieldKeys = new List<string>();
                if (FieldIndex != null && FieldIndex.Length > 0)
                {
                    fieldKeys.Add(_keyGenerator.GetRightKey(arguments, FieldIndex));
                    foreach (var field in FieldIndex)
                    {
                        fieldKeys.Add(_keyGenerator.GetRightKey(arguments, field));
                    }

                }
                var cacheKeys = new List<string>();
                if (itemKeys.Count > 0)
                {
                    foreach (var itemKey in itemKeys)
                    {
                        cacheKeys.Add(_keyGenerator.GetCacheKey(regionKey, itemKey));
                    }
                }
                else
                {
                    cacheKeys.Add(regionKey);
                }
                cacheKeys = cacheKeys.Distinct().ToList();
                if (cacheKeys.Count > 0)
                {
                    foreach (var cacheKey in cacheKeys)
                    {
                        if (fieldKeys.Count > 0)
                        {
                            foreach (var filedKey in fieldKeys)
                            {
                                CacheRepo.HashDelete(cacheKey, filedKey);
                            }
                        }
                        else
                        {
                            CacheRepo.KeyDelete(cacheKey);
                        }
                    }
                }
            }
        }

    }
}
