using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class InvoiceInput
    {
        public string CompanyCode { get; set; }
        public string DocumentNumber { get; set; }
        public string EndDate { get; set; }
        public string FiscalYear { get; set; }
        public string SegmentCode { get; set; }
        public string StartDate { get; set; }
        public string Region { get; set; }
    }
}