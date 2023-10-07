using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class InvoiceGenerationOutput
    {
        public string InvoiceNumber { get; set; }
        public string InvoiceDate { get; set; }
        public string SPUNumber { get; set; }
        public string CRMTicketNumber { get; set; }
        public string DocumentNumber { get; set; }
        public string DocumentDate { get; set; }
        public string VendorName { get; set; }
        public string VendorCode { get; set; }
        public string CustomerName { get; set; }
        public string CustomerCode { get; set; }
        public string ShipToPartyNumber { get; set; }
        public string ShipToPartyName { get; set; }
        public string SubTotal { get; set; }
        public string SGST { get; set; }
        public string CGST { get; set; }
        public string IGST { get; set; }
        public string RoundOff { get; set; }
        public string GrandTotal { get; set; }
        public string InvoiceNumberStatus { get; set; }
        public string InvoicePdfStatus { get; set; }
        public string InvoicePdfDigitalSigStatus { get; set; }
        public string InvoicePdfLocation { get; set; }

        public string regionCode { get; set; }
        public string segmentCode { get; set; }
    }
}