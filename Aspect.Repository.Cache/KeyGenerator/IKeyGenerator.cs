
namespace Aspect.Repository.Cache
{ 
    public interface IKeyGenerator
    {
        /// <summary>
        /// 根据方法参数生成缓存分隔符左边部分
        /// </summary>
        /// <param name="args">参数</param>
        /// <param name="argsIndex">参数索引</param>
        /// <returns></returns>
        string GetRightKey(object[] args, params int[] argsIndex);

        /// <summary>
        /// 获取缓存区域
        /// </summary>
        /// <param name="keyNamespace">缓存命名空间</param>
        /// <param name="perfixKey">缓存区域</param>
        /// <returns></returns>
        string GetKeyRegion(string keyNamespace, string perfixKey);

        /// <summary>
        /// 生成缓存键
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        string GetCacheKey(string left, string right);
    }
}
