using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;
using Microsoft.AspNet.Identity;
using Warehouse.Server.Data;
using Warehouse.Server.Identity;
using Warehouse.Server.Logger;
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
        private readonly ILogger logger;

        public UsersController(ApplicationUserManager userManager, IMongoContext context, ILogger logger)
        {
            this.userManager = userManager;
            this.context = context;
            this.logger = logger;
        }

        [HttpGet]
        [Route]
        public IHttpActionResult Get()
        {
            logger.TrackRequest(Request, true);
            return Ok(context.Users.FindAll());
        }

        [HttpPost]
        [Route]
        public async Task<IHttpActionResult> Post([FromBody] User user)
        {
            if (user == null || string.IsNullOrEmpty(user.UserName) || string.IsNullOrEmpty(user.Password))
            {
                logger.TrackRequest(Request, false);
                return BadRequest();
            }

            var appUser = new ApplicationUser { UserName = user.UserName };
            var result = userManager.Create(appUser, user.Password);
            if (result.Succeeded)
            {
                var result2 = await userManager.AddUserToRolesAsync(appUser.Id, user.Roles);
                if (result2.Succeeded)
                {
                    logger.TrackRequest(Request, true);
                    return Created(string.Empty, string.Empty);
                }

                logger.TrackRequest(Request, false);
                return ErrorResponse(result2.Errors.FirstOrDefault());
            }

            logger.TrackRequest(Request, false);
            return ErrorResponse(result.Errors.FirstOrDefault());
        }

        [HttpPut]
        [Route]
        public async Task<IHttpActionResult> Put([FromBody] User user)
        {
            if (user == null || string.IsNullOrEmpty(user.UserName))
            {
                logger.TrackRequest(Request, false);
                return BadRequest();
            }

            var appUser = await userManager.FindByNameAsync(user.UserName);
            if (appUser == null)
            {
                logger.TrackRequest(Request, false);
                return NotFound();
            }

            var res1 = await userManager.RemoveUserFromRolesAsync(appUser.Id, appUser.Roles);
            if (res1.Succeeded)
            {
                var res2 = await userManager.AddUserToRolesAsync(appUser.Id, user.Roles);
                if (res2.Succeeded)
                {
                    logger.TrackRequest(Request, true);
                    return Ok();
                }
            }

            logger.TrackRequest(Request, false);
            return InternalServerError();
        }

        [HttpPost]
        [Route("{login}/changePassword")]
        public async Task<IHttpActionResult> ChangePassword(string login, ChangePassword model)
        {
            if (model == null || string.IsNullOrEmpty(model.OldPassword) || string.IsNullOrEmpty(model.NewPassword))
            {
                logger.TrackRequest(Request, false);
                return BadRequest();
            }

            if (userManager == null)
            {
                logger.TrackRequest(Request, false);
                return InternalServerError();
            }

            var user = await userManager.FindByNameAsync(login);
            if (user == null)
            {
                logger.TrackRequest(Request, false);
                return NotFound();
            }

            var res = await userManager.ChangePasswordAsync(user.Id, model.OldPassword, model.NewPassword);
            if (!res.Succeeded)
            {
                logger.TrackRequest(Request, false);
                return ErrorResponse(res.Errors.FirstOrDefault());
            }

            logger.TrackRequest(Request, true);
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
