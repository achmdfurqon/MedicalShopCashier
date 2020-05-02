using System;

namespace Data.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        public string Code { get; set; } = Guid.NewGuid().ToString().Replace("-","").Substring(0, 10).ToUpper();
        public DateTime Date { get; set; } = DateTime.Now;
        public int TotalProduct { get; set; }
        public int TotalPrice { get; set; }
        public int Cash { get; set; }
        public int Change { get; set; }
    }
}