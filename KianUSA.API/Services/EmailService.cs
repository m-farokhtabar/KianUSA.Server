using Grpc.Core;
using Hangfire;
using KianUSA.Application.SeedWork;
using KianUSA.Application.Services.Email.PriceList;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace KianUSA.API.Services
{
    [Authorize]
    public class EmailService : EmailSrv.EmailSrvBase
    {
        private readonly Application.Services.Email.PriceList.PriceListEmailService service;
        private readonly IBackgroundJobClient backgroundJobClient;
        public EmailService(IApplicationSettings applicationSettings, IBackgroundJobClient backgroundJobClient)
        {            
            service = new PriceListEmailService(applicationSettings);
            this.backgroundJobClient = backgroundJobClient;
        }

        public override async Task<SendCatalogResponseMessage> SendCatalog(SendCatalogRequestMessage request, ServerCallContext context)
        {
            try
            {
                var Name = context.GetHttpContext().User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.GivenName).Value;
                var LastName = context.GetHttpContext().User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Surname).Value;
                backgroundJobClient.Enqueue(() => service.SendToCustomer(Name, LastName, request.CustomerFullName, request.CustomerEmail, request.CategorySlug));
                return await Task.FromResult(new SendCatalogResponseMessage() { PutInEmailQueue = true });
            }
            catch
            {
                return await Task.FromResult(new SendCatalogResponseMessage() { PutInEmailQueue = false });
            }
        }
    }
}
