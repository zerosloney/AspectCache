using System.Collections.Generic;
using System.Data;
using Aspect.Web.Demo.Models;
using Dapper;

namespace Aspect.Web.Demo.Services
{
    public class UserService : IUserService
    {
        private readonly IDbConnection _dbConnection;

        public UserService(IDbConnection connection)
        {
            _dbConnection = connection;
        }

        public IEnumerable<User> GetUserPageList(int page, int pageSize)
        {
           return _dbConnection.GetListPaged<User>(page, pageSize, null, null);
        }

        public User AddUser(User user)
        {
            var newId = _dbConnection.Insert(user);
            user.Id = newId ?? 0;
            if (user.Id > 0)
            {
                return user;
            }
            return null;
        }

        public User UpdateUserName(int userId, string userName)
        {
            var user = GetUser(userId);
            if (user != null)
            {
                user.Name = userName;
                _dbConnection.Update(user);
            }
            return user;
        }

        public int DeleteUser(int userId)
        {
            return _dbConnection.Delete<User>(userId);
        }

        public User GetUser(int userId)
        {
            return _dbConnection.Get<User>(userId);
        }
    }
}