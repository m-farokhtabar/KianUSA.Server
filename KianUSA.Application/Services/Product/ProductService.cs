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
        /// <summary>
        /// Page Start With 0
        /// </summary>
        /// <param name="Groups"></param>
        /// <param name="Tags"></param>
        /// <param name="PageNumber"></param>
        /// <param name="PageCount"></param>
        /// <returns></returns>
        public async Task<ProductsWithTotalItemDto> GetByGroupAndTagsWithPaging(List<string> Groups, List<string> Tags, int PageNumber, int PageCount)
        {
            var Products = await Get();
            ConcurrentBag<ProductDto> Result = new();
            if ((Groups is null || Groups.Count == 0) && (Tags is null || Tags.Count == 0))
            {
                return new ProductsWithTotalItemDto() { TotalItems = Products is not null ? Products.Count : 0, Products = Products?.OrderBy(x => x.Order).Skip(PageNumber * PageCount).Take(PageCount).ToList() };
            }
            else
            {
                //Just Tags
                if (Tags?.Count > 0 && (Groups is null || Groups.Count == 0))
                {
                    Parallel.ForEach(Products, Prd =>
                    {
                        if (Prd.Tags?.Count > 0 && Prd.Tags.Count >= Tags.Count && Tags.All(x => Prd.Tags.Contains(x)))
                            Result.Add(Prd);
                    });
                    return new ProductsWithTotalItemDto() { TotalItems = Result is not null ? Result.Count : 0, Products = Result?.OrderBy(x => x.Order).Skip(PageNumber * PageCount).Take(PageCount).ToList() };
                }
                //Just Groups
                else if (Groups?.Count > 0 && (Tags is null || Tags.Count == 0))
                {
                    Parallel.ForEach(Products, Prd =>
                    {
                        if (Prd.Groups?.Count > 0 && Prd.Groups.Count >= Groups.Count && Groups.All(x=> Prd.Groups.Contains(x)))
                            Result.Add(Prd);
                    });
                    return new ProductsWithTotalItemDto() { TotalItems = Result is not null ? Result.Count : 0, Products = Result?.OrderBy(x => x.Order).Skip(PageNumber * PageCount).Take(PageCount).ToList() };
                }
                //Groups And Tags
                else
                {
                    Parallel.ForEach(Products, Prd =>
                    {
                        if (Prd.Groups?.Count > 0 && Prd.Groups.Count >= Groups.Count && 
                           Prd.Tags?.Count > 0 && Prd.Tags.Count >= Tags.Count &&
                           Groups.All(x => Prd.Groups.Contains(x)) &&
                           Tags.All(x => Prd.Tags.Contains(x)))                                                        
                            Result.Add(Prd);
                    });
                    return new ProductsWithTotalItemDto() { TotalItems = Result is not null ? Result.Count : 0, Products = Result?.OrderBy(x => x.Order).Skip(PageNumber * PageCount).Take(PageCount).ToList() };
                }
            }

        }

        public async Task<List<ProductWithSlugCatDto>> GetWithCatSlug()
        {
            using var Db = new Context();
            var Models = await Db.Products.Include(x => x.Categories).ToListAsync().ConfigureAwait(false);
            if (Models?.Count == 0)
                throw new ValidationException("There are not any products.");

            return MapToDtoWithSlugCat(Models, ManageImages.GetProductsImagesUrl(appSettings.WwwRootPath));
        }
        public async Task<List<ProductDto>> GetByFirstCategoryInOrder()
        {
            using var Db = new Context();
            var CategoryModel = await Db.Categories.OrderBy(x => x.Order).FirstOrDefaultAsync().ConfigureAwait(false);
            var Models = await Db.Products.Where(x => x.Categories.Any(x => x.CategoryId == CategoryModel.Id)).ToListAsync().ConfigureAwait(false);
            if (Models?.Count == 0)
                throw new ValidationException("In the category there are not any products.");

            return Mapto(Models, ManageImages.GetProductsImagesUrl(appSettings.WwwRootPath));
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
        public async Task<List<ProductDto>> GetByCategoryIds(List<Guid> Ids)
        {
            using var Db = new Context();
            var Models = await Db.Products.Include(x => x.Categories).Where(x => x.Categories.Any(x => Ids.Contains(x.CategoryId))).ToListAsync().ConfigureAwait(false);
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
                Inventory = Model.Inventory,
                CategoryIds = Model.Categories?.Select(x => x.CategoryId).ToList(),
                Tags = !string.IsNullOrWhiteSpace(Model.Tags) ? System.Text.Json.JsonSerializer.Deserialize<List<string>>(Model.Tags) : null,
                Groups = !string.IsNullOrWhiteSpace(Model.Groups) ? System.Text.Json.JsonSerializer.Deserialize<List<string>>(Model.Groups) : null,
                Factories = !string.IsNullOrWhiteSpace(Model.Factories) ? System.Text.Json.JsonSerializer.Deserialize<List<string>>(Model.Factories) : null
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
                CategorySlug = Model.Categories.Select(x => x.CategorySlug).ToList(),
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
                Inventory = Model.Inventory,
                CategoryIds = Model.Categories?.Select(x => x.CategoryId).ToList(),
                Tags = !string.IsNullOrWhiteSpace(Model.Tags) ? System.Text.Json.JsonSerializer.Deserialize<List<string>>(Model.Tags) : null,
                Groups = !string.IsNullOrWhiteSpace(Model.Groups) ? System.Text.Json.JsonSerializer.Deserialize<List<string>>(Model.Groups) : null,
                Factories = !string.IsNullOrWhiteSpace(Model.Factories) ? System.Text.Json.JsonSerializer.Deserialize<List<string>>(Model.Factories) : null
            };
        }
    }
}
