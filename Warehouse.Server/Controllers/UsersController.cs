using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using Warehouse.Server.Data;
using Warehouse.Server.Identity;
using Warehouse.Server.Models;
using Warehouse.Server.ViewModels;

namespace Warehouse.Server.Controllers
{
    [Authorize]
    public class UsersController : ApiController
    {
        private readonly ApplicationUserManager userManager;
        private readonly IMongoContext context;

        public UsersController(ApplicationUserManager userManager, IMongoContext context)
        {
            this.userManager = userManager;
            this.context = context;
        }

        public IEnumerable<User> Get()
        {
            return context.Users.FindAll();
        }

        public async Task<HttpResponseMessage> Post([FromBody] User user)
        {
            if (user == null || string.IsNullOrEmpty(user.UserName) || string.IsNullOrEmpty(user.Password))
                return new HttpResponseMessage(HttpStatusCode.BadRequest);

            var appUser = new ApplicationUser { UserName = user.UserName };
            var result = userManager.Create(appUser, user.Password);
            if (result.Succeeded)
            {
                var result2 = await userManager.AddUserToRolesAsync(appUser.Id, user.Roles);
                if (result2.Succeeded)
                {
                    return Request.CreateResponse(HttpStatusCode.Created);
                }

                return ErrorResponse(result2.Errors.FirstOrDefault());
            }

            return ErrorResponse(result.Errors.FirstOrDefault());
        }

        public async Task<HttpResponseMessage> Put([FromBody] User user)
        {
            if (user == null || string.IsNullOrEmpty(user.UserName))
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }

            var appUser = await userManager.FindByNameAsync(user.UserName);
            if (appUser == null) return new HttpResponseMessage(HttpStatusCode.NotFound);

            var res1 = await userManager.RemoveUserFromRolesAsync(appUser.Id, appUser.Roles);
            if (res1.Succeeded)
            {
                var res2 = await userManager.AddUserToRolesAsync(appUser.Id, user.Roles);
                if (res2.Succeeded)
                {
                    return new HttpResponseMessage(HttpStatusCode.OK);
                }
            }
            return new HttpResponseMessage(HttpStatusCode.InternalServerError);
        }

        [Route("api/users/{login}/changePassword")]
        [HttpPost]
        public async Task<HttpResponseMessage> ChangePassword(string login, ChangePassword model)
        {
            if (model == null || string.IsNullOrEmpty(model.OldPassword) || string.IsNullOrEmpty(model.NewPassword))
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }

            if (userManager == null) return new HttpResponseMessage(HttpStatusCode.InternalServerError);

            var user = await userManager.FindByNameAsync(login);
            if (user == null) return new HttpResponseMessage(HttpStatusCode.NotFound);

            var res = await userManager.ChangePasswordAsync(user.Id, model.OldPassword, model.NewPassword);
            if (!res.Succeeded)
            {
                return ErrorResponse(res.Errors.FirstOrDefault());
            }

            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        private static HttpResponseMessage ErrorResponse(string error)
        {
            return new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent(error)
            };
        }
    }
}
