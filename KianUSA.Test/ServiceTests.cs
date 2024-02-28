using KianUSA.API.Configuration;
using KianUSA.Application.Services.Catalog;
using KianUSA.Application.Services.Email;
using KianUSA.Application.Services.PoData;
using KianUSA.Application.Services.UpdateDataByExcel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Xunit;
using Xunit.Priority;

namespace KianUSA.Test
{
    public class ServiceTests
    {
        private readonly ApplicationSettings ApplicationSettings;
        private const string Path = @"D:\Projects\Sepehr\KainUsa.Server.App\Data\Excels\7\";
        public ServiceTests()
        {
            ApplicationSettings = new()
            {
                WwwRootPath = @"D:\Projects\Sepehr\KainUsa.Server.App\KianUsa.Server.Source\KianUSA.API\wwwroot",
                StartIndexOfImageForUsingInCatalog = 10,
                PoDataPath = "D:\\Projects\\Sepehr\\KainUsa.Server.App\\Data\\Account\\Po.xlsx"
            };
        }
        [Trait("Creator", "Database Data")]
        [Fact, Priority(1)]
        public async Task ShouldBeCreatedAllCategoriesByExcelFile()
        {
            FileStream File = new(Path + "Categories.xlsx", FileMode.Open, FileAccess.Read);
            UpdateCategoryByExcelService Service = new();
            await Service.Update(File);
        }
        [Trait("Creator", "Database Data")]
        [Fact, Priority(2)]
        public async Task ShouldBeCreatedAllProductsByExcelFile()
        {
            FileStream File = new(Path + "Items.xlsx", FileMode.Open, FileAccess.Read);
            UpdateProductByExcelService Service = new(ApplicationSettings);
            await Service.Update(File);
        }
        [Trait("Creator", "Database Data")]
        [Fact, Priority(3)]
        public async Task ShouldBeCreatedAllRolesByExcelFile()
        {
            FileStream File = new(Path + "Roles.xlsx", FileMode.Open, FileAccess.Read);
            UpdateRoleByExcelService Service = new();
            await Service.Update(File);
        }
        [Trait("Creator", "Database Data")]
        [Fact, Priority(4)]
        public async Task ShouldBeCreatedAllUsersByExcelFile()
        {
            FileStream File = new(Path + "Users.xlsx", FileMode.Open, FileAccess.Read);
            UpdateUserByExcelService Service = new();
            await Service.Update(File);
        }
        [Trait("Creator", "Database Data")]
        [Fact, Priority(5)]
        public async Task ShouldBeCreatedAllFiltersByExcelFile()
        {
            FileStream File = new(Path + "Filters.xlsx", FileMode.Open, FileAccess.Read);
            UpdateFilterByExcelService Service = new();
            await Service.Update(File);
        }
        [Trait("Creator", "Database Data")]
        [Fact, Priority(6)]
        public async Task ShouldBeCreatedAllGroupsByExcelFile()
        {
            FileStream File = new(Path + "Groups.xlsx", FileMode.Open, FileAccess.Read);
            UpdateGroupByExcelService Service = new();
            await Service.Update(File);
        }
        [Trait("Creator", "Database Data")]
        [Fact, Priority(7)]
        public async Task ShouldBeCreatedAllPoDataByExcelFile()
        {
            FileStream File = new(Path + "PO.xlsx", FileMode.Open, FileAccess.Read);
            UpdatePoDataByExcelService Service = new();
            await Service.Update(File);
        }
        [Trait("Creator", "Catalogs")]
        [Fact, Priority(7)]
        public async Task CreateCatalogs()
        {
            CatalogService Service = new(ApplicationSettings);
            //await Service.Create();
            await Service.CreateWithLandedPrice(2, "152-group");
        }
        [Trait("Email", "ContactUs")]
        [Fact]
        public async Task Email_SendContactUs()
        {
            EmailService Srv = new EmailService(new ApplicationSettings()
            {
                ContactUsEmailSetting = "contact-us-email-setting"
            });
            await Srv.SendContactUs("Mehdi", "Ahmady", "123-123-1234", "Mehdi.fr@gmail.com", "woowo asajd asdjuad  asdjaksdjasd asd adjasdbnas daskjdasd asd a asdasd.");
        }

        [Trait("Po", "Excel")]
        [Fact]
        public void GetPoData()
        {
            PoDataService srv = new PoDataService(ApplicationSettings, new List<string>() { "admin" });
            srv.GetDataByExcel();
        }
        //[Trait("Generate", "Catalogs")]
        //[Fact, Priority(9)]
        //public async Task GenerateCatalogs()
        //{
        //    CatalogService Service = new(ApplicationSettings, new List<string>() { "admin" });
        //    List<int> Price = new() { 0, 1, 2 };
        //    await Service.Generate(new List<System.Guid>() {
        //        Guid.Parse("fcc4e1b5-ce9e-4735-8ab5-838c9a72cf32"),
        //        //Guid.Parse("f5271ee1-9eec-42c5-817a-b1e49bce06c2"),
        //        //Guid.Parse("cc16f531-c810-4722-ba15-26870b3890ba"),
        //        //Guid.Parse("927b72eb-fc6e-4e8e-a493-cc272f6d0e3a"),
        //        //Guid.Parse("ff43e0b4-bf3e-4c10-94a8-f1f4fad0683c")

        //    }, null, null, Price, 500);
        //}

    }
}
