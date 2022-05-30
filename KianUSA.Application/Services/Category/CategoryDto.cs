using System;
using System.Collections.Generic;

namespace KianUSA.Application.Services.Category
{
    public class CategoryDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
        public string ShortDescription { get; set; }
        public string Description { get; set; }
        public List<CategoryParameterDto> Parameters { get; set; }
        public List<CategoryParameterDto> Features { get; set; }
        public int Order { get; set; }
        public List<string> ImagesUrl { get; set; }
    }
    public class CategoryParameterDto
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
