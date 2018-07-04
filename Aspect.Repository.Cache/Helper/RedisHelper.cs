using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using StackExchange.Redis;

namespace Aspect.Repository.Cache
{
    public class RedisHelper
    {
        private IConnectionMultiplexer ConnMulti { get; }
        private int DefalutDbNum { get; }
        /// <summary>
        /// 当前IDatabase
        /// </summary>
        private IDatabase Db => ConnMulti.GetDatabase(DefalutDbNum);
        /// <summary>
        /// 当前Server
        /// </summary>
        private IServer Server => ConnMulti.GetServer(ConnMulti.GetEndPoints()[0]);
        /// <summary>
        /// 构造函数
        /// </summary>
        public RedisHelper(IConnectionMultiplexer connMulti, ConfigurationOptions options)
        {
            ConnMulti = connMulti;
            DefalutDbNum = options.DefaultDatabase ?? 0;
        }

        public RedisHelper()
        {
            ConnMulti = RedisConnFactory.Instance;
            DefalutDbNum = RedisConnFactory.Options.DefaultDatabase ?? 0;
        }

        #region string操作
        public RedisValue StringGet(string cacheKey)
        {
            var result = Db.StringGet(cacheKey);
            return result;
        }

        public bool StringSet(string cacheKey, byte[] cacheValue, TimeSpan? expires)
        {
            return Db.StringSet(cacheKey, cacheValue, expires);
        }

        #endregion

        #region 键操作
        /// <summary>
        /// 删除缓存
        /// </summary>
        /// <param name="cacheKey"></param>
        /// <returns></returns>
        public bool KeyDelete(string cacheKey)
        {
            return Db.KeyDelete(cacheKey);
        }

        /// <summary>
        /// 根据模式删除所有的键
        /// </summary>
        /// <param name="keyPattern"></param>
        /// <returns></returns>
        public bool KeyDeletePattern(string keyPattern)
        {
            var keys = Server.Keys(DefalutDbNum, keyPattern);
            try
            {
                foreach (var key in keys)
                {
                    Db.KeyDelete(key);
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 设置键的过期
        /// </summary>
        /// <param name="cacheKey"></param>
        /// <param name="expires"></param>
        /// <returns></returns>
        public bool KeyExpire(string cacheKey, TimeSpan? expires)
        {
            return Db.KeyExpire(cacheKey, expires);
        }
        #endregion

        #region hash操作

        /// <summary>
        /// 获取哈希内的值
        /// </summary>
        /// <param name="cacheKey"></param>
        /// <param name="filedKey"></param>
        /// <returns></returns>
        public RedisValue HashGet(string cacheKey, string filedKey)
        {
            return Db.HashGet(cacheKey, filedKey);
        }
        /// <summary>
        /// 设置哈希内的值
        /// </summary>
        /// <param name="cacheKey"></param>
        /// <param name="filedKey"></param>
        /// <param name="cacheValue"></param>
        public void HashSet(string cacheKey, string filedKey, byte[] cacheValue)
        {
            Db.HashSet(cacheKey, filedKey, cacheValue);
        }

        /// <summary>
        /// 哈希删除
        /// </summary>
        /// <param name="cacheKey"></param>
        /// <param name="filedKey"></param>
        /// <returns></returns>
        public bool HashDelete(string cacheKey, string filedKey)
        {
           return Db.HashDelete(cacheKey, filedKey);
        }
        #endregion

        #region Lua脚本
        /// <summary>
        /// 
        /// </summary>
        /// <param name="script"></param>
        /// <param name="keys"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public RedisResult ExecLuaScript(string script, string[] keys, string[] values)
        {
            try
            {
                var sha1 = CalcLuaSha1(script);
                if (!Server.ScriptExists(sha1))
                {
                    Server.ScriptLoadAsync(script);
                }
                var rKeys = new List<RedisKey>();
                var rValues = new List<RedisValue>();
                foreach (var k in keys)
                {
                    rKeys.Add(k);
                }
                foreach (var v in values)
                {
                    rValues.Add(v);
                }

                var result = Db.ScriptEvaluate(sha1, rKeys.ToArray(), rValues.ToArray());
                if (!result.IsNull)
                {
                    return result;
                }
            }
            catch (Exception)
            {
                //
            }
            return null;
        }

        private static byte[] CalcLuaSha1(string script)
        {
            var sha1 = new SHA1CryptoServiceProvider();
            var bytesSha1In = Encoding.UTF8.GetBytes(script);
            return sha1.ComputeHash(bytesSha1In);
        }
        #endregion
    }
}
