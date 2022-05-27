using iText.Html2pdf;
using KianUSA.Application.SeedWork;
using KianUSA.Application.Services.Category;
using KianUSA.Application.Services.Product;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            Task<string> TaskTemplateCatalog = File.ReadAllTextAsync($"{AssetCatalogPath}TemplateCatalog.html");

            await Task.WhenAll(TaskCategories, TaskProducts, TaskStyle).ConfigureAwait(false);

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

            string TemplateCatalog = TaskTemplateCatalog.Result;

            TemplateBody = TemplateBody.Replace("{Banner}", TemplateSingleBanner);
            //TemplateBody = $"<style>{Style}</style>" + TemplateBody;
            await Task.Run(() =>
            {
                Parallel.ForEach(Categories, Category =>
                {
                    string Body = TemplateBody.Replace("{FeaturesTable}", "");
                    using FileStream pdfDest = File.Open($"{CatalogsPath}{Category.Slug}.pdf", FileMode.Create);
                    if (Category.ImagesUrl?.Count > 0)
                    {
                        Category.ImagesUrl.Sort();
                        Body = Body.Replace("{BannerSrc}", appSettings.WwwRootPath + Category.ImagesUrl[0].Replace("/", "\\"));
                    }
                    else
                        Body = Body.Replace("{BannerSrc}", $"{AssetCatalogPath}Images\\kian_usa_comming_soon.jpg");
                    if (Category.Parameters?.Count > 0)
                    {
                        string Rows = "";
                        foreach (var Parameter in Category.Parameters)
                        {
                            if (!string.IsNullOrEmpty(Parameter.Value))
                                Rows += TemplateCatalogDetailsTableRow.Replace("{DetailsTableTitle}", Parameter.Name).Replace("{DetailsTableValue}", Parameter.Value);
                        }
                        Body = Body.Replace("{DetailsTable}", TemplateCatalogDetailsTable.Replace("{CatalogDetailsRowsTable}", Rows));
                    }
                    else
                    {
                        Body = Body.Replace("{DetailsTable}", "");
                    }
                    var CurrentProducts = Products.Where(x => x.CategorySlug.Contains(Category.Slug)).ToList();
                    if (CurrentProducts?.Count > 0)
                    {
                        string Rows = "";
                        foreach (var CurrentProduct in CurrentProducts)
                        {
                            Rows += TemplateCatalogMeasureTableRow.Replace("{Product.Name}", CurrentProduct.Name)
                                                                  .Replace("{Product.ShortDescription}", CurrentProduct.ShortDescription)
                                                                  .Replace("{Product.Price1.Value}", CurrentProduct.Prices[0].Value.ToString())
                                                                  .Replace("{Product.Price2.Value}", CurrentProduct.Prices[1].Value.ToString())
                                                                  .Replace("{Product.Price3.Value}", CurrentProduct.Prices[2].Value.ToString())
                                                                  .Replace("{Product.ItemWidth}", CurrentProduct.W.ToString())
                                                                  .Replace("{Product.ItemDepth}", CurrentProduct.D.ToString())
                                                                  .Replace("{Product.ItemHeight}", CurrentProduct.H.ToString())
                                                                  .Replace("{Product.BoxWidth}", CurrentProduct.BoxW.ToString())
                                                                  .Replace("{Product.BoxDepth}", CurrentProduct.BoxD.ToString())
                                                                  .Replace("{Product.BoxHeight}", CurrentProduct.BoxH.ToString())
                                                                  .Replace("{Product.Cube}", CurrentProduct.Cube.ToString());
                        }
                        Body = Body.Replace("{MeasureTable}", TemplateCatalogMeasureTable.Replace("{CatalogMeasureTableRows}", Rows));
                    }
                    else
                    {
                        Body = Body.Replace("{MeasureTable}", "");
                    }
                    Body = Body.Replace("{PageNumber}", "1");
                    HtmlConverter.ConvertToPdf(TemplateCatalog.Replace("{Style}", Style).Replace("{Body}", TemplateFirstPage + Body), pdfDest);
                });
            });

        }
    }
}
