
using System.Collections.Generic;

namespace Data.Models
{
    public class Product : Base
    {
        public int Stock { get; set; } 
        public int Price { get; set; }
        public string Description { get; set; }
    }

    public class DataTableProducts
    {
        public IEnumerable<Product> data { get; set; }
        public int length { get; set; }
        public int filterLength { get; set; }
    }
}