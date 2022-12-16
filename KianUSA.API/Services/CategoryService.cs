﻿using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using KianUSA.Application.SeedWork;
using System.Threading.Tasks;
using System.Collections.Generic;
using KianUSA.Application.Services.Category;
using System;
using KianUSA.API.Helper;
using Microsoft.AspNetCore.Authorization;
using System.Linq;

namespace KianUSA.API.Services
{
    [Authorize]
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
        public override async Task<CategoriesResponseMessage> GetAllWithChildren(Empty request, ServerCallContext context)
        {
            List<CategoryDto> categories = await service.GetWithChildren().ConfigureAwait(false);
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
        public override async Task<CategoriesResponseMessage> GetBySlugWithChildren(CategoryBySlugRequestMessage request, ServerCallContext context)
        {           
            List<CategoryDto> categories = await service.GetBySlugWithChildren(request.Slug).ConfigureAwait(false);
            CategoriesResponseMessage result = new();
            foreach (var category in categories)
                result.Categories.Add(MapToCategory(category));
            return result;
        }

        public override async Task<CategoryResponseMessage> GetFirst(Empty request, ServerCallContext context)
        {
            return MapToCategory(await service.GetFirstByOrder().ConfigureAwait(false));
        }        
        public override async Task<CategoriesResponseMessage> GetByFilter(CategoryByTagsRequestMessage request, ServerCallContext context)
        {
            List<CategoryDto> categories = await service.GetByTags(request.Tags.ToList()).ConfigureAwait(false);
            CategoriesResponseMessage result = new();
            foreach (var category in categories)
                result.Categories.Add(MapToCategory(category));
            return result;
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
            if (category.Tags?.Count > 0)
                Message.Tags.AddRange(category.Tags);
            if (category.Securities?.Count > 0)
                Message.Securities.AddRange(category.Securities);
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
            if (category.Features?.Count > 0)
            {
                foreach (var feature in category.Features)
                {
                    Message.Features.Add(new CategoryParameterResponseMessage()
                    {
                        Name = Tools.NullStringToEmpty(feature.Name),
                        Value = Tools.NullStringToEmpty(feature.Value)
                    });
                }
            }
            if (category.Children?.Count > 0)
            {
                foreach (var child in category.Children)
                {
                    Message.Children.Add(new ChildrenCategoryResponseMessage()
                    {
                        Id = child.Id.ToString(),
                        Slug = child.Slug,
                        Order = child.Order
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
