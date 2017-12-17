using Nancy;
using Nancy.Testing;
using Sklad.Api.Modules.Auth;
using Xunit;

namespace Sklad.Api.Tests.Modules.Auth
{
    public class AuthModuleTests
    {
        private readonly Browser browser;

        public AuthModuleTests()
        {
            var bootstrapper = new ConfigurableBootstrapper(with => with
                .Module<AuthModule>()
            );
            browser = new Browser(bootstrapper);
        }

        [Fact(DisplayName = "create token - without parameters - should fail")]
        public void Test1()
        {
            var result = browser.Post("auth/tokens");
            var body = result.Body.AsString();

            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
            Assert.Contains("invalid grant type", body);
        }

        [Fact(DisplayName = "create token - with invalid grant type - should fail")]
        public void Test2()
        {
            var result = browser.Post("auth/tokens", with => with
                .FormValue("grant_type", "invalid")
            );
            var body = result.Body.AsString();

            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
            Assert.Contains("invalid grant type", body);
        }
    }
}
