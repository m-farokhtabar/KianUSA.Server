using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KianUSA.Domain.Entity
{
    public class Tutorial
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Abstract { get; set; }
        public string Description { get; set; }
        public string[] VideoUrls { get; set; }
        public string[] ImageUrls { get; set; }
    }
}
