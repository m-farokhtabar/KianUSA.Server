using KianUSA.Application.Data;
using KianUSA.Application.SeedWork;
using KianUSA.Application.Services.Helper.Products;
using KianUSA.Application.Services.Product;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KianUSA.Application.Services.Order
{
    public class OrderService
    {
        private readonly IApplicationSettings appSettings;
        public OrderService(IApplicationSettings appSettings)
        {
            this.appSettings = appSettings;
        }
        private void OrdersComplexItemWithNoPriority(List<Entity.Product> Products, List<Entity.Product> OrderedProducts, List<ProductOrder> OrdersCount)
        {
            var OrderedComplexItemsWithNoPriority = OrderedProducts.Where(x => !string.IsNullOrWhiteSpace(x.ComplexItemPieces) && x.ComplexItemPriority == 0).ToList();
            if (OrderedComplexItemsWithNoPriority?.Count > 0)
            {
                foreach (var OrderedComplexItemWithNoPriority in OrderedComplexItemsWithNoPriority)
                {
                    double OrderCount = OrdersCount.Find(x => x.ProductId == OrderedComplexItemWithNoPriority.Id).Count;
                    var PiecesNames = System.Text.Json.JsonSerializer.Deserialize<List<string>>(OrderedComplexItemWithNoPriority.ComplexItemPieces);
                    foreach (var PieceName in PiecesNames)
                    {
                        var Prd = Products.Find(x => x.Name == PieceName);
                        Prd.Inventory -= OrderCount;
                        if (Prd.Inventory < 0)
                            throw new System.Exception($"Unfortunately, the product({OrderedComplexItemWithNoPriority.Name}) is out of stock!");
                    }
                }
            }
        }
        private void OrdersSimpleItem(List<Entity.Product> OrderedProducts, List<ProductOrder> OrdersCount)
        {
            var OrderedSimpleItems = OrderedProducts.Where(x => string.IsNullOrWhiteSpace(x.ComplexItemPieces)).ToList();
            if (OrderedSimpleItems?.Count > 0)
            {
                foreach (var OrderedComplexItem in OrderedSimpleItems)
                {
                    double OrderCount = OrdersCount.Find(x => x.ProductId == OrderedComplexItem.Id).Count;
                    OrderedComplexItem.Inventory -= OrderCount;
                    if (OrderedComplexItem.Inventory < 0)
                        throw new System.Exception($"Unfortunately, the product({OrderedComplexItem.Name}) is out of stock!");
                }
            }
        }
        private void OrdersComplexItem(List<Entity.Product> Products, List<Entity.Product> OrderedProducts, List<ProductOrder> OrdersCount)
        {
            var OrderedComplexItems = OrderedProducts.Where(x => !string.IsNullOrWhiteSpace(x.ComplexItemPieces) && x.ComplexItemPriority > 0).ToList();            
            if (OrderedComplexItems?.Count > 0)
            {
                var OrderedComplexItemsWithOrderCount = OrderedComplexItems.Select(x => new { Product = x, Count = OrdersCount.Find(y => y.ProductId == x.Id).Count }).OrderBy(x => (x.Product.Inventory - x.Count)).ToList();

                foreach (var OrderedComplexItemWithOrderCount in OrderedComplexItemsWithOrderCount)
                {
                    double OrderCount = OrderedComplexItemWithOrderCount.Count;
                    //var PiecesNames = System.Text.Json.JsonSerializer.Deserialize<List<string>>(OrderedComplexItemWithOrderCount.Product.ComplexItemPieces);
                    var Peices = InventoryManager.FindPieces(Products, OrderedComplexItemWithOrderCount.Product);
                    var Max = InventoryManager.ComputeMaxInventoryForComplexItem(Peices, OrderCount);
                    if (Max > 0)
                    {
                        foreach (var Peice in Peices)
                            Peice.Item.Inventory -= Max;
                    }
                    var RemindCount = OrderCount - Max;
                    if (RemindCount >= 1)
                    {
                        OrderedComplexItemWithOrderCount.Product.Inventory -= RemindCount;
                    }
                    if (OrderedComplexItemWithOrderCount.Product.Inventory <0)
                        throw new System.Exception($"Unfortunately, the product({OrderedComplexItemWithOrderCount.Product.Name}) is out of stock!");
                }
            }
        }
        public async Task SaveOrder(OrderDto Order)
        {
            if (Order.Orders?.Count > 0)
            {
                //Merge sum of repeated products which are ordered
                var Orders = Order.Orders.GroupBy(x => x.ProductId).Select(x => new ProductOrder()
                {
                    ProductId = x.Key,
                    Count = x.Sum(y => y.Count)
                }).ToList();

                var PrSrv = new ProductService(appSettings);
                var Products = await PrSrv.GetModels();

                var SelectedProducts = Products.Where(x => Orders.Any(y => y.ProductId == x.Id)).ToList();
                if (SelectedProducts?.Count > 0)
                {
                    //اول محصولات ترکیبی که اولویت صفر دارند
                    OrdersComplexItemWithNoPriority(Products, SelectedProducts, Orders);
                    //سفارش محصولات ساده
                    OrdersSimpleItem(SelectedProducts, Orders);
                    //سفارش محصولات ترکیبی
                    OrdersComplexItem(Products, SelectedProducts, Orders);
                    //به روز رسانی وضعیت محصولات
                    InventoryManager.SetInventory(Products);

                    using var Db = new Context();                    
                    await Db.SaveChangesAsync();                   
                }
                else
                    throw new System.Exception("Your selected items are not available.");
            }
            else
                throw new System.Exception("There are not any orders. Please select your items");

            ////لیست محصولاتی که مشتری سفارش داده
            //var OrderedProducts = Products.Where(x => Orders.Any(y => y.ProductId == x.Id)).ToList();
            ////لیست محصولات ترکیبی
            //var OrderedComplexProducts = OrderedProducts.Where(x => !string.IsNullOrWhiteSpace(x.ComplexItemPieces)).ToList();
            //// کم کردن سفارشات مربوط به محصولات ترکیبی
            //if (OrderedComplexProducts?.Count > 0)
            //{
            //    foreach (var OrderedComplexProduct in OrderedComplexProducts)
            //    {
            //        //بدست آوردن میزان سفارش
            //        var ComplexItemOrders = Orders.Find(x => x.ProductId == OrderedComplexProduct.Id);
            //        if (ComplexItemOrders.Count > 0)
            //        {
            //            var PiecesNames = System.Text.Json.JsonSerializer.Deserialize<List<string>>(OrderedComplexProduct.ComplexItemPieces);
            //            foreach (var PieceName in PiecesNames)
            //            {
            //                var Prd = Products.Find(x => x.Name == PieceName);
            //                Prd.Inventory -= ComplexItemOrders.Count;
            //                if (Prd.Inventory < 0)
            //                {
            //                    throw new System.Exception($"Unfortunately, the product({OrderedComplexProduct.Name}) is out of stock!");
            //                }
            //            }
            //        }
            //    }
            //}
            //if (Orders.Count == OrderedProducts.Count)
            //{
            //    var OrdersWithMoreThan20 = Orders.Where(x => x.Count > 20).ToList();
            //    var OrdersWithMoreThan20ForComplexItem = OrdersWithMoreThan20.Where(x => OrderedProducts.Any(y => y.Id == x.ProductId && !string.IsNullOrWhiteSpace(y.ComplexItemPieces))).Select(x => (x.ProductId, x.Count)).ToList();
            //    InventoryManager.GetInventoryForComplexItems(Products, OrdersWithMoreThan20ForComplexItem);

            //    foreach (var ProductOrder in Orders)
            //    {
            //        var SelectedPrd = Products.Find(x => x.Id == ProductOrder.ProductId);
            //        if (SelectedPrd is not null)
            //        {
            //            //Regular Product
            //            if (string.IsNullOrWhiteSpace(SelectedPrd.ComplexItemPieces))
            //            {
            //                var RemainingStock = SelectedPrd.Inventory - ProductOrder.Count;
            //                if (RemainingStock < 0)
            //                    throw new System.Exception($"Unfortunately, the product({SelectedPrd.Name}) is out of stock!");
            //            }
            //            else
            //            {
            //                var PiecesNames = System.Text.Json.JsonSerializer.Deserialize<List<string>>(SelectedPrd.ComplexItemPieces);
            //            }
            //        }
            //        else
            //            throw new System.Exception("Some products do not exist!");
            //    }
            //}
            //else
            //{
            //    throw new System.Exception("Some products does not exist!");
            //}
        }
    }
}
