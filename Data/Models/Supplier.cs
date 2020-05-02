
using System.Collections.Generic;

namespace Data.Models
{
    public class Supplier : Base { }

    public class DataTableSuppliers
    {
        public IEnumerable<Supplier> data { get; set; }
        public int length { get; set; }
        public int filterLength { get; set; }
    }
}