## 基于方法的缓存拦截器

### 技术应用
1. castle aop
2. stackexchange.redis
3. autofac

### 应用场景

对同一请求Db的方法进行缓存，可以减少服务器压力、加快响应速度

查询接口：根据请求条件对返回结果进行缓存

更新接口：根据条件对指定缓存进行更新

清除接口：根据条件对指定缓存进行清楚


### 示例

String类型操作
```
        [StringCacheable("DEMO", "USER", 300)] //根据DEMO：USER：{page}_{pageSize}键获取缓存内容
        IEnumerable<User> GetUserPageList(int page, int pageSize);
  
        //[StringCachePut("DEMO", "USER", 300, 0)] //根据DEMO：USER：{userId}键更新缓存内容
        User UpdateUserName(int userId, string userName);

        //[StringCacheEvit("DEMO", new[] { "USER" })] //删除DEMO：USER：*
        [StringCacheEvit("DEMO", new[] { "USER" }, 0)] 删除DEMO：USER：{userId}
        int DeleteUser(int userId);

        [StringCacheable("DEMO", "USER", 300)] //根据DEMO：USER：{userId}键获取缓存内容
        User GetUser(int userId);
```

Hash类型操作

```
        [HashCacheable("DEMO", "USER", 300)] //根据DEMO：USER键，{page}_{pageSize}字段键获取缓存内容
        IEnumerable<User> GetUserPageList(int page, int pageSize);
  
        [HashCachePut("DEMO", "USER", 300, null, 0)]//根据DEMO：USER键，{userId}字段键获取缓存内容
        User UpdateUserName(int userId, string userName);

        //[HashCacheEvit("DEMO", "USER", null, 0)] //删除DEMO：USER键，{userId}字段键
        [HashCacheEvit("DEMO", "USER", null)] //删除DEMO：USER键
        int DeleteUser(int userId);

        [HashCacheable("DEMO", "USER", 300, null, 0)]  //根据DEMO：USER键，{userId}字段键获取缓存内容(Hash类型)
        User GetUser(int userId);

```