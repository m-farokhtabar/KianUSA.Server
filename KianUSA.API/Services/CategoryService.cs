using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using KianUSA.Application.SeedWork;
using System.Threading.Tasks;
using System.Collections.Generic;
using KianUSA.Application.Services.Category;
using System;
using KianUSA.API.Helper;

namespace KianUSA.API.Services
{
    public class CategoryService : CategorySrv.CategorySrvBase
    {
        private readonly Application.Services.Category.CategoryService service;

        public CategoryService(IApplicationSettings applicationSettings)
        {
            service = new(applicationSettings);
        }
        public override async Task<CategoriesResponseMessage> GetAll(Empty request, ServerCallContext context)
        {            
            List<CategoryDto> categories = await service.Get().ConfigureAwait(false);
            CategoriesResponseMessage result = new();
            foreach (var category in categories)
                result.Categories.Add(MapToCategory(category));
            return result;
        }

        public override async Task<CategoriesShortDataResponseMessage> GetAllShortData(Empty request, ServerCallContext context)
        {
            List<CategoryShortDto> categories = await service.GetShortData().ConfigureAwait(false);
            CategoriesShortDataResponseMessage result = new();
            foreach (var category in categories)
                result.Categories.Add(MapToCategoryShortData(category));
            return result;            
        }

        public override async Task<CategoryResponseMessage> GetById(CategoryByIdRequestMessage request, ServerCallContext context)
        {            
            return MapToCategory(await service.Get(Guid.Parse(request.Id)).ConfigureAwait(false));
        }

        public override async Task<CategoryResponseMessage> GetBySlug(CategoryBySlugRequestMessage request, ServerCallContext context)
        {
            return MapToCategory(await service.Get(request.Slug).ConfigureAwait(false));
        }

        public override async Task<CategoryResponseMessage> GetFirst(Empty request, ServerCallContext context)
        {
            return MapToCategory(await service.GetFirstByOrder().ConfigureAwait(false));
        }

        private CategoryResponseMessage MapToCategory(CategoryDto category)
        {            
            var Message = new CategoryResponseMessage()
            {
                Id = category.Id.ToString(),
                Description = Tools.NullStringToEmpty(category.Description),
                Name = Tools.NullStringToEmpty(category.Name),
                Order = category.Order,
                Slug = Tools.NullStringToEmpty(category.Slug),
                ShortDescription = Tools.NullStringToEmpty(category.ShortDescription)
            };
            if (category.ImagesUrl?.Count > 0)
                Message.ImagesUrl.AddRange(category.ImagesUrl);
            if (category.Parameters?.Count > 0)
            {
                foreach (var parameter in category.Parameters)
                {
                    Message.Parameters.Add(new CategoryParameterResponseMessage()
                    {
                        Name = Tools.NullStringToEmpty(parameter.Name),
                        Value = Tools.NullStringToEmpty(parameter.Value)
                    });
                }
            }
            return Message;            
        }
        private CategoryShortDataResponseMessage MapToCategoryShortData(CategoryShortDto category)
        {
            return new CategoryShortDataResponseMessage()
            {
                Id = category.Id.ToString(),
                Name = Tools.NullStringToEmpty(category.Name),
                Order = category.Order,
                Slug = Tools.NullStringToEmpty(category.Slug),
                ShortDescription = Tools.NullStringToEmpty(category.ShortDescription)
            };            
        }
    }
}
