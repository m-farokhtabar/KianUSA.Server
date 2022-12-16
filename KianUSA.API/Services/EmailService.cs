using Grpc.Core;
using Hangfire;
using KianUSA.Application.SeedWork;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Threading.Tasks;

namespace KianUSA.API.Services
{
    [Authorize]
    public class EmailService : EmailSrv.EmailSrvBase
    {
        private readonly Application.Services.Email.EmailService service;
        private readonly IBackgroundJobClient backgroundJobClient;
        private readonly Application.Services.Catalog.CatalogService catalogService;
        private readonly IApplicationSettings applicationSettings;
        public EmailService(IApplicationSettings applicationSettings, IBackgroundJobClient backgroundJobClient)
        {
            service = new Application.Services.Email.EmailService(applicationSettings);
            this.backgroundJobClient = backgroundJobClient;
            catalogService = new Application.Services.Catalog.CatalogService(applicationSettings);
            this.applicationSettings = applicationSettings;
        }

        public override async Task<SendResponseMessage> SendCatalog(SendCatalogRequestMessage request, ServerCallContext context)
        {
            try
            {
                var Name = context.GetHttpContext().User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.GivenName).Value;
                var LastName = context.GetHttpContext().User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Surname).Value;
                backgroundJobClient.Enqueue(() => service.SendCatalog(Name, LastName, request.CustomerFullName, request.CustomerEmail, request.CategorySlug, request.WhichPrice, null));
                return await Task.FromResult(new SendResponseMessage() { PutInEmailQueue = true });
            }
            catch
            {
                return await Task.FromResult(new SendResponseMessage() { PutInEmailQueue = false });
            }
        }
        public override async Task<SendResponseMessage> SendCatalogWithLandedPrice(SendCatalogWithLandedPriceRequestMessage request, ServerCallContext context)
        {
            try
            {
                var Name = context.GetHttpContext().User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.GivenName).Value;
                var LastName = context.GetHttpContext().User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Surname).Value;
                backgroundJobClient.Enqueue(() => CreateAndSend(request, Name, LastName));
                return await Task.FromResult(new SendResponseMessage() { PutInEmailQueue = true });
            }
            catch
            {
                return await Task.FromResult(new SendResponseMessage() { PutInEmailQueue = false });
            }
        }
        [AllowAnonymous]
        public override async Task<SendResponseMessage> SendContactUs(SendContactUsRequestMessage request, ServerCallContext context)
        {
            try
            {
                backgroundJobClient.Enqueue(() => service.SendContactUs(request.Name, request.Family, request.Phone, request.Email, request.Comment));
                return await Task.FromResult(new SendResponseMessage() { PutInEmailQueue = true });
            }
            catch
            {
                return await Task.FromResult(new SendResponseMessage() { PutInEmailQueue = false });
            }
        }
        public async Task CreateAndSend(SendCatalogWithLandedPriceRequestMessage request, string Name, string LastName)
        {
            string CatalogsPath = applicationSettings.WwwRootPath + $@"\Catalogs\LandedPrices\0\{request.CategorySlug}_0_LandedPrice_{request.Factor}.pdf";
            if (!System.IO.File.Exists(CatalogsPath))
                await catalogService.CreateWithLandedPrice(request.Factor, request.CategorySlug);
            await service.SendCatalog(Name, LastName, request.CustomerFullName, request.CustomerEmail, request.CategorySlug, "0", request.Factor.ToString());
        }
    }
}
