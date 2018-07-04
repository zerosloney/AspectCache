using System.Linq;
using Castle.DynamicProxy;

namespace Aspect.Repository.Cache
{
    public class UseCacheInterceptor : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            if (CacheConstants.IsOpenCache)
            {
                var b = Before(invocation);
                if (!b)
                {
                    invocation.Proceed();
                    After(invocation);
                }
            }
            else
            {
                invocation.Proceed();
            }
        }

        /// <summary>
        /// 执行方法前
        /// </summary>
        /// <param name="invocation"></param>
        private bool Before(IInvocation invocation)
        {

            if (invocation.Method.GetCustomAttributes(typeof(ServiceDegradationAttribute), false).FirstOrDefault() is
                ServiceDegradationAttribute degradtionAttribute)
            {
                var b = degradtionAttribute.IsOpenDegrade();
                if (b)
                {
                    invocation.ReturnValue = null;
                }
                return b;
            }
            if (invocation.Method.GetCustomAttributes(typeof(StringCacheableAttribute), false).FirstOrDefault() is
                StringCacheableAttribute stringAttr)
            {
                var obj = stringAttr.LoadIntercept(invocation.Arguments, invocation.Method.ReturnType);
                if (obj != null)
                {
                    invocation.ReturnValue = obj;
                    return true;
                }
            }

            if (invocation.Method.GetCustomAttributes(typeof(HashCacheableAttribute), false).FirstOrDefault() is
                HashCacheableAttribute hashAttr)
            {
                var obj = hashAttr.LoadIntercept(invocation.Arguments, invocation.Method.ReturnType);
                if (obj != null)
                {
                    invocation.ReturnValue = obj;
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 执行方法后
        /// </summary>
        /// <param name="invocation"></param>
        private void After(IInvocation invocation)
        {
            #region string
            if (invocation.Method.GetCustomAttributes(typeof(StringCacheableAttribute), false).FirstOrDefault() is
                    StringCacheableAttribute cacheableAttribute)
            {
                cacheableAttribute.StoreIntercept(invocation.Arguments, invocation.Method.ReturnType,
                    invocation.ReturnValue);
            }

            if (invocation.Method.GetCustomAttributes(typeof(StringCachePutAttribute), false).FirstOrDefault() is
                StringCachePutAttribute putAttribute)
            {
                putAttribute.UpdateIntercept(invocation.Arguments, invocation.Method.ReturnType,
                    invocation.ReturnValue);
            }

            if (invocation.Method.GetCustomAttributes(typeof(StringCacheEvitAttribute), false).FirstOrDefault() is
                StringCacheEvitAttribute evitAttribute)
            {
                evitAttribute.RemoveIntercept(invocation.Arguments, invocation.Method.ReturnType,
                    invocation.ReturnValue);
            }

            if (invocation.Method.GetCustomAttributes(typeof(StringCachePostAttribute), false).FirstOrDefault() is
                StringCachePostAttribute postAttribute)
            {
                postAttribute.AddIntercept(invocation.Arguments, invocation.Method.ReturnType,
                invocation.ReturnValue);
            }
            #endregion

            #region hash
            if (invocation.Method.GetCustomAttributes(typeof(HashCacheableAttribute), false).FirstOrDefault() is
                HashCacheableAttribute hashCacheableAttribute)
            {
                hashCacheableAttribute.StoreIntercept(invocation.Arguments, invocation.Method.ReturnType,
                    invocation.ReturnValue);
            }

            if (invocation.Method.GetCustomAttributes(typeof(HashCachePutAttribute), false).FirstOrDefault() is
                HashCachePutAttribute hashCachePutAttribute)
            {
                hashCachePutAttribute.UpdateIntercept(invocation.Arguments, invocation.Method.ReturnType,
                    invocation.ReturnValue);
            }

            if (invocation.Method.GetCustomAttributes(typeof(HashCacheEvitAttribute), false).FirstOrDefault() is
                HashCacheEvitAttribute hashCacheEvitAttribute)
            {
                hashCacheEvitAttribute.RemoveIntercept(invocation.Arguments, invocation.Method.ReturnType,
                    invocation.ReturnValue);
            }

            if (invocation.Method.GetCustomAttributes(typeof(HashCachePostAttribute), false).FirstOrDefault() is
                HashCachePostAttribute hashCachePostAttribute)
            {
                hashCachePostAttribute.AddIntercept(invocation.Arguments, invocation.Method.ReturnType,
                    invocation.ReturnValue);
            }
            #endregion
        }
    }
}