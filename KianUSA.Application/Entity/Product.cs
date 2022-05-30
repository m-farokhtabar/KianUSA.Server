﻿using System;
using System.Collections.Generic;

namespace KianUSA.Application.Entity
{
    public class Product
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
        public double? Inventory { get; set; }
        public string ShortDescription { get; set; }
        public string Description { get; set; }
        /// <summary>
        /// Json format
        /// </summary>
        public string Price { get; set; }        
        public double? Cube { get; set; }
        public double? W { get; set; }
        public double? D { get; set; }
        public double? H { get; set; }
        public double? Weight { get; set; }
        public double? BoxW { get; set; }
        public double? BoxD { get; set; }
        public double? BoxH { get; set; } 
        /// <summary>
        /// A,B,C
        /// </summary>
        public string Security { get; set; }
        public string WHQTY { get; set; }
        public int Order { get; set; }
        /// <summary>
        /// False => means it is a regular product
        /// true => means it is a group of products that have a behaviour like a product
        /// </summary>
        public bool IsGroup { get; set; }
        public virtual ICollection<CategoryProduct> Categories { get; set; }
    }
    public class ProductPrice
    {
        public string Name { get; set; }
        public decimal? Value { get; set; }
    }
}
