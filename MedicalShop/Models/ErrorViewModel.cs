using System;

namespace MedicalShop.Models
{
    public class ErrorViewModel
    {
        public string RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }

    public class ProductModel
    {
        public int data { get; set; }
        public string value { get; set; }
    }
}
