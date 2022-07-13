using KianUSA.API.Configuration;
using KianUSA.Application.Services.Catalog;
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
                WwwRootPath = @"F:\Project\SEPEHR\KianUsa\Project\Server\KianUSA.API\wwwroot"
            };

        }
        [Trait("Creator","Database Data")]
        [Fact, Priority(1)]
        public async Task ShouldBeCreatedAllCategoriesByExcelFile()
        {
            FileStream File = new(@"F:\Project\SEPEHR\KianUsa\Files\Excel\1\Categories.xlsx", FileMode.Open,FileAccess.Read);            
            UpdateCateogryByExcelService Service = new();
            await Service.Update(File);
        }
        [Trait("Creator", "Database Data")]
        [Fact, Priority(2)]
        public async Task ShouldBeCreatedAllProductsByExcelFile()
        {
            FileStream File = new(@"F:\Project\SEPEHR\KianUsa\Files\Excel\1\Items.xlsx", FileMode.Open, FileAccess.Read);
            UpdateProductByExcelService Service = new(ApplicationSettings);
            await Service.Update(File);
        }
        [Trait("Creator", "Database Data")]
        [Fact, Priority(3)]
        public async Task ShouldBeCreatedAllRolesByExcelFile()
        {
            FileStream File = new(@"F:\Project\SEPEHR\KianUsa\Files\Excel\1\Roles.xlsx", FileMode.Open, FileAccess.Read);
            UpdateRoleByExcelService Service = new();
            await Service.Update(File);
        }
        [Trait("Creator", "Database Data")]
        [Fact, Priority(4)]
        public async Task ShouldBeCreatedAllUsersByExcelFile()
        {
            FileStream File = new(@"F:\Project\SEPEHR\KianUsa\Files\Excel\1\Users.xlsx", FileMode.Open, FileAccess.Read);
            UpdateUserByExcelService Service = new();
            await Service.Update(File);
        }
        [Trait("Creator", "Catalogs")]
        [Fact, Priority(3)]
        public async Task CreateCatalogs()
        {
            CatalogService Service = new(ApplicationSettings);
            await Service.Create();
        }
    }
}
