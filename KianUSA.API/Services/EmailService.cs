using Grpc.Core;
using Hangfire;
using KianUSA.Application.SeedWork;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace KianUSA.API.Services
{
    [Authorize]
    public class EmailService : EmailSrv.EmailSrvBase
    {
        private readonly Application.Services.Email.EmailService service;
        private readonly IBackgroundJobClient backgroundJobClient;
        public EmailService(IApplicationSettings applicationSettings, IBackgroundJobClient backgroundJobClient)
        {
            service = new Application.Services.Email.EmailService(applicationSettings);
            this.backgroundJobClient = backgroundJobClient;
        }

        public override async Task<SendResponseMessage> SendCatalog(SendCatalogRequestMessage request, ServerCallContext context)
        {
            try
            {
                var Name = context.GetHttpContext().User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.GivenName).Value;
                var LastName = context.GetHttpContext().User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Surname).Value;
                backgroundJobClient.Enqueue(() => service.SendCatalog(Name, LastName, request.CustomerFullName, request.CustomerEmail, request.CategorySlug));
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
    }
}
