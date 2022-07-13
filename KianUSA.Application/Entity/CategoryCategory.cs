using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KianUSA.Application.Entity
{
    public class CategoryCategory
    {
        public int Id { get; set; }
        public Guid CategoryId { get; set; }
        public string CategorySlug { get; set; }         
        public Guid ParentCategoryId { get; set; }
        public string ParentCategorySlug { get; set; }
        public int Order { get; set; }
    }
}
