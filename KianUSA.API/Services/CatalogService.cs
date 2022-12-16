using Grpc.Core;
using KianUSA.Application.SeedWork;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;

namespace KianUSA.API.Services
{
    [Authorize]
    public class CatalogService : CatalogSrv.CatalogSrvBase
    {
        private readonly KianUSA.Application.Services.Catalog.CatalogService catalogService;
        private readonly IApplicationSettings applicationSettings;
        public CatalogService(IApplicationSettings applicationSettings)
        {
            catalogService = new(applicationSettings);
            this.applicationSettings = applicationSettings;
        }
        public override async Task<CatalogResponseMessage> GetLandedPriceCatalogUrl(LandedPriceCatalogRequestMessage request, ServerCallContext context)
        {
            if (request.Factor > 0)
            {
                string CurrentCatalogPath = applicationSettings.WwwRootPath + $@"\Catalogs\LandedPrices\0\{request.CatalogSlug}_0_LandedPrice_{request.Factor}.pdf";
                string AllCatalogPath = applicationSettings.WwwRootPath + $@"\Catalogs\LandedPrices\0\Catalog_0_LandedPrice_{request.Factor}.pdf";
                if (request.CatalogSlug != "All_Cat")
                {
                    if (!System.IO.File.Exists(CurrentCatalogPath) || !System.IO.File.Exists(AllCatalogPath))
                    {
                        await catalogService.CreateWithLandedPrice(request.Factor, request.CatalogSlug);
                    }
                }
                else
                {
                    if (!System.IO.File.Exists(AllCatalogPath))
                    {
                        await catalogService.CreateWithLandedPrice(request.Factor, request.CatalogSlug);
                    }
                }

                string LandedPrice = "_LandedPrice_" + request.Factor.ToString();
                string PriceType = "_0";
                var Result = new CatalogResponseMessage
                {
                    UrlCurrent = $"LandedPrices/0/{request.CatalogSlug}{PriceType}{LandedPrice}.pdf",
                    UrlAll = $"LandedPrices/0/Catalog{PriceType}{LandedPrice}.pdf"
                };
                return Result;
            }
            else
                throw new System.Exception("Factor Parameter is not valid. Needs to be more than zero.");
        }
    }
}
