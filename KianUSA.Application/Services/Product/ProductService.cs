using KianUSA.Application.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using KianUSA.Application.Services.Helper;
using KianUSA.Application.SeedWork;

namespace KianUSA.Application.Services.Product
{
    using KianUSA.Application.Entity;
    using Microsoft.Extensions.Logging;
    using System.Collections.Concurrent;

    public class ProductService
    {
        private readonly IApplicationSettings appSettings;
        private readonly ILogger logger;
        public ProductService(IApplicationSettings appSettings)
        {
            this.appSettings = appSettings;            
        }
        public async Task<ProductDto> Get(Guid Id)
        {
            using var Db = new Context();
            var Model = await Db.Products.FindAsync(Id).ConfigureAwait(false);
            if (Model is null)
                throw new ValidationException("Product does not exist.");
            
            var ImagesUrl = ManageImages.GetProductImagesUrl(Model.Name, appSettings.WwwRootPath);
            return MapTo(Model, ImagesUrl);
        }
        public async Task<ProductDto> Get(string Slug)
        {
            using var Db = new Context();
            var Model = await Db.Products.FirstOrDefaultAsync(x => x.Slug == Slug.ToLower()).ConfigureAwait(false);
            if (Model is null)
                throw new ValidationException("Product does not exist.");
            
            var ImagesUrl = ManageImages.GetProductImagesUrl(Model.Name, appSettings.WwwRootPath);
            return MapTo(Model, ImagesUrl);
        }
        public async Task<List<ProductDto>> Get()
        {
            using var Db = new Context();
            var Models = await Db.Products.ToListAsync().ConfigureAwait(false);
            if (Models?.Count == 0)
                throw new ValidationException("There are not any products.");
            
            return Mapto(Models, ManageImages.GetProductsImagesUrl(appSettings.WwwRootPath));
        }
        public async Task<List<ProductWithSlugCatDto>> GetWithCatSlug()
        {
            using var Db = new Context();
            var Models = await Db.Products.Include(x=>x.Categories).ToListAsync().ConfigureAwait(false);
            if (Models?.Count == 0)
                throw new ValidationException("There are not any products.");

            return MapToDtoWithSlugCat(Models, ManageImages.GetProductsImagesUrl(appSettings.WwwRootPath));
        }
        public async Task<List<ProductDto>> GetByCategoryId(Guid CategoryId)
        {
            using var Db = new Context();
            var Models = await Db.Products.Where(x => x.Categories.Any(x => x.CategoryId == CategoryId)).ToListAsync().ConfigureAwait(false);
            if (Models?.Count == 0)
                throw new ValidationException("In the category there are not any products.");
            
            return Mapto(Models, ManageImages.GetProductsImagesUrl(appSettings.WwwRootPath));
        }
        public async Task<List<ProductDto>> GetByCategorySlug(string CategorySlug)
        {
            using var Db = new Context();
            var Models = await Db.Products.Where(x => x.Categories.Any(x => x.CategorySlug == CategorySlug.ToLower())).ToListAsync().ConfigureAwait(false);            
            if (Models?.Count == 0)
                throw new ValidationException("In the category there are not any products.");

            return Mapto(Models, ManageImages.GetProductsImagesUrl(appSettings.WwwRootPath));
        }

        private List<ProductDto> Mapto(List<Product> Models, List<string> AllImagesUrl)
        {
            ConcurrentBag<ProductDto> Result = new();
            Parallel.ForEach(Models, Model =>
            {
                List<string> ImagesUrl = null;
                if (AllImagesUrl?.Count > 0)
                    ImagesUrl = AllImagesUrl.Where(x => x.StartsWith("/Images/Products/" + ManageImages.GetStartNameOfProductImageFileName(Model.Name))).ToList();

                Result.Add(MapTo(Model, ImagesUrl));
            });
            return Result.ToList();
        }

        private ProductDto MapTo(Product Model, List<string> ImagesUrl)
        {
            return new ProductDto()
            {
                BoxD = Model.BoxD,
                BoxH = Model.BoxH,
                BoxW = Model.BoxW,
                Cube = Model.Cube,
                D = Model.D,
                Description = Model.Description,
                H = Model.H,
                Id = Model.Id,
                IsGroup = Model.IsGroup,
                Name = Model.Name,
                Order = Model.Order,
                ShortDescription = Model.ShortDescription,
                W = Model.W,
                Weight = Model.Weight,
                WHQTY = Model.WHQTY,
                Prices = !string.IsNullOrWhiteSpace(Model.Price) ? System.Text.Json.JsonSerializer.Deserialize<List<ProductPriceDto>>(Model.Price) : null,
                Securities = Tools.SecurityToList(Model.Security),
                ImagesUrls = ImagesUrl,
                Slug = Model.Slug,
                Inventory = Model.Inventory
            };
        }
        private List<ProductWithSlugCatDto> MapToDtoWithSlugCat(List<Product> Models, List<string> AllImagesUrl)
        {
            ConcurrentBag<ProductWithSlugCatDto> Result = new();
            Parallel.ForEach(Models, Model =>
            {
                List<string> ImagesUrl = null;
                if (AllImagesUrl?.Count > 0)
                    ImagesUrl = AllImagesUrl.Where(x => x.StartsWith("/Images/Products/" + ManageImages.GetStartNameOfProductImageFileName(Model.Name))).ToList();

                Result.Add(MapToDtoWithSlugCat(Model, ImagesUrl));
            });
            return Result.ToList();
        }
        private ProductWithSlugCatDto MapToDtoWithSlugCat(Product Model, List<string> ImagesUrl)
        {
            return new ProductWithSlugCatDto()
            {
                CategorySlug = Model.Categories.Select(x=>x.CategorySlug).ToList(),
                BoxD = Model.BoxD,
                BoxH = Model.BoxH,
                BoxW = Model.BoxW,
                Cube = Model.Cube,
                D = Model.D,
                Description = Model.Description,
                H = Model.H,
                Id = Model.Id,
                IsGroup = Model.IsGroup,
                Name = Model.Name,
                Order = Model.Order,
                ShortDescription = Model.ShortDescription,
                W = Model.W,
                Weight = Model.Weight,
                WHQTY = Model.WHQTY,
                Prices = !string.IsNullOrWhiteSpace(Model.Price) ? System.Text.Json.JsonSerializer.Deserialize<List<ProductPriceDto>>(Model.Price) : null,
                Securities = Tools.SecurityToList(Model.Security),
                ImagesUrls = ImagesUrl,
                Slug = Model.Slug,
                Inventory = Model.Inventory
            };
        }
    }
}
