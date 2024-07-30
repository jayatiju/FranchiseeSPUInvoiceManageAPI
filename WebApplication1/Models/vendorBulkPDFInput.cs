using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class vendorBulkPDFInput
    {
        public string startDate { get; set; }
        public string endDate { get; set; }
        //public string regionCode { get; set; }

        public string region { get; set; }

        public string vendorcode { get; set; }
    }
}