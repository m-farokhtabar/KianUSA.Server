using Grpc.Core;
using Hangfire;
using KianUSA.Application.SeedWork;
using KianUSA.Application.Services.Email.PriceList;
using System.Threading.Tasks;

namespace KianUSA.API.Services
{
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
                backgroundJobClient.Enqueue(() => service.SendToCustomer("Mehdi", "Far", request.CustomerFullName, request.CustomerEmail, request.CategorySlug));
                return await Task.FromResult(new SendCatalogResponseMessage() { PutInEmailQueue = true });
            }
            catch
            {
                return await Task.FromResult(new SendCatalogResponseMessage() { PutInEmailQueue = false });
            }
        }
    }
}
