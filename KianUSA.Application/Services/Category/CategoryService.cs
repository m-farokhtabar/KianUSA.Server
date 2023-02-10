using KianUSA.Application.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using KianUSA.Application.SeedWork;
using KianUSA.Application.Services.Helper;

namespace KianUSA.Application.Services.Category
{
    using KianUSA.Application.Entity;
    using System.Collections.Concurrent;

    public class CategoryService
    {
        private readonly IApplicationSettings appSettings;
        public CategoryService(IApplicationSettings appSettings)
        {
            this.appSettings = appSettings;
        }
        public async Task<CategoryDto> GetFirstByOrder()
        {
            using var Db = new Context();
            var Model = await Db.Categories.OrderBy(x => x.Order).FirstOrDefaultAsync().ConfigureAwait(false);
            if (Model is null)
                throw new ValidationException("There are not any Categories.");
            var Children = await Db.CategoryCategories.Where(x => x.ParentCategoryId == Model.Id).ToListAsync();
            var ImagesUrl = ManageImages.GetCategoryImagesUrl(Model.Name, appSettings.WwwRootPath);
            return MapTo(Model, ImagesUrl, Children);
        }
        public async Task<List<CategoryDto>> GetBySlugWithChildren(string Slug)
        {
            using var Db = new Context();
            var Model = await Db.Categories.Where(x => x.Slug == Slug.ToLower().Trim()).FirstOrDefaultAsync().ConfigureAwait(false);
            if (Model is null)
                throw new ValidationException("Category does not exist.");
            var Children = await Db.CategoryCategories.Where(x => x.ParentCategorySlug == Slug.ToLower().Trim()).ToListAsync();
            var ChildrenIds = Children.ConvertAll(x => x.CategoryId);
            var Models = await Db.Categories.Where(x => ChildrenIds.Contains(x.Id)).ToListAsync().ConfigureAwait(false);
            if (Models?.Count > 0)
            {
                foreach (var ChildModel in Models)
                {
                    var ChildData = Children.FirstOrDefault(x => x.CategoryId == ChildModel.Id);
                    ChildModel.Order = ChildData.Order;
                }
            }
            else
                Models = new List<Category>();
            Model.Order = -1;
            Models.Add(Model);


            var AllImagesUrl = ManageImages.GetCategoriesImagesUrl(appSettings.WwwRootPath);
            ConcurrentBag<CategoryDto> Result = new();
            Parallel.ForEach(Models, Model =>
            {
                List<string> ImagesUrl = null;
                if (AllImagesUrl?.Count > 0)
                    ImagesUrl = AllImagesUrl.Where(x => x.StartsWith("/Images/Products/" + ManageImages.GetStartNameOfCategoryImageFileName(Model.Name))).ToList();
                Result.Add(MapTo(Model, ImagesUrl, null));
            });
            return Result.ToList();
        }
        public async Task<CategoryDto> Get(string Slug)
        {
            using var Db = new Context();
            var Model = await Db.Categories.Where(x => x.Slug == Slug.ToLower().Trim()).FirstOrDefaultAsync().ConfigureAwait(false);
            if (Model is null)
                throw new ValidationException("Category does not exist.");
            var Children = await Db.CategoryCategories.Where(x => x.ParentCategorySlug == Slug.ToLower().Trim()).ToListAsync();
            var ImagesUrl = ManageImages.GetCategoryImagesUrl(Model.Name, appSettings.WwwRootPath);
            return MapTo(Model, ImagesUrl, Children);
        }
        public async Task<CategoryDto> Get(Guid Id)
        {
            using var Db = new Context();
            var Model = await Db.Categories.FindAsync(Id).ConfigureAwait(false);
            if (Model is null)
                throw new ValidationException("Category does not exist.");
            var Children = await Db.CategoryCategories.Where(x => x.ParentCategoryId == Id).ToListAsync();
            var ImagesUrl = ManageImages.GetCategoryImagesUrl(Model.Name, appSettings.WwwRootPath);
            return MapTo(Model, ImagesUrl, Children);
        }
        public async Task<List<CategoryDto>> Get()
        {
            using var Db = new Context();
            var Models = await Db.Categories.ToListAsync().ConfigureAwait(false);
            if (Models?.Count == 0)
                throw new ValidationException("There are not any Category.");
            var AllImagesUrl = ManageImages.GetCategoriesImagesUrl(appSettings.WwwRootPath);
            ConcurrentBag<CategoryDto> Result = new();
            Parallel.ForEach(Models, Model =>
            {
                List<string> ImagesUrl = null;
                if (AllImagesUrl?.Count > 0)
                    ImagesUrl = AllImagesUrl.Where(x => x.StartsWith("/Images/Products/" + ManageImages.GetStartNameOfCategoryImageFileName(Model.Name))).ToList();
                Result.Add(MapTo(Model, ImagesUrl, null));
            });
            return Result.OrderBy(x => x.Order).ToList();
        }        
        public async Task<List<CategoryDto>> GetWithChildren()
        {
            using var Db = new Context();
            var Models = await Db.Categories.Include(x => x.Parents).ToListAsync().ConfigureAwait(false);
            if (Models?.Count == 0)
                throw new ValidationException("There are not any Category.");
            var AllImagesUrl = ManageImages.GetCategoriesImagesUrl(appSettings.WwwRootPath);
            ConcurrentBag<CategoryDto> Result = new();
            Parallel.ForEach(Models, Model =>
            {
                List<string> ImagesUrl = null;
                if (AllImagesUrl?.Count > 0)
                    ImagesUrl = AllImagesUrl.Where(x => x.StartsWith("/Images/Products/" + ManageImages.GetStartNameOfCategoryImageFileName(Model.Name))).ToList();
                var s = Models.Where(x => x.Parents.Any(x => x.ParentCategoryId == Model.Id)).Select(y => y.Parents.Where(y => y.ParentCategoryId == Model.Id).First()).ToList();
                Result.Add(MapTo(Model, ImagesUrl, s));
            });
            return Result.OrderBy(x => x.Order).ToList();
        }

        public async Task<List<CategoryDto>> GetByTags(List<string> Tags)
        {
            List<CategoryDto> Cats = await GetWithChildren();
            ConcurrentBag<CategoryDto> Result = new();
            if (Tags?.Count > 0)
            {                                
                Parallel.ForEach(Cats, Cat =>
                {
                    if (Cat.Tags?.Count > 0 && Cat.Tags.Count >= Tags.Count && Cat.Tags.All(x => Tags.Contains(x)))
                    {
                        //Super Cat
                        if (Cat.Children?.Count > 0)
                        {
                            var subCats = Cats.Where(x => Cat.Children.Select(x => x.Id).Contains(x.Id)).ToList();
                            foreach (var subCat in subCats)
                                Result.Add(subCat);
                        }
                        //Cat
                        else
                            Result.Add(Cat);
                    }
                });
            }
            else
            {
                Parallel.ForEach(Cats, Cat =>
                {
                    //Super Cat
                    if (Cat.Children?.Count > 0)
                    {
                        var subCats = Cats.Where(x => Cat.Children.Select(x => x.Id).Contains(x.Id)).ToList();
                        foreach (var subCat in subCats)
                            Result.Add(subCat);
                    }
                    //Cat
                    else
                        Result.Add(Cat);
                });
            }
            //Delete Repitive data !!!
            return Result.OrderBy(x => x.Order).Distinct().ToList();
        }

        public async Task<List<CategoryShortDto>> GetShortData()
        {
            using var Db = new Context();
            var Result = await Db.Categories.Select(x => new CategoryShortDto { Id = x.Id, Name = x.Name, Slug = x.Slug, Order = x.Order, ShortDescription = x.ShortDescription }).ToListAsync().ConfigureAwait(false);
            if (Result?.Count == 0)
                throw new ValidationException("There are not any Category.");
            return Result;
        }

        private CategoryDto MapTo(Category Model, List<string> ImagesUrl, List<CategoryCategory> Children)
        {
            return new()
            {
                Id = Model.Id,
                Name = Model.Name,
                Slug = Model.Slug,
                Order = Model.Order,
                Description = Model.Description,
                ShortDescription = Model.ShortDescription,
                Parameters = !string.IsNullOrWhiteSpace(Model.Parameter) ? (System.Text.Json.JsonSerializer.Deserialize<List<CategoryParameter>>(Model.Parameter))?.Where(x => x.IsFeature == false)?.ToList().Select(x => new CategoryParameterDto { Name = x.Name, Value = x.Value }).ToList() : null,
                Features = !string.IsNullOrWhiteSpace(Model.Parameter) ? (System.Text.Json.JsonSerializer.Deserialize<List<CategoryParameter>>(Model.Parameter))?.Where(x => x.IsFeature == true)?.ToList().Select(x => new CategoryParameterDto { Name = x.Name, Value = x.Value }).ToList() : null,
                ImagesUrl = ImagesUrl,
                Children = Children?.ConvertAll(x => new ChildCategoryDto() { Id = x.CategoryId, Slug = x.CategorySlug, Order = x.Order }),
                PublishedCatalogType = (PublishedCatalogTypeDto)(int)Model.PublishedCatalogType,
                Tags = !string.IsNullOrWhiteSpace(Model.Tags) ? System.Text.Json.JsonSerializer.Deserialize<List<string>>(Model.Tags) : null,
                Securities = !string.IsNullOrWhiteSpace(Model.Security) ? System.Text.Json.JsonSerializer.Deserialize<List<string>>(Model.Security) : null
            };
        }
    }
}
