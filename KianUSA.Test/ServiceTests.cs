using KianUSA.API.Configuration;
using KianUSA.Application.Services.Catalog;
using KianUSA.Application.Services.Email;
using KianUSA.Application.Services.UpdateDataByExcel;
using System.IO;
using System.Threading.Tasks;
using Xunit;
using Xunit.Priority;

namespace KianUSA.Test
{
    public class ServiceTests
    {
        private readonly ApplicationSettings ApplicationSettings;
        public ServiceTests()
        {
            ApplicationSettings = new()
            {
                WwwRootPath = @"F:\Project\SEPEHR\KianUsa\Project\Server\KianUSA.API\wwwroot",
                StartIndexOfImageForUsingInCatalog = 10
            };
        }
        [Trait("Creator","Database Data")]
        [Fact, Priority(1)]
        public async Task ShouldBeCreatedAllCategoriesByExcelFile()
        {
            FileStream File = new(@"F:\Project\SEPEHR\KianUsa\Files\Excel\6\Categories.xlsx", FileMode.Open,FileAccess.Read);            
            UpdateCateogryByExcelService Service = new();
            await Service.Update(File);
        }
        [Trait("Creator", "Database Data")]
        [Fact, Priority(2)]
        public async Task ShouldBeCreatedAllProductsByExcelFile()
        {
            FileStream File = new(@"F:\Project\SEPEHR\KianUsa\Files\Excel\6\Items.xlsx", FileMode.Open, FileAccess.Read);
            UpdateProductByExcelService Service = new(ApplicationSettings);
            await Service.Update(File);
        }
        [Trait("Creator", "Database Data")]
        [Fact, Priority(3)]
        public async Task ShouldBeCreatedAllRolesByExcelFile()
        {
            FileStream File = new(@"F:\Project\SEPEHR\KianUsa\Files\Excel\6\Roles.xlsx", FileMode.Open, FileAccess.Read);
            UpdateRoleByExcelService Service = new();
            await Service.Update(File);
        }
        [Trait("Creator", "Database Data")]
        [Fact, Priority(4)]
        public async Task ShouldBeCreatedAllUsersByExcelFile()
        {
            FileStream File = new(@"F:\Project\SEPEHR\KianUsa\Files\Excel\6\Users.xlsx", FileMode.Open, FileAccess.Read);
            UpdateUserByExcelService Service = new();
            await Service.Update(File);
        }
        [Trait("Creator", "Database Data")]
        [Fact, Priority(5)]
        public async Task ShouldBeCreatedAllFiltersByExcelFile()
        {
            FileStream File = new(@"F:\Project\SEPEHR\KianUsa\Files\Excel\6\Filters.xlsx", FileMode.Open, FileAccess.Read);
            UpdateFilterByExcelService Service = new();
            await Service.Update(File);
        }
        [Trait("Creator", "Database Data")]
        [Fact, Priority(6)]
        public async Task ShouldBeCreatedAllGroupsByExcelFile()
        {
            FileStream File = new(@"F:\Project\SEPEHR\KianUsa\Files\Excel\6\Groups.xlsx", FileMode.Open, FileAccess.Read);
            UpdateGroupByExcelService Service = new();
            await Service.Update(File);
        }
        [Trait("Creator", "Catalogs")]
        [Fact, Priority(7)]
        public async Task CreateCatalogs()
        {
            CatalogService Service = new(ApplicationSettings);
            await Service.Create();
            await Service.CreateWithLandedPrice(2, "152-group");
        }
        [Trait("Email","ContactUs")]
        [Fact]
        public async Task Email_SendContactUs()
        {
            EmailService Srv = new EmailService(new ApplicationSettings()
            {
                ContactUsEmailSetting = "contact-us-email-setting"
            });
            await Srv.SendContactUs("Mehdi", "Ahmady", "123-123-1234", "Mehdi.fr@gmail.com", "woowo asajd asdjuad  asdjaksdjasd asd adjasdbnas daskjdasd asd a asdasd.");
        }
    }
}
