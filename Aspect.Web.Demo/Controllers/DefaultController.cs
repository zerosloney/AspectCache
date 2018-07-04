
using System.Web.Http;
using Aspect.Web.Demo.Models;
using Aspect.Web.Demo.Services;

namespace Aspect.Web.Demo.Controllers
{
    [RoutePrefix("api/user")]
    public class DefaultController : ApiController
    {

        public IUserService UserService { get; set; }

        [HttpGet]
        [Route("")]
        public IHttpActionResult GetUserList(int page, int pageSize = 10)
        {
            var userList = UserService.GetUserPageList(page, pageSize);
            return Json(userList);
        }

        [HttpGet]
        [Route("{userId:int}")]
        public IHttpActionResult GetUser(int userId)
        {
            var user = UserService.GetUser(userId);
            return Json(user);
        }


        [HttpPost]
        [Route("")]
        public IHttpActionResult PostUser(User user)
        {
            var entity = UserService.AddUser(user);
            return Json(entity);
        }


        [HttpPut]
        [Route("{userId:int}")]
        public IHttpActionResult PostUser(int userId, User user)
        {
            var entity = UserService.UpdateUserName(userId, user.Name);
            return Json(entity);
        }

        [HttpDelete]
        [Route("{userId:int}")]
        public IHttpActionResult DeleteUser(int userId)
        {
            var entity = UserService.DeleteUser(userId);
            return Json(entity);
        }
    }
}
