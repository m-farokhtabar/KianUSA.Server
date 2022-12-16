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
        public async Task SaveOrder(OrderDto Order)
        {
            //Merge sum of repeated products which are ordered
            var Orders = Order.Orders.GroupBy(x => x.ProductId).Select(x => new ProductOrder()
            {
                ProductId = x.Key,
                Count = x.Sum(y => y.Count)
            }).ToList();

            var PrSrv = new ProductService(appSettings);
            var Products = await PrSrv.GetModels();

            //لیست محصولاتی که مشتری سفارش داده
            var OrderedProducts = Products.Where(x => Orders.Any(y => y.ProductId == x.Id)).ToList();
            //لیست محصولات ترکیبی
            var OrderedComplexProducts = OrderedProducts.Where(x => !string.IsNullOrWhiteSpace(x.ComplexItemPieces)).ToList();
            // کم کردن سفارشات مربوط به محصولات ترکیبی
            if (OrderedComplexProducts?.Count > 0)
            {
                foreach (var OrderedComplexProduct in OrderedComplexProducts)
                {
                    //بدست آوردن میزان سفارش
                    var ComplexItemOrders = Orders.Find(x => x.ProductId == OrderedComplexProduct.Id);
                    if (ComplexItemOrders.Count > 0)
                    {
                        var PiecesNames = System.Text.Json.JsonSerializer.Deserialize<List<string>>(OrderedComplexProduct.ComplexItemPieces);
                        foreach (var PieceName in PiecesNames)
                        {
                            var Prd = Products.Find(x => x.Name == PieceName);
                            Prd.Inventory -= ComplexItemOrders.Count;
                            if (Prd.Inventory < 0)
                            {
                                throw new System.Exception($"Unfortunately, the product({OrderedComplexProduct.Name}) is out of stock!");
                            }
                        }
                    }
                }
            }
            if (Orders.Count == OrderedProducts.Count)
            {
                var OrdersWithMoreThan20 = Orders.Where(x => x.Count > 20).ToList();
                var OrdersWithMoreThan20ForComplexItem = OrdersWithMoreThan20.Where(x => OrderedProducts.Any(y => y.Id == x.ProductId && !string.IsNullOrWhiteSpace(y.ComplexItemPieces))).Select(x => (x.ProductId, x.Count)).ToList();
                ComputeOrders.GetInventoryForComplexItems(Products, OrdersWithMoreThan20ForComplexItem);

                foreach (var ProductOrder in Orders)
                {
                    var SelectedPrd = Products.Find(x => x.Id == ProductOrder.ProductId);
                    if (SelectedPrd is not null)
                    {
                        //Regular Product
                        if (string.IsNullOrWhiteSpace(SelectedPrd.ComplexItemPieces))
                        {
                            var RemainingStock = SelectedPrd.Inventory - ProductOrder.Count;
                            if (RemainingStock < 0)
                                throw new System.Exception($"Unfortunately, the product({SelectedPrd.Name}) is out of stock!");
                        }
                        else
                        {
                            var PiecesNames = System.Text.Json.JsonSerializer.Deserialize<List<string>>(SelectedPrd.ComplexItemPieces);
                        }
                    }
                    else
                        throw new System.Exception("Some products do not exist!");
                }
            }
            else
            {
                throw new System.Exception("Some products does not exist!");
            }
        }
    }
}
