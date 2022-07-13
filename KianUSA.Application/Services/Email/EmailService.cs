using System;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace KianUSA.Application.Services.Email
{
    public class EmailService
    {
        /// <summary>
        /// price-list-email-setting
        /// </summary>
        /// <param name="SettingKey"></param>
        /// <param name="subject"></param>
        /// <param name="To"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        public async Task SendMailAsync(EmailSetting Setting, string subject, string To, string body)
        {
            MailMessage mailMessage = new()
            {
                From = new MailAddress(Setting.From),
                Subject = subject,
                IsBodyHtml = true,
                Body = body,
                SubjectEncoding = Encoding.UTF8,
                BodyEncoding = Encoding.UTF8                
                
            };
            mailMessage.To.Add(To);

            using var client = new SmtpClient(Setting.Host);
            client.Credentials = new NetworkCredential(Setting.UserName, Setting.Password);
            client.Port = Setting.Port;
            client.EnableSsl = true;
            client.UseDefaultCredentials = false;
            try
            {
                await client.SendMailAsync(mailMessage);
            }
            catch(Exception Ex)
            {
                throw new Exception("Cannot send the email.");
            }
        }
    }
    public class EmailSetting
    {
        public string From { get; set; }
        public string SubjectTemplate { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string BodyTemplate { get; set; }
    }
}
