
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
}