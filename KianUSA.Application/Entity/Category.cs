using System;

namespace KianUSA.Application.Entity
{
    public class Category
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
        public string ShortDescription { get; set; }
        public string Description { get; set; }
        /// <summary>
        /// Json Format
        /// <see cref="CategoryParameter"/>
        /// </summary>
        public string Parameter { get; set; }
        public int Order { get; set; }
    }
    public class CategoryParameter
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }

}
