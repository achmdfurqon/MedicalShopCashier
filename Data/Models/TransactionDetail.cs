using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Models
{
    public class TransactionDetail
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public DateTime Date { get; set; } 
        public int TotalProduct { get; set; }
        public int TotalPrice { get; set; }
        public int Cash { get; set; }
        public int Change { get; set; }
        public List<Sales> Sales { get; set; }
    }

    public class Sales
    {
        public int Id { get; set; }
        public Product Product { get; set; }
        public int Quantity { get; set; }
        public int SubTotal { get; set; }
        public bool Status { get; set; } 
    }

    public class DataTableTransactions
    {
        public IEnumerable<TransactionDetail> data { get; set; }
        public int length { get; set; }
        public int filterLength { get; set; }
    }
}
