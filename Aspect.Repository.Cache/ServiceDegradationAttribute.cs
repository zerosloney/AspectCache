using System;

namespace Aspect.Repository.Cache
{
    /// <inheritdoc />
    /// <summary>
    /// 服务降级特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property)]
    public class ServiceDegradationAttribute : Attribute
    {
        public bool IsOpenDegrade()
        {
            return ThrottlingHelper.IsOpenDegrade();
        }
    }
}
