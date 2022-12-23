using System;
using System.Collections.Generic;
using System.Linq;

namespace KianUSA.Application.Services.Helper.Products
{
    using KianUSA.Application.Entity;
    public class InventoryManager
    {

        public static void SetInventory(List<Product> Products)
        {
            foreach (var Product in Products)
                Product.OriginalInventory = Product.Inventory;

            //لیست تمامی محصولات ترکیبی به ترتیب بالاترین اولویت
            var ComplexProducts = Products.Where(x => !string.IsNullOrWhiteSpace(x.ComplexItemPieces) && x.ComplexItemPriority > 0).OrderByDescending(x => x.ComplexItemPriority).ToList();
            foreach (var ComplexProduct in ComplexProducts)
            {
                var Pieces = FindPieces(Products, ComplexProduct);
                var ReservedStuckForComplex = ComputeMaxInventoryForComplexItem(Pieces);
                if (ReservedStuckForComplex > 0)
                {
                    foreach (var Piece in Pieces)
                    {
                        Piece.Item.Inventory -= (Piece.Count * ReservedStuckForComplex);
                    }
                }
                ComplexProduct.Inventory = ReservedStuckForComplex;
            }
            var ComplexProductsWithoutPriority = Products.Where(x => !string.IsNullOrWhiteSpace(x.ComplexItemPieces) && x.ComplexItemPriority == 0).OrderByDescending(x => x.ComplexItemPriority).ToList();
            foreach (var ComplexProductWithoutPriority in ComplexProductsWithoutPriority)
            {
                ComplexProductWithoutPriority.Inventory = 0;
            }
            SetWHQTY(Products);
        }
        private static void SetWHQTY(List<Product> Products)
        {
            foreach (var Product in Products)
            {
                Product.WHQTY = GetProductWHQTY(Product);
            }
        }
        public static List<PieceInfo> FindPieces(List<Product> Products, Product Product)
        {
            List<PieceInfo> Pieces = null;
            var PiecesNames = System.Text.Json.JsonSerializer.Deserialize<List<string>>(Product.ComplexItemPieces);
            //this is complex
            if (PiecesNames?.Count > 0)
            {
                //Find all Pieces
                Pieces = new();
                foreach (var PieceName in PiecesNames)
                {
                    var Piece = Products.Find(x => string.Equals(PieceName, x.Name, StringComparison.OrdinalIgnoreCase));
                    if (Piece is not null)
                    {
                        var PieceIsExist = Pieces.Find(x => string.Equals(Piece.Name, x.Item.Name, StringComparison.OrdinalIgnoreCase));
                        if (PieceIsExist is null)
                            Pieces.Add(new PieceInfo(Piece, 1));
                        else
                        {
                            PieceIsExist.Count++;
                        }
                    }
                }
            }
            return Pieces;
        }
        public static double ComputeMaxInventoryForComplexItem(List<PieceInfo> Pieces, double Max = 20)
        {
            //با توجه به به عناصر تشکیل دهنده یک محصول ترکیبی بایستی تعداد با تعداد کم ترین عنصر منطبق شود
            double Min = 9999999999;
            if (Pieces.Count > 0)
            {
                foreach (var Piece in Pieces)
                {
                    if (!Piece.Item.Inventory.HasValue || Piece.Item.Inventory <= 0)
                    {
                        //یعنی امکان ساخت محصول ترکیبی وجود ندارد
                        Min = 0;
                        break;
                    }
                    else
                    {
                        double RealInventory = Math.Floor(Piece.Item.Inventory.Value / Piece.Count);
                        if (Min > RealInventory)
                            Min = RealInventory;
                    }
                }
            }
            //تعیین موجودیت محصول ترکیبی
            Min = Min == 9999999999 ? 0 : Min;
            //یعنی اگر بیش از  تعداد مورد نیاز امکان نگهداری محصول پیچیده داریم فقط به اندازه تعیین شده رزرو شود
            if (Min > Max)
                Min = Max;
            //حداکثر تعداد محصول ترکیبی با توجه به موجودی فعلی
            return Min;
        }
        private static string GetProductWHQTY(Product Product)
        {
            if (string.IsNullOrWhiteSpace(Product.WHQTY))
            {                
                //محصول معمولی
                if (Product.PiecesCount == 1 && Product.ComplexItemPriority == 0)
                {
                    //یعنی این محصول در هیچ محصول ترکیبی استفاده نشده است
                    if (Product.Inventory == Product.OriginalInventory)
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
                        if (Product.Inventory <= 0)
                            return "Set Only";
                        else
                            return "Available";
                    }
                }
                //محصول ترکیبی
                else
                {
                    if (!Product.Inventory.HasValue || Product.Inventory <= 0)
                        return "Out of Stock";
                    else if (Product.Inventory > 0 && Product.Inventory <= 5)
                        return "Call Office";
                    else return "Available";
                }
            }
            else
                return Product.WHQTY;
        }
    }

    public class PieceInfo
    {
        public PieceInfo(Product item, int count)
        {
            Item = item;
            Count = count;
        }

        public Product Item { get; set; }
        public int Count { get; set; }
    }
}
