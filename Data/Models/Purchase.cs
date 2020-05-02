using System;

namespace Data.Models
{
    public class Purchase
    {
        public int Id { get; set; }
        public DateTime Date { get; set; } 
        public Product Product { get; set; }
        public int Quantity { get; set; }
        public int Cost { get; set; }
        public Supplier Supplier { get; set; }
    }

    public class PurchaseVM
    {
        public DateTime Date { get; set; } = DateTime.Now;
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public int Cost { get; set; }
        public int SupplierId { get; set; }
    }
}
