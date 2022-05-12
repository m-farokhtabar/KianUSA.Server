using KianUSA.API.Configuration;
using KianUSA.Application.SeedWork;
using KianUSA.Application.Services.UpdateDataByExcel;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace KianUSA.Test
{
    public class ServiceTests
    {
        private readonly ApplicationSettings ApplicationSettings;
        public ServiceTests()
        {
            ApplicationSettings = new()
            {
                WwwRootPath = @"F:\Project\SEPEHR\KianUsa\Project\KianUSA\KianUSA.API\wwwroot"
            };

        }
        [Fact]
        public async Task ShouldBeCreatedAllCategoriesByExcelFile()
        {
            FileStream File = new(@"F:\Project\SEPEHR\KianUsa\Files\Excel\Category.xlsx", FileMode.Open,FileAccess.Read);            
            UpdateCateogryByExcelService Service = new();
            await Service.Update(File);
        }
        [Fact]
        public async Task ShouldBeCreatedAllProductsByExcelFile()
        {
            FileStream File = new(@"F:\Project\SEPEHR\KianUsa\Files\Excel\All Items.xlsx", FileMode.Open, FileAccess.Read);
            UpdateProductByExcelService Service = new(ApplicationSettings);
            await Service.Update(File);
        }
        [Fact]
        public async Task ShouldBeCreatedAllRolesByExcelFile()
        {
            FileStream File = new(@"F:\Project\SEPEHR\KianUsa\Files\Excel\Role.xlsx", FileMode.Open, FileAccess.Read);
            UpdateRoleByExcelService Service = new();
            await Service.Update(File);
        }
        [Fact]
        public async Task ShouldBeCreatedAllUsersByExcelFile()
        {
            FileStream File = new(@"F:\Project\SEPEHR\KianUsa\Files\Excel\Users Access.xlsx", FileMode.Open, FileAccess.Read);
            UpdateUserByExcelService Service = new();
            await Service.Update(File);
        }
    }
}
