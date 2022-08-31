using iText.Html2pdf;
using KianUSA.Application.SeedWork;
using KianUSA.Application.Services.Category;
using KianUSA.Application.Services.Product;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using KianUSA.Application.Services.Helper;
using System;

namespace KianUSA.Application.Services.Catalog
{
    public class CatalogService
    {
        private readonly IApplicationSettings appSettings;
        private readonly CategoryService categoryService;
        private readonly ProductService productService;
        public CatalogService(IApplicationSettings appSettings)
        {
            this.appSettings = appSettings;
            categoryService = new CategoryService(appSettings);
            productService = new ProductService(appSettings);
        }
        public async Task Create()
        {
            string CurrentDateTime = DateTime.Now.ToString("MM/dd/yyyy hh:mm tt");
            string CatalogsPath = appSettings.WwwRootPath + @"\Catalogs\";
            string AssetCatalogPath = appSettings.WwwRootPath + @"\Assets\Catalog\";
            Task<List<CategoryDto>> TaskCategories = categoryService.Get();
            Task<List<ProductWithSlugCatDto>> TaskProducts = productService.GetWithCatSlug();
            Task<string> TaskStyle = File.ReadAllTextAsync($"{AssetCatalogPath}CatalogStyle.css");
            Task<string> TaskTemplateFirstPage = File.ReadAllTextAsync($"{AssetCatalogPath}CatalogFirstPage.html");
            Task<string> TaskTemplateBody = File.ReadAllTextAsync($"{AssetCatalogPath}CatalogBody.html");
            Task<string> TaskTemplateSingleBanner = File.ReadAllTextAsync($"{AssetCatalogPath}CatalogSingleBanner.html");
            Task<string> TaskCatalogDetailsTable = File.ReadAllTextAsync($"{AssetCatalogPath}CatalogDetailsTable.html");
            Task<string> TaskCatalogDetailsTableRow = File.ReadAllTextAsync($"{AssetCatalogPath}CatalogDetailsTableRow.html");
            Task<string> TaskCatalogMeasureTable = File.ReadAllTextAsync($"{AssetCatalogPath}CatalogMeasureTable.html");
            Task<string> TaskCatalogMeasureTableRow = File.ReadAllTextAsync($"{AssetCatalogPath}CatalogMeasureTableRow.html");
            Task<string> TaskCatalogMeasureTablePriceHeader = File.ReadAllTextAsync($"{AssetCatalogPath}CatalogMeasureTablePriceHeader.html");
            Task<string> TaskCatalogMeasureTablePriceCellValue = File.ReadAllTextAsync($"{AssetCatalogPath}CatalogMeasureTablePriceCellValue.html");
            Task<string> TaskCatalogFeaturesTable = File.ReadAllTextAsync($"{AssetCatalogPath}CatalogFeaturesTable.html");
            Task<string> TaskCatalogFeaturesTableRow = File.ReadAllTextAsync($"{AssetCatalogPath}CatalogFeaturesTableRow.html");

            Task<string> TaskTemplateCatalog = File.ReadAllTextAsync($"{AssetCatalogPath}TemplateCatalog.html");

            await Task.WhenAll(TaskCategories, TaskProducts, TaskStyle, TaskTemplateFirstPage, TaskTemplateBody,
                               TaskTemplateSingleBanner, TaskCatalogDetailsTable, TaskCatalogDetailsTableRow,
                               TaskCatalogMeasureTable, TaskCatalogMeasureTableRow, TaskCatalogFeaturesTable,
                               TaskCatalogFeaturesTableRow, TaskCatalogMeasureTablePriceHeader).ConfigureAwait(false);

            List<CategoryDto> Categories = TaskCategories.Result;
            List<ProductWithSlugCatDto> Products = TaskProducts.Result;
            string Style = TaskStyle.Result;
            string TemplateFirstPage = TaskTemplateFirstPage.Result.Replace("{Logo}", $"{AssetCatalogPath}Images\\kian_usa_logo.jpg").Replace("{TitleLogo}", $"{AssetCatalogPath}Images\\kian_usa_title_logo.jpg");
            string TemplateSingleBanner = TaskTemplateSingleBanner.Result;
            string TemplateBody = TaskTemplateBody.Result;
            string TemplateCatalogDetailsTable = TaskCatalogDetailsTable.Result;
            string TemplateCatalogDetailsTableRow = TaskCatalogDetailsTableRow.Result;
            string TemplateCatalogMeasureTable = TaskCatalogMeasureTable.Result;
            string TemplateCatalogMeasureTableRow = TaskCatalogMeasureTableRow.Result;
            string TemplateCatalogFeaturesTable = TaskCatalogFeaturesTable.Result;
            string TemplateCatalogFeaturesTableRow = TaskCatalogFeaturesTableRow.Result;
            string TemplateCatalogMeasureTablePriceCellValue = TaskCatalogMeasureTablePriceCellValue.Result;
            string TemplateCatalogMeasureTablePriceHeader = TaskCatalogMeasureTablePriceHeader.Result;
            string TemplateCatalog = TaskTemplateCatalog.Result;

            await Task.Run(() =>
            {
                DirectoryInfo CatalogRoot = new DirectoryInfo(CatalogsPath);
                foreach (var File in CatalogRoot.GetFiles())
                    File.Delete();

                ConcurrentBag<(int Order, string Body)> Catalogs = new();
                Parallel.ForEach(Categories, Category =>
                {
                    if (Category.PublishedCatalogType != PublishedCatalogTypeDto.None)
                    {
                        string Body = TemplateBody;
                        using FileStream pdfDest = File.Open($"{CatalogsPath}{Category.Slug}.pdf", FileMode.Create);
                        Body = Body.Replace("{Banner}", CreateSingleBanner(Category, TemplateSingleBanner, AssetCatalogPath));
                        Body = Body.Replace("{DetailsTable}", CreateDetailsTable(Category, TemplateCatalogDetailsTable, TemplateCatalogDetailsTableRow));
                        Body = Body.Replace("{FeaturesTable}", CreateFeaturesTable(Category, TemplateCatalogFeaturesTable, TemplateCatalogFeaturesTableRow));
                        Body = Body.Replace("{MeasureTable}", CreateMeasureTable(Category, Products, TemplateCatalogMeasureTable, TemplateCatalogMeasureTableRow, TemplateCatalogMeasureTablePriceHeader, TemplateCatalogMeasureTablePriceCellValue));                        
                        Catalogs.Add((Category.Order, Body));
                        Body = Body.Replace("{PageNumber}", "1");
                        Body = Body.Replace("{DateTime}", CurrentDateTime);
                        if (Category.PublishedCatalogType != PublishedCatalogTypeDto.Main)
                            HtmlConverter.ConvertToPdf(TemplateCatalog.Replace("{Style}", Style).Replace("{Body}", TemplateFirstPage + Body), pdfDest);
                    }
                });

                //Create MainCatalog
                if (Catalogs?.Count > 0)
                {
                    using FileStream AllInOnePdf = File.Open($"{CatalogsPath}Catalog.pdf", FileMode.Create, FileAccess.ReadWrite);
                    StringBuilder All = new();
                    var SortedCatalogs = Catalogs.OrderBy(x => x.Order).ToList();
                    for (int i = 0; i < SortedCatalogs.Count; i++)
                    {
                        All.Append(SortedCatalogs[i].Body.Replace("{PageNumber}", (i + 1).ToString()).Replace("{DateTime}", CurrentDateTime));
                    }
                    HtmlConverter.ConvertToPdf(TemplateCatalog.Replace("{Style}", Style).Replace("{Body}", TemplateFirstPage + All.ToString()), AllInOnePdf);
                }
            });

        }
        private static string CreateMeasureTable(CategoryDto Category, List<ProductWithSlugCatDto> Products, string TemplateCatalogMeasureTable, string TemplateCatalogMeasureTableRow, string TemplateCatalogMeasureTablePriceHeader, string TemplateCatalogMeasureTablePriceCellValue)
        {
            var CurrentProducts = Products.Where(x => x.CategorySlug.Contains(Category.Slug)).OrderBy(x => x.Order).ToList();
            if (CurrentProducts?.Count > 0)
            {
                (List<int> AcceptablePriceIndexes, string AcceptablePriceHeaders) = GetAcceptablePriceIndexesAndHeaders(CurrentProducts, TemplateCatalogMeasureTablePriceHeader);
                string Rows = "";
                foreach (var CurrentProduct in CurrentProducts)
                {
                    string AcceptablePriceCells = "";
                    if (AcceptablePriceIndexes?.Count > 0)
                    {
                        foreach (var Index in AcceptablePriceIndexes)
                            AcceptablePriceCells += TemplateCatalogMeasureTablePriceCellValue.Replace("{ProductPriceValue}", Tools.GetPriceFormat(CurrentProduct.Prices[Index].Value));
                    }

                    Rows += TemplateCatalogMeasureTableRow.Replace("{Product.Name}", CurrentProduct.Name)
                                                          .Replace("{Product.ShortDescription}", CurrentProduct.ShortDescription)
                                                          .Replace("{PriceValues}", AcceptablePriceCells)
                                                          .Replace("{Product.ItemWidth}", CurrentProduct.W.ToString())
                                                          .Replace("{Product.ItemDepth}", CurrentProduct.D.ToString())
                                                          .Replace("{Product.ItemHeight}", CurrentProduct.H.ToString())
                                                          .Replace("{Product.BoxWidth}", CurrentProduct.BoxW.ToString())
                                                          .Replace("{Product.BoxDepth}", CurrentProduct.BoxD.ToString())
                                                          .Replace("{Product.BoxHeight}", CurrentProduct.BoxH.ToString())
                                                          .Replace("{Product.Cube}", CurrentProduct.Cube.ToString());
                }
                return TemplateCatalogMeasureTable.Replace("{PriceHeaders}", AcceptablePriceHeaders).Replace("{CatalogMeasureTableRows}", Rows);
            }
            return "";
        }
        private static (List<int>, string) GetAcceptablePriceIndexesAndHeaders(List<ProductWithSlugCatDto> CurrentProducts, string TemplateHeader)
        {
            List<int> AcceptablePriceIndexes = null;
            string Headers = "";
            var FirstProduct = CurrentProducts[0];
            if (FirstProduct.Prices?.Count > 0)
            {
                AcceptablePriceIndexes = new List<int>();
                for (int i = 0; i < FirstProduct.Prices.Count; i++)
                {
                    foreach (var Product in CurrentProducts)
                    {
                        if (Product.Prices[i].Value != null)
                        {
                            AcceptablePriceIndexes.Add(i);
                            Headers += TemplateHeader.Replace("{PriceHeaderName}", Product.Prices[i].Name);
                            break;
                        }
                    }
                }
            }
            return (AcceptablePriceIndexes, Headers);
        }

        private string CreateSingleBanner(CategoryDto Category, string TemplateSingleBanner, string AssetCatalogPath)
        {
            if (Category.ImagesUrl?.Count > 0)
            {
                //Category.ImagesUrl.Sort();
                foreach (var ImageUrl in Category.ImagesUrl)
                {
                    if (System.Convert.ToInt32(ImageUrl.Substring(ImageUrl.LastIndexOf("_") + 1, 4)) >= appSettings.StartIndexOfImageForUsingInCatalog)
                        return TemplateSingleBanner.Replace("{BannerSrc}", appSettings.WwwRootPath + ImageUrl.Replace("/", "\\"));
                }
            }
            return TemplateSingleBanner.Replace("{BannerSrc}", $"{AssetCatalogPath}Images\\kian_usa_comming_soon.jpg");
        }

        private static string CreateDetailsTable(CategoryDto Category, string TemplateCatalogDetailsTable, string TemplateCatalogDetailsTableRow)
        {
            if (Category.Parameters?.Count > 0 && Category.Parameters.Any(x => !string.IsNullOrWhiteSpace(x.Value)))
            {
                string Rows = "";
                foreach (var Parameter in Category.Parameters)
                {
                    if (!string.IsNullOrEmpty(Parameter.Value))
                        Rows += TemplateCatalogDetailsTableRow.Replace("{DetailsTableTitle}", Parameter.Name).Replace("{DetailsTableValue}", Parameter.Value);
                }
                return TemplateCatalogDetailsTable.Replace("{CatalogDetailsRowsTable}", Rows);
            }
            return "";
        }

        private static string CreateFeaturesTable(CategoryDto Category, string TemplateCatalogFeaturesTable, string TemplateCatalogFeaturesTableRow)
        {
            if (Category.Features?.Count > 0 && Category.Features.Any(x => !string.IsNullOrWhiteSpace(x.Value)))
            {
                string Rows = "";
                foreach (var Feature in Category.Features)
                {
                    if (!string.IsNullOrEmpty(Feature.Value))
                        Rows += TemplateCatalogFeaturesTableRow.Replace("{FeaturesTableRowValue}", Feature.Value);
                }
                return TemplateCatalogFeaturesTable.Replace("{CatalogFeaturesTableRows}", Rows);
            }
            return "";
        }
    }
}
