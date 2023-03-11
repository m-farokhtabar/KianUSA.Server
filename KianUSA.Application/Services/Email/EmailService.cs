using KianUSA.Application.Data;
using KianUSA.Application.SeedWork;
using KianUSA.Application.Services.Account;
using KianUSA.Application.Services.Helper;
using KianUSA.Application.Services.Order;
using KianUSA.Application.Services.Product;
using System;
using System.Collections.Generic;
using System.IO;
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="CustomerFullName"></param>
        /// <param name="CustomerEmail"></param>
        /// <param name="CategorySlug"></param>
        /// <returns></returns>
        public async Task SendCatalog(string UserFirstName, string UserLastName, string CustomerFullName, string CustomerEmail, string CategorySlug, string WhichPrice, string LandedPriceFactor = null)
        {
            if (string.IsNullOrWhiteSpace(CustomerFullName))
                throw new Exception("Please input customer name.");
            if (string.IsNullOrWhiteSpace(CustomerEmail))
                throw new Exception("Please input customer email.");
            if (!Tools.EmailIsValid(CustomerEmail))
                throw new Exception("Customer email is not valid.");
            if (CategorySlug.Contains("/") || CategorySlug.Contains(@"\"))
                throw new Exception("Category name is not valid.");

            string CatalogUrl = $"{CategorySlug}";
            if (!string.IsNullOrWhiteSpace(LandedPriceFactor))
            {
                if (!string.IsNullOrWhiteSpace(WhichPrice))
                    CatalogUrl = $"LandedPrices/0/{CategorySlug}_0_LandedPrice_{LandedPriceFactor}";
                else
                    CatalogUrl = $"LandedPrices/{WhichPrice}/{CategorySlug}_{WhichPrice}_LandedPrice_{LandedPriceFactor}";
            }
            else if (!string.IsNullOrWhiteSpace(WhichPrice))
            {
                CatalogUrl = $"{WhichPrice}/{CategorySlug}_{WhichPrice}";
            }
            using var Db = new Context();
            var Result = await Db.Settings.FindAsync(settings.CatalogEmailSetting).ConfigureAwait(false);
            if (Result is not null)
            {
                try
                {
                    EmailSetting Setting = System.Text.Json.JsonSerializer.Deserialize<EmailSetting>(Result.Value);
                    string Body = Setting.BodyTemplate.Replace("{CustomerName}", CustomerFullName).Replace("{CatalogSlug}", $"{CatalogUrl}.pdf?id={new Random(Guid.NewGuid().GetHashCode()).Next(1, 999999999)}")
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

        public async Task SendOrder(OrderDto Order, List<Entity.Product> Products, string UserFullName, string RepEmail, CustomerDto Customer, DateTime OrderDate, string ConfirmedBy, string InvoiceNumber, string PoNumber)
        {
            string AssetCatalogPath = settings.WwwRootPath + @"\Assets\Email\";
            string OrderEmailTemplate = await File.ReadAllTextAsync($"{AssetCatalogPath}OrderEmailTemplate.html");
            string OrderEmailTableBodyTemplate = await File.ReadAllTextAsync($"{AssetCatalogPath}OrderEmailTableBodyTemplate.html");
            string PoNumberContent = "";
            if (!string.IsNullOrWhiteSpace(PoNumber))
                PoNumberContent = $"<br/><span style=\"color: black;\">P.O. Number: {PoNumber}</span>";
            OrderEmailTemplate = OrderEmailTemplate.Replace("{InvoiceNumber}", InvoiceNumber)
                                                   .Replace("{PoNumber}", PoNumberContent)
                                                   .Replace("{CustomerName}", Customer.FullName);

            string Rows = "";
            double TotalPieces = 0;
            decimal TotalPrices = 0;
            double TotalCount = 0;
            double TotalCubes = 0;
            double TotalWeight = 0;
            double TotalContainers = 0;
            double TotalContainersRound = 0;
            int i = 1;
            foreach (var orderItem in Order.Orders)
            {
                var Prd = Products.Find(x => x.Slug == orderItem.ProductSlug);
                decimal Price = GetPrice(Prd, Order.PriceType, Order.Tariff, Order.Cost);
                decimal TotalPrice = Price * (decimal)orderItem.Count;
                Rows += OrderEmailTableBodyTemplate.Replace("{Item Number}", Prd.Name)
                                                   .Replace("{Qty}", orderItem.Count.ToString())
                                                   .Replace("{Price}", Tools.GetPriceFormat(Price))
                                                   .Replace("{Total Price}", Tools.GetPriceFormat(TotalPrice))
                                                   .Replace("{RowStyle}", GetRowStyle(i, Order.Orders.Count));
                TotalPrices += TotalPrice;
                TotalPieces += (Prd.PiecesCount * orderItem.Count);
                TotalCubes += Prd.Cube.HasValue ? Prd.Cube.Value * orderItem.Count : 0;
                TotalWeight += Prd.Weight.HasValue ? Prd.Weight.Value * orderItem.Count : 0;
                TotalCount += orderItem.Count;
                i++;
            }
            TotalCubes = Math.Round(TotalCubes, 2);
            TotalContainers = Math.Round(TotalCubes / 2400, 2);
            TotalContainersRound = Math.Ceiling(TotalCubes / 2400);
            Rows += OrderEmailTableBodyTemplate
                                   .Replace("{Item Number}", "")
                                   .Replace("{Qty}", TotalCount.ToString())
                                   .Replace("{Price}", "")
                                   .Replace("{Total Price}", Tools.GetPriceFormat(TotalPrices))
                                   .Replace("{RowStyle}", GetRowStyle(i, Order.Orders.Count));

            OrderEmailTemplate = OrderEmailTemplate.Replace("{OrderTableBody}", Rows)
                                                    .Replace("{TotalPricesBelow}", Tools.GetPriceFormat(TotalPrices))
                                                    .Replace("{TotalPieces}", TotalPieces.ToString())
                                                    .Replace("{TotalCubes}", TotalCubes.ToString())
                                                    .Replace("{TotalWeight}", TotalWeight.ToString())                                                    
                                                    .Replace("{Description}", !string.IsNullOrWhiteSpace(Order.Description) ? "Note: " + Order.Description : "");

            OrderEmailTemplate = OrderEmailTemplate.Replace("{CurrentDate}", DateTime.Now.ToLongDateString() + " " + DateTime.Now.ToLongTimeString());
            using var Db = new Context();
            var Result = await Db.Settings.FindAsync(settings.OrderEmailSetting).ConfigureAwait(false);
            if (Result is not null)
            {
                try
                {
                    EmailSetting Setting = System.Text.Json.JsonSerializer.Deserialize<EmailSetting>(Result.Value);
                    string Body = OrderEmailTemplate;
                    string CC = !string.IsNullOrWhiteSpace(Setting.Cc) ? ("," + Setting.Cc) : "";
                    if (RepEmail.Trim() != Customer.Email.Trim())
                        await Provider.SendMailAsync(Setting, Setting.SubjectTemplate, Setting.UserName, Body, RepEmail + "," + Customer.Email + CC, Setting.Bcc);
                    else
                        await Provider.SendMailAsync(Setting, Setting.SubjectTemplate, Setting.UserName, Body, RepEmail + CC, Setting.Bcc);
                }
                catch
                {
                    throw new Exception("Email setting is not valid.");
                }
            }
            else
                throw new Exception("Email setting is not valid.");
        }
        private decimal GetPrice(Entity.Product Model, PriceType Type, TariffType Tariff, double Cost)
        {
            decimal Result = 0;
            List<ProductPriceDto> Prices = !string.IsNullOrWhiteSpace(Model.Price) ? System.Text.Json.JsonSerializer.Deserialize<List<ProductPriceDto>>(Model.Price) : null;
            switch (Type)
            {
                case PriceType.Fob:
                    if (Tariff == TariffType.IORCustomer && Type == PriceType.Fob)
                        Result = Prices[0].Value.HasValue ? Prices[0].Value.Value - ((Prices[0].Value.Value * 20) / 100) : 0;
                    else
                        Result = Prices[0].Value.HasValue ? Prices[0].Value.Value : 0;
                    break;
                case PriceType.Sac:
                    Result = Prices[2].Value.HasValue && Prices[2].Value.Value > 0 ? Prices[2].Value.Value : (Prices[1].Value ?? 0);
                    break;
                case PriceType.LandedPrice:
                    if (Cost > 0 && Model.Cube.HasValue && Model.Cube > 0)
                        return Prices[0].Value.Value + (decimal)(Model.Cube.Value * (Cost / 2350));
                    break;
            }
            return Result;
        }
        private string GetRowStyle(int i, int OrdersCount)
        {
            var RowStyle = "style='border-bottom: solid black 1px;text-align: center;'";
            if (i == 1)
            {
                RowStyle = "style='padding-top:10px;border-bottom: solid black 1px;text-align: center;'";
            }
            else
            {
                if (i == OrdersCount)
                {
                    RowStyle = "style='text-align: center;padding-bottom:10px;'";
                }
            }
            if (i == 1 && i == OrdersCount)
            {
                RowStyle = "style='padding-top:10px;padding-bottom:10px;text-align: center;'";
            }
            return RowStyle;
        }
    }
}
