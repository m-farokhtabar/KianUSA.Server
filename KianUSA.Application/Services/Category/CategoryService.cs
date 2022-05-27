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
    using Microsoft.Extensions.Logging;
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
            var Model = await Db.Categories.OrderBy(x=>x.Order).FirstOrDefaultAsync().ConfigureAwait(false);
            if (Model is null)
                throw new ValidationException("There are not any Categories.");
            var ImagesUrl = ManageImages.GetCategoryImagesUrl(Model.Name, appSettings.WwwRootPath);
            return MapTo(Model, ImagesUrl);
        }

        public async Task<CategoryDto> Get(string Slug)
        {
            using var Db = new Context();
            var Model = await Db.Categories.Where(x => x.Slug == Slug.ToLower().Trim()).FirstOrDefaultAsync().ConfigureAwait(false);
            if (Model is null)
                throw new ValidationException("Category does not exist.");
            var ImagesUrl = ManageImages.GetCategoryImagesUrl(Model.Name, appSettings.WwwRootPath);
            return MapTo(Model, ImagesUrl);
        }
        public async Task<CategoryDto> Get(Guid Id)
        {
            using var Db = new Context();
            var Model = await Db.Categories.FindAsync(Id).ConfigureAwait(false);
            if (Model is null)
                throw new ValidationException("Category does not exist.");
            var ImagesUrl = ManageImages.GetCategoryImagesUrl(Model.Name, appSettings.WwwRootPath);
            return MapTo(Model, ImagesUrl);
        }
        public async Task<List<CategoryDto>> Get()
        {
            using var Db = new Context();
            var Models = await Db.Categories.OrderBy(x => x.Order).ToListAsync().ConfigureAwait(false);
            if (Models?.Count == 0)
                throw new ValidationException("There are not any Category.");
            var AllImagesUrl = ManageImages.GetCategoriesImagesUrl(appSettings.WwwRootPath);
            ConcurrentBag<CategoryDto> Result = new();
            Parallel.ForEach(Models, Model =>
            {
                List<string> ImagesUrl = null;
                if (AllImagesUrl?.Count > 0)
                    ImagesUrl = AllImagesUrl.Where(x => x.StartsWith("/Images/Products/" + ManageImages.GetStartNameOfCategoryImageFileName(Model.Name))).ToList();
                Result.Add(MapTo(Model, ImagesUrl));
            });
            return Result.ToList();
        }
        public async Task<List<CategoryShortDto>> GetShortData()
        {
            using var Db = new Context();
            var Result = await Db.Categories.Select(x => new CategoryShortDto { Id = x.Id, Name = x.Name, Slug = x.Slug, Order = x.Order, ShortDescription = x.ShortDescription }).ToListAsync().ConfigureAwait(false);
            if (Result?.Count == 0)
                throw new ValidationException("There are not any Category.");
            return Result;
        }

        private CategoryDto MapTo(Category Model, List<string> ImagesUrl)
        {
            return new()
            {
                Id = Model.Id,
                Name = Model.Name,
                Slug = Model.Slug,
                Order = Model.Order,
                Description = Model.Description,
                ShortDescription = Model.ShortDescription,
                Parameters = !string.IsNullOrWhiteSpace(Model.Parameter) ? System.Text.Json.JsonSerializer.Deserialize<List<CategoryParameterDto>>(Model.Parameter) : null,
                ImagesUrl = ImagesUrl
            };
        }
    }
}
