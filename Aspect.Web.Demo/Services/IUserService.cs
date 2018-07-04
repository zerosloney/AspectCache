using System.Collections.Generic;
using Aspect.Web.Demo.Models;
using Aspect.Repository.Cache;

namespace Aspect.Web.Demo.Services
{
    public interface IUserService
    {
        //[StringCacheable("DEMO", "USER", 300)] //根据DEMO：USER：{page}_{pageSize}键获取缓存内容(String类型)
        [HashCacheable("DEMO", "USER", 300)] //根据DEMO：USER键，{page}_{pageSize}字段键获取缓存内容(Hash类型)
        IEnumerable<User> GetUserPageList(int page, int pageSize);
  
        //[StringCachePut("DEMO", "USER", 300, 0)] //根据DEMO：USER：{userId}键更新缓存内容
        [HashCachePut("DEMO", "USER", 300, null, 0)]
        User UpdateUserName(int userId, string userName);

        //[StringCacheEvit("DEMO", new[] { "USER" })] //删除DEMO：USER：*
        //[StringCacheEvit("DEMO", new[] { "USER" }, 0)] //删除DEMO：USER：{userId}
        //[HashCacheEvit("DEMO", "USER", null, 0)] //删除DEMO：USER键，{userId}字段键
        [HashCacheEvit("DEMO", "USER", null)] //删除DEMO：USER键
        int DeleteUser(int userId);

        //[StringCacheable("DEMO", "USER", 300)] //根据DEMO：USER：{userId}键获取缓存内容
        [HashCacheable("DEMO", "USER", 300, null, 0)]  //根据DEMO：USER键，{userId}字段键获取缓存内容(Hash类型)
        User GetUser(int userId);

    
        User AddUser(User user);
    }
}