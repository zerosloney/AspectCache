using System;
using System.Collections.Generic;
using System.Linq;

namespace Aspect.Repository.Cache
{
    /// <inheritdoc />
    /// <summary>
    /// 缓存移除属性
    /// </summary>
    public class StringCacheEvitAttribute : Attribute
    {
        private readonly IKeyGenerator _keyGenerator;
        private ICacheRepo CacheRepo { get; }
        private string KeyNamespace { get; }
        private string[] KeysPerfix { get; }
        private int[] FieldIndex { get; }
        /// <inheritdoc />
        /// <summary>
        /// </summary>
        /// <param name="keyNamespace">缓存命名空间</param>
        /// <param name="keyPerfix">缓存前缀</param>
        /// <param name="fieldIndex"></param>
        public StringCacheEvitAttribute(string keyNamespace, string[] keyPerfix, params int[] fieldIndex)
        {
            CacheRepo = new CacheRepo();
            _keyGenerator = new DefaultKeyGenerator();
            KeyNamespace = keyNamespace;
            KeysPerfix = keyPerfix;
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
                var leftKey = _keyGenerator.GetKeyRegion(KeyNamespace,
                    string.IsNullOrEmpty(keyPerfix) ? returnType.Name : keyPerfix);

                var rightKeys = new List<string>();
                if (FieldIndex != null && FieldIndex.Length > 0)
                {
                    rightKeys.Add(_keyGenerator.GetRightKey(arguments, FieldIndex));
                    foreach (var field in FieldIndex)
                    {
                        rightKeys.Add(_keyGenerator.GetRightKey(arguments, field));
                    }
                }
                var cacheKeys = new List<string>();
                if (rightKeys.Count > 0)
                {
                    foreach (var rightKey in rightKeys)
                    {
                        cacheKeys.Add(_keyGenerator.GetCacheKey(leftKey, rightKey));
                    }
                    cacheKeys = cacheKeys.Distinct().ToList();
                    if (cacheKeys.Count > 0)
                    {
                        foreach (var cacheKey in cacheKeys)
                        {
                            CacheRepo.KeyDelete(cacheKey);
                        }
                    }
                }
                else
                {
                    cacheKeys.Add(leftKey + ":*");
                    cacheKeys = cacheKeys.Distinct().ToList();
                    if (cacheKeys.Count > 0)
                    {
                        foreach (var cacheKey in cacheKeys)
                        {
                            CacheRepo.KeyDeletePattern(cacheKey);
                        }
                    }
                }

            }
        }
    }
}