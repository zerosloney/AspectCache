using System;
using System.Collections;

namespace Aspect.Repository.Cache
{
    public class DefaultKeyGenerator : IKeyGenerator
    {
        private string Separator => ":";

        public string GetRightKey(object[] args, params int[] argsIndex)
        {
            if (args != null && args.Length > 0)
            {
                var result = string.Empty;
                var len = args.Length;
                if (argsIndex != null && argsIndex.Length > 0)
                {
                    foreach (var index in argsIndex)
                    {
                        if (index <= len - 1)
                        {
                            result += GetArgument(args[index]) + "_";
                        }
                    }
                }
                else
                {
                    foreach (var ag in args)
                    {
                        result += GetArgument(ag) + "_";
                    }
                }
                return result.TrimEnd('_');
            }
            return string.Empty;
        }

        public string GetLeftKey(string keyNamespace, string perfixKey)
        {
            if (string.IsNullOrEmpty(perfixKey))
            {
                return keyNamespace;
            }
            return keyNamespace + Separator + perfixKey;
        }

        public string GetKeyRegion(string keyNamespace, string perfixKey)
        {
            if (string.IsNullOrEmpty(perfixKey))
            {
                return keyNamespace;
            }
            return keyNamespace + Separator + perfixKey;
        }

        public string GetCacheKey(string left, string right)
        {
            if (string.IsNullOrEmpty(right))
            {
                return left;
            }
            return left + Separator + right;
        }

        string GetArgument(object argument)
        {
            var obj = argument.GetType();
            if (obj.IsArray && argument is IEnumerable enumerable)
            {
                var item = string.Empty;
                foreach (var ele in enumerable)
                {
                    item += ele + ".";
                }
                return item.TrimEnd('.');
            }
            if (obj == typeof(DateTime))
            {
                return ((DateTime)argument).ToString("yyyyMMddHHmmss");
            }
            if (obj == typeof(Boolean))
            {
                return ((bool)argument) ? "1" : "0";
            }
            return argument.ToString();
        }
    }
}
