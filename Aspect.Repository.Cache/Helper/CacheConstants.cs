
namespace Aspect.Repository.Cache
{
    public static class CacheConstants
    {
        /// <summary>
        /// 默认缓存失效时间(5min)
        /// </summary>
        public const int DefalutCacheExperies = 300;
        /// <summary>
        /// 缓存命名空间
        /// </summary>
        public const string KeyNamespace = "某项目";
        /// <summary>
        /// 服务缓存命名空间
        /// </summary>
        public const string ServiceKeyNamespace = "SERVICES";
        /// <summary>
        /// 降级
        /// </summary>
        public const string KeyDegradation = "DEGRADE";
   
        /// <summary>
        /// 降级缓存头部
        /// </summary>
        public const string DegradationHttpHeader = "CacheDegradation";
        /// <summary>
        /// 每秒连接数
        /// </summary>
        public const string KeyLimitSecond = "LIMIT";

        /// <summary>
        /// 是否开启缓存
        /// </summary>
        public static bool IsOpenCache => ConfigHelper.GetIntValue("IsOpenCache") > 0;

        #region 用户缓存
        /// <summary>
        /// 用户缓存前缀(ID)
        /// </summary>
        public const string UserId = "ID";

        /// <summary>
        /// 用户列表
        /// </summary>
        public const string Users = "USER";
        #endregion

      
    }
}
