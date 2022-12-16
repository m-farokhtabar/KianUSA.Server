using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KianUSA.Application.Services.Helper.Products
{
    using KianUSA.Application.Entity;
    public class ComputeOrders
    {
        public static void GetInventoryForComplexItems(List<Product> Products, List<(Guid ProductId, double MaxCount)>  ComplexProductsMax = null)
        {
            Products = Products.OrderByDescending(x => x.ComplexItemPriority).ToList();
            int Priority = Products[0].ComplexItemPriority;
            Dictionary<Guid, List<(double InventoryNeedsPerEach, int CountOfItem, Product ComplexProduct)>> PiecesResult = new();
            foreach (var Product in Products)
            {
                //پس از محاسبه برای تمام اولویت های یکسان حالا باید بزرگترین ها را از محصولات کم کرد تا سقف 20
                if (Product.ComplexItemPriority != Priority)
                {
                    foreach (var PieceResult in PiecesResult)
                    {
                        
                        var CurrentPiece = Products.Find(x => x.Id == PieceResult.Key);
                        (double InventoryPerEach, int Count) Max = (0, 0);
                        foreach (var (InventoryNeedsPerEach, CountOfItem, ComplexProduct) in PieceResult.Value)
                        {
                            var MaxForComplexItem = ComplexProductsMax.Find(x => x.ProductId == ComplexProduct.Id).MaxCount;
                            if (MaxForComplexItem < 20)
                                MaxForComplexItem = 20;
                            if (InventoryNeedsPerEach > MaxForComplexItem)
                            {
                                Max.InventoryPerEach = MaxForComplexItem;
                                break;
                            }
                            if (InventoryNeedsPerEach > Max.InventoryPerEach)
                            {
                                Max.InventoryPerEach = InventoryNeedsPerEach;
                                Max.Count = CountOfItem;
                            }
                        }
                        CurrentPiece.Inventory -= (Max.InventoryPerEach * Max.Count);
                    }
                    PiecesResult.Clear();
                    Priority = Product.ComplexItemPriority;
                }
                if (!string.IsNullOrWhiteSpace(Product.ComplexItemPieces))
                {
                    var PiecesNames = System.Text.Json.JsonSerializer.Deserialize<List<string>>(Product.ComplexItemPieces);
                    //this is complex
                    if (PiecesNames?.Count > 0)
                    {
                        //Find all Pieces
                        List<(Product Piece, int Count)> Pieces = new();
                        foreach (var PieceName in PiecesNames)
                        {
                            var Piece = Products.Find(x => string.Equals(PieceName, x.Name, StringComparison.OrdinalIgnoreCase));
                            if (Piece is not null)
                            {
                                var PieceIsExist = Pieces.Find(x => string.Equals(Piece.Name, x.Piece.Name, StringComparison.OrdinalIgnoreCase));
                                if (PieceIsExist.Piece is null)
                                    Pieces.Add((Piece, 1));
                                else
                                {
                                    PieceIsExist.Count++;
                                }
                            }
                        }
                        //با توجه به به عناصر تشکیل دهنده یک محصول ترکیبی بایستی تعداد با تعداد کم ترین عنصر منطبق شود
                        double Min = 9999999999;
                        if (Pieces.Count > 0)
                        {
                            foreach (var (Piece, Count) in Pieces)
                            {
                                if (!Piece.Inventory.HasValue || Piece.Inventory == 0)
                                    break;
                                else
                                {
                                    double RealInventory = Math.Floor(Piece.Inventory.Value / Count);
                                    if (Min > RealInventory)
                                        Min = RealInventory;
                                }
                            }
                        }
                        //تعیین موجودیت محصول ترکیبی
                        Min = Min == 9999999999 ? 0 : Min;
                        //حداکثر تعداد محصول ترکیبی با توجه به موجودی فعلی
                        Product.Inventory = Min;
                        foreach (var Piece in Pieces)
                        {
                            PiecesResult.TryGetValue(Piece.Piece.Id, out List<(double InventoryNeedsPerEach, int CountOfItem, Product ComplexProduct)> Value);
                            if (Value is not null)
                                Value.Add((Min, Piece.Count, Product));
                            else
                                PiecesResult.Add(Piece.Piece.Id, new List<(double InventoryNeedsPerEach, int CountOfItem, Product ComplexProduct)>() { (Min, Piece.Count, Product) });
                        }
                    }
                }
            }
        }
        public static string GetProductWHQTY(Product Product, List<Product> ProductsForComputingInventory)
        {
            if (string.IsNullOrWhiteSpace(Product.WHQTY))
            {
                var ProductForComputingInventory = ProductsForComputingInventory.Find(x => x.Id == Product.Id);
                //محصول معمولی
                if (Product.PiecesCount == 1)
                {
                    //یعنی این محصول در هیچ محصول ترکیبی استفاده نشده است
                    if (Product.Inventory == ProductForComputingInventory.Inventory)
                    {
                        if (!Product.Inventory.HasValue || Product.Inventory <= 0)
                            return "Out of Stock";
                        else if (Product.Inventory > 0 && Product.Inventory <= 5)
                            return "Call Office";
                        else return "Available";
                    }
                    //یعنی این محصول در حداقل یک محصول ترکیبی استفاده شده است
                    else
                    {
                        if (ProductForComputingInventory.Inventory <= 0)
                            return "Set Only";
                        else
                            return "Available";
                    }
                }
                //محصول ترکیبی
                else
                {
                    if (!ProductForComputingInventory.Inventory.HasValue || ProductForComputingInventory.Inventory <= 0)
                        return "Out of Stock";
                    else if (ProductForComputingInventory.Inventory > 0 && ProductForComputingInventory.Inventory <= 5)
                        return "Call Office";
                    else return "Available";
                }
            }
            else
                return Product.WHQTY;
        }
    }
}
