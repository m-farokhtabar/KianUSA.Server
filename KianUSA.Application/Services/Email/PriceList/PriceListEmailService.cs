using KianUSA.Application.Data;
using KianUSA.Application.SeedWork;
using KianUSA.Application.Services.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KianUSA.Application.Services.Email.PriceList
{
    public class PriceListEmailService
    {
        private readonly IApplicationSettings settings;
        public PriceListEmailService(IApplicationSettings settings)
        {
            this.settings = settings;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="CustomerFullName"></param>
        /// <param name="CustomerEmail"></param>
        /// <param name="CategorySlug"></param>
        /// <returns></returns>
        public async Task SendToCustomer(string UserFirstName, string UserLastName, string CustomerFullName, string CustomerEmail, string CategorySlug)
        {
            if (string.IsNullOrWhiteSpace(CustomerFullName))
                throw new Exception("Please input customer name.");
            if (string.IsNullOrWhiteSpace(CustomerEmail))
                throw new Exception("Please input customer email.");
            if (!Tools.EmailIsValid(CustomerEmail))
                throw new Exception("Customer email is not valid.");
            if (CategorySlug.Contains("/") || CategorySlug.Contains(@"\"))
                    throw new Exception("Category name is not valid.");

            using var Db = new Context();            
            var Result = await Db.Settings.FindAsync(settings.CatalogEmailSetting).ConfigureAwait(false);
            if (Result is not null)
            {
                try
                {
                    EmailSetting Setting = System.Text.Json.JsonSerializer.Deserialize<EmailSetting>(Result.Value);                    
                    string Body = Setting.BodyTemplate.Replace("{CustomerName}", CustomerFullName).Replace("{CatalogSlug}", CategorySlug)
                                                      .Replace("{User_FirstName}", UserFirstName).Replace("{User_LastName}", UserLastName).Replace("{CurrentDate}", DateTime.Now.ToLongDateString() + " " + DateTime.Now.ToLongTimeString());
                    EmailService Service = new();
                    await Service.SendMailAsync(Setting, Setting.SubjectTemplate, CustomerEmail, Body);

                }
                catch
                {
                    throw new Exception("Email setting is not valid.");
                }                
            }
        }
    }
}
