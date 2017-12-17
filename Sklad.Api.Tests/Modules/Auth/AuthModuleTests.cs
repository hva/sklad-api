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

        [Fact]
        public void CreateToken_WithoutParameters_ShouldFail_Test()
        {
            var result = browser.Post("auth/tokens");
            var body = result.Body.AsString();

            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
            Assert.Contains("invalid grant type", body);
        }

        [Fact]
        public void CreateToken_WithInvalidGrantType_ShouldFail_Test()
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
