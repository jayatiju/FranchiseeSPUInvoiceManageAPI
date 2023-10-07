using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class InvoiceGenerationInput
    {
        public string startDate { get; set; }
        public string endDate { get; set; }
        //public string regionCode { get; set; }

        public string segment { get; set; }

    }
}