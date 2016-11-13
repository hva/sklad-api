using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;
using Microsoft.AspNet.Identity;
using Warehouse.Server.Data;
using Warehouse.Server.Identity;
using Warehouse.Server.Models;
using Warehouse.Server.ViewModels;

namespace Warehouse.Server.Controllers
{
    [Authorize]
    [RoutePrefix("api/users")]
    public class UsersController : ApiController
    {
        private readonly ApplicationUserManager userManager;
        private readonly IMongoContext context;

        public UsersController(ApplicationUserManager userManager, IMongoContext context)
        {
            this.userManager = userManager;
            this.context = context;
        }

        [HttpGet]
        [Route]
        public IHttpActionResult Get()
        {
            return Ok(context.Users.FindAll());
        }

        [HttpPost]
        [Route]
        public async Task<IHttpActionResult> Post([FromBody] User user)
        {
            if (user == null || string.IsNullOrEmpty(user.UserName) || string.IsNullOrEmpty(user.Password))
            {
                return BadRequest();
            }

            var appUser = new ApplicationUser { UserName = user.UserName };
            var result = userManager.Create(appUser, user.Password);
            if (result.Succeeded)
            {
                var result2 = await userManager.AddUserToRolesAsync(appUser.Id, user.Roles);
                if (result2.Succeeded)
                {
                    return Created(string.Empty, string.Empty);
                }

                return ErrorResponse(result2.Errors.FirstOrDefault());
            }

            return ErrorResponse(result.Errors.FirstOrDefault());
        }

        [HttpPut]
        [Route]
        public async Task<IHttpActionResult> Put([FromBody] User user)
        {
            if (user == null || string.IsNullOrEmpty(user.UserName))
            {
                return BadRequest();
            }

            var appUser = await userManager.FindByNameAsync(user.UserName);
            if (appUser == null)
            {
                return NotFound();
            }

            var res1 = await userManager.RemoveUserFromRolesAsync(appUser.Id, appUser.Roles);
            if (res1.Succeeded)
            {
                var res2 = await userManager.AddUserToRolesAsync(appUser.Id, user.Roles);
                if (res2.Succeeded)
                {
                    return Ok();
                }
            }

            return InternalServerError();
        }

        [HttpPost]
        [Route("{login}/changePassword")]
        public async Task<IHttpActionResult> ChangePassword(string login, ChangePassword model)
        {
            if (model == null || string.IsNullOrEmpty(model.OldPassword) || string.IsNullOrEmpty(model.NewPassword))
            {
                return BadRequest();
            }

            if (userManager == null)
            {
                return InternalServerError();
            }

            var user = await userManager.FindByNameAsync(login);
            if (user == null)
            {
                return NotFound();
            }

            var res = await userManager.ChangePasswordAsync(user.Id, model.OldPassword, model.NewPassword);
            if (!res.Succeeded)
            {
                return ErrorResponse(res.Errors.FirstOrDefault());
            }

            return Ok();
        }

        private static IHttpActionResult ErrorResponse(string error)
        {
            return new ResponseMessageResult(new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent(error)
            });
        }
    }
}
