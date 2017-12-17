using Nancy;
using Nancy.ModelBinding;

namespace Sklad.Api.Modules.Auth
{
    public class AuthModule : NancyModule
    {
        public AuthModule()
        {
            Post["auth/tokens"] = _ =>
            {
                this.BindAndValidate<TokenRequest>();
                if (!ModelValidationResult.IsValid)
                {
                    return Response.AsJson(ModelValidationResult.FormattedErrors, HttpStatusCode.BadRequest);
                }

                return Response.AsJson(new TokenResponse());
            };
        }
    }
}
