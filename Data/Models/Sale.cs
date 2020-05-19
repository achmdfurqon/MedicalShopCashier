
namespace Data.Models
{
    public class Sale
    {
        public int Id { get; set; }
        public Product Product { get; set; }
        public int Quantity { get; set; }
        public int SubTotal { get; set; }
        public bool Status { get; set; } 
        public Transaction Transaction { get; set; }
    }

    public class SaleVM
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }

    public class SaleReport
    {
        public string Name { get; set; }
        public int? Qty { get; set; } = 0;
        public int? Price { get; set; } = 0;
    }
}