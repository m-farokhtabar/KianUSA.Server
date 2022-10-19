using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using KianUSA.API.Helper;
using KianUSA.Application.SeedWork;
using KianUSA.Application.Services.Product;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KianUSA.API.Services
{
    [Authorize]
    public class ProductService : ProductSrv.ProductSrvBase
    {
        private readonly Application.Services.Product.ProductService service;
        private readonly ILogger<ProductService> logger;
        public ProductService(IApplicationSettings applicationSettings, ILogger<ProductService> logger)
        {
            service = new(applicationSettings);
            this.logger = logger;
        }
        public override async Task<ProductsResponseMessage> GetAll(Empty request, ServerCallContext context)
        {
            List<ProductDto> products = await service.Get().ConfigureAwait(false);
            ProductsResponseMessage result = new();
            foreach (var product in products)
                result.Products.Add(MapToProduct(product));
            return result;
        }
        public override async Task<ProductsWithTotalItemsResponseMessage> GetByGroupsTagsWithPaging(ProductsByGroupsTagsWithPagingRequestMessage request, ServerCallContext context)
        {
            ProductsWithTotalItemDto products = await service.GetByGroupAndTagsWithPaging(request.Groups?.ToList(), request.Tags?.ToList(), request.PageNumber, request.PageCount).ConfigureAwait(false);
            ProductsWithTotalItemsResponseMessage result = new();
            foreach (var product in products.Products)
                result.Products.Add(MapToProduct(product));
            result.TotalItems = products.TotalItems;
            return result;
        }

        public override async Task<ProductsResponseMessage> GetByFirstCategory(Empty request, ServerCallContext context)
        {
            List<ProductDto> products = await service.GetByFirstCategoryInOrder().ConfigureAwait(false);
            ProductsResponseMessage result = new();
            foreach (var product in products)
                result.Products.Add(MapToProduct(product));
            return result;
        }

        public override async Task<ProductsResponseMessage> GetByCategoryId(ProductsByCategoryIdRequestMessage request, ServerCallContext context)
        {
            List<ProductDto> products = await service.GetByCategoryId(Guid.Parse(request.CategoryId)).ConfigureAwait(false);
            ProductsResponseMessage result = new();
            foreach (var product in products)
                result.Products.Add(MapToProduct(product));
            return result;
        }
        public override async Task<ProductsResponseMessage> GetByCategoryIds(ProductsByCategoryIdsRequestMessage request, ServerCallContext context)
        {
            List<ProductDto> products = await service.GetByCategoryIds(request.CategoryIds.ToList().ConvertAll(x => Guid.Parse(x))).ConfigureAwait(false);
            ProductsResponseMessage result = new();
            foreach (var product in products)
                result.Products.Add(MapToProduct(product));
            return result;
        }

        public override async Task<ProductsResponseMessage> GetByCategorySlug(ProductsByCategorySlugRequestMessage request, ServerCallContext context)
        {
            List<ProductDto> products = await service.GetByCategorySlug(request.CategorySlug).ConfigureAwait(false);
            logger.LogInformation("Count OF DTOProducts:" + products.Count);
            ProductsResponseMessage result = new();
            foreach (var product in products)
                result.Products.Add(MapToProduct(product));
            logger.LogInformation("Count OF Products:" + result.Products.Count);
            return result;
        }
        public override async Task<ProductResponseMessage> GetById(ProductByIdRequestMessage request, ServerCallContext context)
        {
            return MapToProduct(await service.Get(Guid.Parse(request.Id)).ConfigureAwait(false));
        }

        public override async Task<ProductResponseMessage> GetBySlug(ProductBySlugRequestMessage request, ServerCallContext context)
        {
            return MapToProduct(await service.Get(request.Slug).ConfigureAwait(false));
        }

        private ProductResponseMessage MapToProduct(ProductDto product)
        {
            var Message = new ProductResponseMessage()
            {
                Id = product.Id.ToString(),
                Description = Tools.NullStringToEmpty(product.Description),
                Name = Tools.NullStringToEmpty(product.Name),
                Order = product.Order,
                Slug = Tools.NullStringToEmpty(product.Slug),
                ShortDescription = Tools.NullStringToEmpty(product.ShortDescription),
                BoxD = product.BoxD,
                BoxH = product.BoxH,
                BoxW = product.BoxW,
                Cube = product.Cube,
                D = product.D,
                H = product.H,
                Inventory = product.Inventory,
                IsGroup = product.IsGroup,
                W = product.W,
                Weight = product.Weight,
                WHQTY = product.WHQTY
            };
            if (product.CategoryIds?.Count > 0)
                Message.CategoryIds.AddRange(product.CategoryIds.ConvertAll(x => x.ToString()));

            if (product.ImagesUrls?.Count > 0)
                Message.ImagesUrls.AddRange(product.ImagesUrls);
            if (product.Securities?.Count > 0)
                Message.Securities.AddRange(product.Securities);

            if (product.Tags?.Count > 0)
                Message.Tags.AddRange(product.Tags);
            if (product.Groups?.Count > 0)
                Message.Groups.AddRange(product.Groups);
            if (product.Factories?.Count > 0)
                Message.Factories.AddRange(product.Factories);

            if (product.Prices?.Count > 0)
            {
                foreach (var parameter in product.Prices)
                {
                    Message.Prices.Add(new ProductPriceResponseMessage()
                    {
                        Name = Tools.NullStringToEmpty(parameter.Name),
                        Value = parameter.Value.HasValue ? Convert.ToDouble(parameter.Value) : null
                    });
                }
            }
            return Message;
        }

    }
}
