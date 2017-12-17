using Nancy;
using Nancy.Testing;
using Sklad.Api.Modules;
using Xunit;

namespace Sklad.Api.Tests.Modules
{
    public class HomeModuleTests
    {
        private readonly Browser browser;

        public HomeModuleTests()
        {
            var bootstrapper = new ConfigurableBootstrapper(with => with
                .Module<HomeModule>()
            );
            browser = new Browser(bootstrapper);
        }

        [Fact]
        public void ShouldSucceedTest()
        {
            var result = browser.Get("/");
            var body = result.Body.AsString();

            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.Equal("application/json; charset=utf-8", result.ContentType);
            Assert.Contains("\"version\":", body);
            Assert.Contains("\"commit\":", body);
        }
    }
}

