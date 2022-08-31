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
        public PublishedCatalogTypeDto PublishedCatalogType { get; set; }
        public List<CategoryParameterDto> Parameters { get; set; }
        public List<CategoryParameterDto> Features { get; set; }
        public List<ChildCategoryDto> Children { get; set; }
        public int Order { get; set; }
        public List<string> Tags { get; set; }
        public List<string> ImagesUrl { get; set; }
    }
    public enum PublishedCatalogTypeDto
    {
        None,
        SingleAndMain,
        Single,
        Main
    }
    public class CategoryParameterDto
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
    public class ChildCategoryDto
    {
        public Guid Id { get; set; }
        public string Slug { get; set; }
        public int Order { get; set; }
    }
}
