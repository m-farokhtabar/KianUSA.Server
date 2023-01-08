using Hangfire;
using KianUSA.Application.Data;
using KianUSA.Application.SeedWork;
using KianUSA.Application.Services.Account;
using KianUSA.Application.Services.Email;
using KianUSA.Application.Services.Helper.Products;
using KianUSA.Application.Services.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KianUSA.Application.Services.Order
{
    public class OrderService
    {
        private readonly IApplicationSettings appSettings;
        private readonly IBackgroundJobClient BackgroundJobClient;
        public OrderService(IApplicationSettings appSettings, IBackgroundJobClient backgroundJobClient)
        {
            this.appSettings = appSettings;
            BackgroundJobClient = backgroundJobClient;
        }
        public async Task SaveOrder(OrderDto Order, string UserEmail, string UserFullName)
        {
            DateTime Current = DateTime.Now;
            if (Order.Orders?.Count > 0)
            {
                var Ac = new AccountService();
                var Customer = await Ac.GetEmail(Order.CustomerUserName);
                if (Customer is not null)
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

                        try
                        {
                            using var Db = new Context();
                            await Db.SaveChangesAsync();
                            
                            var Es = new EmailService(appSettings);
                            BackgroundJobClient.Enqueue(() => Es.SendOrder(Order, SelectedProducts, UserFullName, UserEmail, Customer, Current));
                        }
                        catch
                        {
                            throw new System.Exception("It is not possible to place your orders.");
                        }
                    }
                    else
                        throw new System.Exception("Your selected items are not available.");
                }
                else
                    throw new System.Exception("Customer is not found.");
            }
            else
                throw new System.Exception("There are not any orders. Please select your items");

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
                    if (OrderedComplexItemWithOrderCount.Product.Inventory < 0)
                        throw new System.Exception($"Unfortunately, the product({OrderedComplexItemWithOrderCount.Product.Name}) is out of stock!");
                }
            }
        }        
    }
}
