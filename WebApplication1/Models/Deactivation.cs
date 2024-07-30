using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class Deactivation
    {
        public string email { get; set; }

        public string branchcode { get; set; }

        public string vendorcode { get; set; }

        public string materialcode { get; set; }

        public string deactivationreason { get; set; }
    }
}