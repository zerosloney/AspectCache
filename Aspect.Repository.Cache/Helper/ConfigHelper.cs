using System.Collections.Generic;
using System.Configuration;

namespace Aspect.Repository.Cache
{
    public static class ConfigHelper
    {
        public static string GetStrValue(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }

        public static int GetIntValue(string key, int defaultInt = 0)
        {
            int.TryParse(ConfigurationManager.AppSettings[key] ?? "0", out var v);
            return v;
        }

        public static bool GetBoolValue(string key)
        {
            bool.TryParse(ConfigurationManager.AppSettings[key] ?? "false", out var b);
            return b;
        }

        public static string GetSqlConnection(string key)
        {
            var con = ConfigurationManager.ConnectionStrings[key];
            return con?.ConnectionString;
        }

        public static string GetRedisConnection(string key)
        {
            return ConfigurationManager.AppSettings[key] ?? "";
        }

        public static int[] GetIntsValue(string key)
        {
            var str = GetStrValue(key);
            if (string.IsNullOrEmpty(str)) return new int[0];
            var arr = str.Split(',');
            var list = new List<int>();
            foreach (var i in arr)
            {
                int.TryParse(i, out var h);
                list.Add(h);
            }
            return list.ToArray();
        }
    }
}
