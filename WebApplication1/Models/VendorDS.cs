using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class VendorDS
    {
        public string monthYear { get; set; }
        public string region { get; set; }
        public string vendorcode { get; set; }
        public string filePath { get; set; }
        public string fileName { get; set; }
        public string DigitalSignatureStatus { get; set; }
        public string TransactionNumber { get; set; }
        public string ErrorMessage { get; set; }
        public string ReferenceNum { get; set; }
        public string InvoicePdfDSStatus { get; set; }



    }
}