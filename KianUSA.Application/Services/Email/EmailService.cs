using KianUSA.Application.Data;
using KianUSA.Application.SeedWork;
using KianUSA.Application.Services.Helper;
using System;
using System.Threading.Tasks;

namespace KianUSA.Application.Services.Email
{
    public class EmailService
    {
        private readonly IApplicationSettings settings;
        private readonly EmailProvider Provider;
        public EmailService(IApplicationSettings settings)
        {
            this.settings = settings;
            Provider = new();
        }
        public async Task SendCatalogWithLandedPrice(string UserFirstName, string UserLastName, string CustomerFullName, string CustomerEmail, string CategorySlug)
        {

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="CustomerFullName"></param>
        /// <param name="CustomerEmail"></param>
        /// <param name="CategorySlug"></param>
        /// <returns></returns>
        public async Task SendCatalog(string UserFirstName, string UserLastName, string CustomerFullName, string CustomerEmail, string CategorySlug)
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
                    string Body = Setting.BodyTemplate.Replace("{CustomerName}", CustomerFullName).Replace("{CatalogSlug}", $"{CategorySlug}.pdf?id={new Random(Guid.NewGuid().GetHashCode()).Next(1,999999999)}")
                                                      .Replace("{User_FirstName}", UserFirstName).Replace("{User_LastName}", UserLastName).Replace("{CurrentDate}", DateTime.Now.ToLongDateString() + " " + DateTime.Now.ToLongTimeString());
                    
                    await Provider.SendMailAsync(Setting, Setting.SubjectTemplate, CustomerEmail, Body);

                }
                catch
                {
                    throw new Exception("Email setting is not valid.");
                }                
            }
        }
        public async Task SendContactUs(string Name, string Family, string Phone, string Email, string Comment)
        {
            if (string.IsNullOrWhiteSpace(Name))
                throw new Exception("Please input name.");
            if (string.IsNullOrWhiteSpace(Family))
                throw new Exception("Please input family.");

            if (!string.IsNullOrWhiteSpace(Phone))
            {
                if (!Tools.PhoneIsValid(Phone))
                    throw new Exception("phone is not valid.");
            }
            if (string.IsNullOrWhiteSpace(Email))
                throw new Exception("Please input email.");
            if (!Tools.EmailIsValid(Email))
                throw new Exception("email is not valid.");

            if (string.IsNullOrWhiteSpace(Comment))
                throw new Exception("Please input Comment.");

            using var Db = new Context();
            var Result = await Db.Settings.FindAsync(settings.ContactUsEmailSetting).ConfigureAwait(false);
            if (Result is not null)
            {
                try
                {
                    EmailSetting Setting = System.Text.Json.JsonSerializer.Deserialize<EmailSetting>(Result.Value);
                    string Body = Setting.BodyTemplate.Replace("{Name}", Name).Replace("{Family}", Family).Replace("{Phone}", Phone)
                                                      .Replace("{Email}", Email).Replace("{Comment}", Comment)
                                                      .Replace("{CurrentDate}", DateTime.Now.ToLongDateString() + " " + DateTime.Now.ToLongTimeString());
                    await Provider.SendMailAsync(Setting, Setting.SubjectTemplate, Setting.UserName, Body);

                }
                catch
                {
                    throw new Exception("Email setting is not valid.");
                }
            }
            else
                throw new Exception("Email setting is not valid.");
        }

    }
}
