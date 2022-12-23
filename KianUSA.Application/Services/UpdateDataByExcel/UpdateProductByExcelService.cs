using KianUSA.Application.Data;
using KianUSA.Application.Services.UpdateDataByExcel.Helper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Threading.Tasks;

namespace KianUSA.Application.Services.UpdateDataByExcel
{
    using KianUSA.Application.Entity;
    using KianUSA.Application.SeedWork;
    using KianUSA.Application.Services.Category;
    using KianUSA.Application.Services.Helper.Products;

    public class UpdateProductByExcelService
    {
        private readonly IApplicationSettings appSettings;
        public UpdateProductByExcelService(IApplicationSettings appSettings)
        {
            this.appSettings = appSettings;
        }
        public async Task Update(Stream stream)
        {
            if (stream is null)
                throw new Exception("Please upload correct excel file.");
            List<Product> Products = new();
            //List<Product> ProductsForComputingInventory = new();
            try
            {
                var Tables = UpdateByExcelHelper.ReadExcel(stream);
                if (Tables?.Count > 0 && Tables[0].Rows?.Count > 0)
                {
                    CategoryService Service = new(appSettings);
                    List<CategoryShortDto> Categories = null;
                    try { Categories = (await Service.GetShortData().ConfigureAwait(false)); } catch { }
                    for (int i = 0; i < Tables[0].Rows.Count; i++)
                    {
                        var Row = Tables[0].Rows[i];
                        Guid Id = Guid.NewGuid();
                        Product NewProduct = CreateProduct(Products, Tables, Categories, Row, Id);
                        Products.Add(NewProduct);
                        //ProductsForComputingInventory.Add(CreateProduct(Products, Tables, Categories, Row, Id));
                    }
                    SetPieces(Products);
                    InventoryManager.SetInventory(Products);                    
                }
            }
            catch (Exception ex)
            {
                throw new Exception("there are some errors during reading data from excel.[" + ex.Message + "]");
            }
            await UpdateDatabase(Products);
        }

        private Product CreateProduct(List<Product> Products, DataTableCollection Tables, List<CategoryShortDto> Categories, DataRow Row, Guid Id)
        {
            return new()
            {
                Id = Id,
                Name = Row["Name"].ToString().Trim(),
                Slug = UpdateByExcelHelper.GenerateSlug(Row["Name"].ToString(), Row["Slug"], Products.ConvertAll(x => x.Slug)),
                Description = Row["Description"].ToString().Trim(),
                ShortDescription = Row["Short description"].ToString().Trim(),
                BoxD = UpdateByExcelHelper.GetDouble(Row["Box D"]),
                BoxW = UpdateByExcelHelper.GetDouble(Row["Box W"]),
                BoxH = UpdateByExcelHelper.GetDouble(Row["Box H"]),
                WHQTY = Row["WH QTY"].ToString().Trim(),
                Cube = UpdateByExcelHelper.GetDouble(Row["Cube"]),
                D = UpdateByExcelHelper.GetDouble(Row["D"]),
                H = UpdateByExcelHelper.GetDouble(Row["H"]),
                W = UpdateByExcelHelper.GetDouble(Row["W"]),
                Weight = UpdateByExcelHelper.GetDouble(Row["Weight (lbs)"]),
                Security = UpdateByExcelHelper.ConvertStringWithbracketsToJsonArrayString(Row["Security"].ToString().Trim()),
                Categories = GetCategoriesByName(Id, Row["Categories"]?.ToString(), Categories),
                Price = CreateJsonPrices(Tables[0].Columns, Row),
                Order = UpdateByExcelHelper.GetInt32WithDefaultZero(Row["Position"]),
                Inventory = UpdateByExcelHelper.GetDouble(Row["Inventory"]),
                Groups = UpdateByExcelHelper.ConvertStringWithbracketsToJsonArrayString(Row["Groups"].ToString().Trim()),
                Factories = UpdateByExcelHelper.ConvertStringWithbracketsToJsonArrayString(Row["Factories"].ToString().Trim()),
                Tags = UpdateByExcelHelper.ConvertStringWithbracketsToJsonArrayString(Row["Tags"].ToString().Trim()),
                ComplexItemPieces = UpdateByExcelHelper.ConvertStringWithbracketsToJsonArrayString(Row["Peace of Complex Item"].ToString().Trim()),
                ComplexItemPriority = UpdateByExcelHelper.GetInt32WithDefaultZero(Row["Complex Item Priority"])
            };
        }

        private static void SetPieces(List<Product> Products)
        {
            foreach (var Product in Products)
            {
                if (!string.IsNullOrWhiteSpace(Product.ComplexItemPieces))
                {
                    var PiecesNames = System.Text.Json.JsonSerializer.Deserialize<List<string>>(Product.ComplexItemPieces);
                    Product.PiecesCount = PiecesNames?.Count > 0 ? PiecesNames.Count : 1;
                }
                else
                    Product.PiecesCount = 1;
            }
        }

        //private static void GetInventoryForComplexItems(List<Product> Products)
        //{
        //    Products = Products.OrderByDescending(x => x.ComplexItemPriority).ToList();
        //    int Priority = Products[0].ComplexItemPriority;
        //    Dictionary<Guid, List<(double InventoryNeedsPerEach, int CountOfItem, Product ComplexProduct)>> PiecesResult = new();
        //    foreach (var Product in Products)
        //    {
        //        //پس از محاسبه برای تمام اولویت های یکسان حالا باید بزرگترین ها را از محصولات کم کرد تا سقف 20
        //        if (Product.ComplexItemPriority != Priority)
        //        {
        //            foreach (var PieceResult in PiecesResult)
        //            {
        //                var CurrentPiece = Products.Find(x => x.Id == PieceResult.Key);
        //                (double InventoryPerEach, int Count) Max = (0, 0);
        //                foreach (var (InventoryNeedsPerEach, CountOfItem, ComplexProduct) in PieceResult.Value)
        //                {
        //                    if (InventoryNeedsPerEach > 20)
        //                    {
        //                        Max.InventoryPerEach = 20;
        //                        break;
        //                    }
        //                    if (InventoryNeedsPerEach > Max.InventoryPerEach)
        //                    {
        //                        Max.InventoryPerEach = InventoryNeedsPerEach;
        //                        Max.Count = CountOfItem;
        //                    }
        //                }
        //                CurrentPiece.Inventory -= (Max.InventoryPerEach * Max.Count);
        //            }
        //            PiecesResult.Clear();
        //            Priority = Product.ComplexItemPriority;
        //        }
        //        if (!string.IsNullOrWhiteSpace(Product.ComplexItemPieces))
        //        {
        //            var PiecesNames = System.Text.Json.JsonSerializer.Deserialize<List<string>>(Product.ComplexItemPieces);
        //            //this is complex
        //            if (PiecesNames?.Count > 0)
        //            {
        //                //Find all Pieces
        //                List<(Product Piece, int Count)> Pieces = new();
        //                foreach (var PieceName in PiecesNames)
        //                {
        //                    var Piece = Products.Find(x => string.Equals(PieceName, x.Name, StringComparison.OrdinalIgnoreCase));
        //                    if (Piece is not null)
        //                    {
        //                        var PieceIsExist = Pieces.Find(x => string.Equals(Piece.Name, x.Piece.Name, StringComparison.OrdinalIgnoreCase));
        //                        if (PieceIsExist.Piece is null)
        //                            Pieces.Add((Piece, 1));
        //                        else
        //                        {
        //                            PieceIsExist.Count++;
        //                        }
        //                    }
        //                }
        //                //Find Min count of the Pieces
        //                double Min = 9999999999;
        //                if (Pieces.Count > 0)
        //                {
        //                    foreach (var (Piece, Count) in Pieces)
        //                    {
        //                        if (!Piece.Inventory.HasValue || Piece.Inventory == 0)
        //                            break;
        //                        else
        //                        {
        //                            double RealInventory = Math.Floor(Piece.Inventory.Value / Count);
        //                            if (Min > RealInventory)
        //                                Min = RealInventory;
        //                        }
        //                    }
        //                }
        //                //تعیین موجودیت محصول ترکیبی
        //                Min = Min == 9999999999 ? 0 : Min;
        //                Product.Inventory = Min;
        //                foreach (var Piece in Pieces)
        //                {
        //                    PiecesResult.TryGetValue(Piece.Piece.Id, out List<(double InventoryNeedsPerEach, int CountOfItem, Product ComplexProduct)> Value);
        //                    if (Value is not null)
        //                        Value.Add((Min, Piece.Count, Product));
        //                    else
        //                        PiecesResult.Add(Piece.Piece.Id, new List<(double InventoryNeedsPerEach, int CountOfItem, Product ComplexProduct)>() { (Min, Piece.Count, Product) });
        //                }
        //            }
        //        }
        //    }
        //}
        //private void SetWHQTY(List<Product> Products, List<Product> ProductsForComputingInventory)
        //{
        //    foreach (var Product in Products)
        //    {
        //        Product.WHQTY = ComputeOrders.GetProductWHQTY(Product, ProductsForComputingInventory);
        //    }
        //}
        //private string GetProductWHQTY(Product Product, List<Product> ProductsForComputingInventory)
        //{
        //    if (string.IsNullOrWhiteSpace(Product.WHQTY))
        //    {
        //        var ProductForComputingInventory = ProductsForComputingInventory.Find(x => x.Id == Product.Id);
        //        //محصول معمولی
        //        if (Product.PiecesCount == 1)
        //        {
        //            //یعنی این محصول در هیچ محصول ترکیبی استفاده نشده است
        //            if (Product.Inventory == ProductForComputingInventory.Inventory)
        //            {
        //                if (!Product.Inventory.HasValue || Product.Inventory <= 0)
        //                    return "Out of Stock";
        //                else if (Product.Inventory > 0 && Product.Inventory <= 5)
        //                    return "Call Office";
        //                else return "Available";
        //            }
        //            //یعنی این محصول در حداقل یک محصول ترکیبی استفاده شده است
        //            else
        //            {
        //                if (ProductForComputingInventory.Inventory <= 0)
        //                    return "Set Only";
        //                else
        //                    return "Available";
        //            }
        //        }
        //        //محصول ترکیبی
        //        else
        //        {
        //            if (!ProductForComputingInventory.Inventory.HasValue || ProductForComputingInventory.Inventory <= 0)
        //                return "Out of Stock";
        //            else if (ProductForComputingInventory.Inventory > 0 && ProductForComputingInventory.Inventory <= 5)
        //                return "Call Office";
        //            else return "Available";
        //        }
        //    }
        //    else
        //        return Product.WHQTY;
        //}
        private static async Task UpdateDatabase(List<Product> Products)
        {
            try
            {
                using var Db = new Context();
                using var Trans = await Db.Database.BeginTransactionAsync();
                try
                {
                    Db.Database.ExecuteSqlRaw("DELETE FROM \"Product\"");
                    Db.Products.AddRange(Products);
                    await Db.SaveChangesAsync();
                    Trans.Commit();
                }
                catch (Exception Ex)
                {
                    Trans.Rollback();
                    throw new Exception("Cannot update database");
                }
            }
            catch
            {
                throw new Exception("Cannot connect To the database");
            }
        }

        private string CreateJsonPrices(DataColumnCollection Columns, DataRow Row)
        {
            List<ProductPrice> PricesList = new();
            foreach (var Column in Columns)
            {
                string ColumnName = Column.ToString();
                if (ColumnName.StartsWith("Price", StringComparison.OrdinalIgnoreCase))
                {
                    decimal? Price = UpdateByExcelHelper.GetDecimal(Row[ColumnName]);
                    PricesList.Add(new ProductPrice() { Name = $"{ColumnName.Replace("Price", "", StringComparison.OrdinalIgnoreCase).Trim()}", Value = Price });
                }
            }
            return System.Text.Json.JsonSerializer.Serialize(PricesList);
        }
        private List<CategoryProduct> GetCategoriesByName(Guid ProductId, string CategoryNames, List<CategoryShortDto> Categories)
        {
            if (!string.IsNullOrWhiteSpace(CategoryNames))
            {
                List<CategoryProduct> CategoriesProducts = new();
                string[] CatNames = CategoryNames.ToString().Split(",");
                foreach (var CatName in CatNames)
                {
                    var Cat = Categories.Find(x => x.Name.Equals(CatName, StringComparison.OrdinalIgnoreCase));
                    if (Cat is not null)
                        CategoriesProducts.Add(new CategoryProduct() { CategoryId = Cat.Id, ProductId = ProductId, CategorySlug = Cat.Slug });
                }
                return CategoriesProducts;

            }
            return null;
        }

    }
}
