using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KianUSA.Application.Services.Product
{
    public class ProductsWithTotalItemDto
    {
        public List<ProductDto> Products { get; set; }
        public int TotalItems { get; set; }
    }
}
