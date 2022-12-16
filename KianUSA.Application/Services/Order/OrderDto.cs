using System;
using System.Collections.Generic;

namespace KianUSA.Application.Services.Order
{
    public class OrderDto
    {
        public List<ProductOrder> Orders { get; set; }
        public PriceType PriceType { get; set; }
        public DeliveryType Delivery { get; set; }
        public TariffType Tariff { get; set; }
        public string Description { get; set; }
    }

    public class ProductOrder
    {
        public Guid ProductId { get; set; }
        public double Count { get; set; }
    }

    public enum PriceType
    {
        Fob,
        Sac,
        LandedPrice
    }
    public enum DeliveryType
    {
        CustomerForwarder,
        WillCall,
        KIANUSA
    }
    public enum TariffType
    {
        IORKIAN,
        IORCustomer
    }
}
