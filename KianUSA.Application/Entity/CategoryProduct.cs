using System;

namespace KianUSA.Application.Entity
{
    public class CategoryProduct
    {
        public int Id { get; set; } 
        public Guid ProductId { get; set; }
        public Guid CategoryId { get; set; }
    }
}
