using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Aspect.Repository.Cache;

namespace Aspect.Web.Cache
{
    /// <inheritdoc />
    /// <summary>
    /// 响应缓存
    /// </summary>
    public class ResponseCacheFilter : ActionFilterAttribute
    {
        /// <summary>
        /// 缓存键值
        /// </summary>
        protected string CacheKey { get; set; }

        /// <summary>
        /// 缓存时间
        /// </summary>
        public int Duration { get; }

        /// <inheritdoc />
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="duration"></param>
        public ResponseCacheFilter(int duration)
        {
            Duration = duration;
        }

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        /// <param name="actionContext"></param>
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            if (actionContext.Request.Method == HttpMethod.Get)
            {
                //5min降级时间
                var b = ThrottlingHelper.ThrottlingDegrade(ConfigHelper.GetIntValue("Throttling"), 300);
                //当开启动态降级
                if (b)
                {
                    var controllerName = actionContext.ControllerContext.ControllerDescriptor.ControllerName;
                    var actionName = actionContext.ActionDescriptor.ActionName;

                    var item = GetKey(controllerName, actionName, actionContext.ActionArguments);
                    var bRes = MemoryCacheHelper.Get(item);
                    if (bRes != null)
                    {
                        var bResult = (ResponseResult)bRes;
                        if (DateTimeOffset.Now.DateTime <= bResult.Expires.DateTime)
                        {
                            var res = actionContext.Request.CreateResponse();
                            res.Headers.CacheControl = new CacheControlHeaderValue
                            {
                                MaxAge = TimeSpan.FromSeconds(180),
                                Public = true,
                                NoCache = false
                            };
                            res.Content = new StringContent(bResult.ResponseContent, Encoding.UTF8, "application/json");
                            actionContext.Response = res;
                        }
                    }
                }
            }
            base.OnActionExecuting(actionContext);
        }

        /// <inheritdoc />
        /// <summary>
        /// 
        /// </summary>
        /// <param name="actionExecutedContext"></param>
        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            if (actionExecutedContext.Request.Method == HttpMethod.Get)
            {
                if (actionExecutedContext.Response.IsSuccessStatusCode)
                {
                    var controllerName = actionExecutedContext.ActionContext.ControllerContext.ControllerDescriptor.ControllerName;
                    var actionName = actionExecutedContext.ActionContext.ActionDescriptor.ActionName;
                    actionExecutedContext.Response.Headers.TryGetValues(CacheConstants.DegradationHttpHeader, out var dCache);

                    var cacheList = (dCache ?? new List<string>()).ToList();
                    var res = cacheList.Count > 0 ? cacheList[0] : actionExecutedContext.Response.Content.ReadAsStringAsync().Result;
                    var item = GetKey(controllerName, actionName, actionExecutedContext.ActionContext.ActionArguments);

                    var result = new ResponseResult { Expires = DateTimeOffset.Now.AddSeconds(Duration), ResponseContent = res };
                    var bRes = MemoryCacheHelper.Get(item);
                    if (bRes != null)
                    {
                        var bResult = (ResponseResult)bRes;
                        if (DateTimeOffset.Now.DateTime > bResult.Expires.DateTime)
                        {
                            MemoryCacheHelper.Set(item, result, TimeSpan.FromSeconds(Duration));
                        }
                    }
                    else
                    {
                        MemoryCacheHelper.Set(item, result, TimeSpan.FromSeconds(Duration));
                    }
                    if (actionExecutedContext.Response.Headers.Contains(CacheConstants.DegradationHttpHeader))
                    {
                        actionExecutedContext.Response.Headers.Remove(CacheConstants.DegradationHttpHeader);
                    }
                    //设置响应缓存30s
                    if (actionExecutedContext.Response.Headers.CacheControl == null)
                    {
                        actionExecutedContext.Response.Headers.CacheControl = new CacheControlHeaderValue();
                    }
                    actionExecutedContext.Response.Headers.CacheControl.MaxAge = TimeSpan.FromSeconds(30);
                    actionExecutedContext.Response.Headers.CacheControl.Public = true;
                    actionExecutedContext.Response.Headers.CacheControl.NoCache = false;
                }
            }
            base.OnActionExecuted(actionExecutedContext);
        }

        private string GetKey(string controllerName, string actionName, Dictionary<string, object> arguments)
        {
            var cacheKey = controllerName + "_" + actionName;
            var item = string.Empty;
            if (arguments != null)
            {
                foreach (var args in arguments)
                {
                    item += args.Value + "_";
                }
            }
            if (!string.IsNullOrEmpty(item))
            {
                item = cacheKey + ":" + item.TrimEnd('_');
            }
            else
            {
                item = cacheKey;
            }
            return item;
        }

        /// <summary>
        /// 响应内容缓存设置
        /// </summary>
        protected class ResponseResult
        {
            /// <summary>
            /// 缓存过期时间
            /// </summary>
            public DateTimeOffset Expires { get; set; }
            /// <summary>
            /// 响应内容
            /// </summary>
            public string ResponseContent { get; set; }
        }
    }
}