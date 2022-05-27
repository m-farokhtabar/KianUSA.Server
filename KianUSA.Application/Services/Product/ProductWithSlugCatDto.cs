using System;
using System.Collections.Generic;

namespace KianUSA.Application.Services.Product
{
    public class ProductWithSlugCatDto : ProductDto
    {
        public List<string> CategorySlug { get; set; }
    }
}
