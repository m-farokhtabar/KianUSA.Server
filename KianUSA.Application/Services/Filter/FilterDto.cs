using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KianUSA.Application.Services.Filter
{
    public class FilterDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int Order { get; set; }
        public List<string> Tags { get; set; }
    }
}
