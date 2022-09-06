using System.Threading.Tasks;
using AbpVuenetcore.Models.TokenAuth;
using AbpVuenetcore.Web.Controllers;
using Shouldly;
using Xunit;

namespace AbpVuenetcore.Web.Tests.Controllers
{
    public class HomeController_Tests: AbpVuenetcoreWebTestBase
    {
        [Fact]
        public async Task Index_Test()
        {
            await AuthenticateAsync(null, new AuthenticateModel
            {
                UserNameOrEmailAddress = "admin",
                Password = "123qwe"
            });

            //Act
            var response = await GetResponseAsStringAsync(
                GetUrl<HomeController>(nameof(HomeController.Index))
            );

            //Assert
            response.ShouldNotBeNullOrEmpty();
        }
    }
}