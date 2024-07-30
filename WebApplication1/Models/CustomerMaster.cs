using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class CustomerMaster
    {
        public string branchcode { get; set; }
        public string branchname { get; set; }
        public string gstinnum { get; set; }
        public string address { get; set; }
        public string pincode { get; set; }
        public string regioncode { get; set; }
        public string regiondesc { get; set; }
        public string pannum { get; set; }
        public string mobilenum { get; set; }
        public string emailid { get; set; }
        public string isactive { get; set; }
        public string segment { get; set; }
        public string customercin { get; set; }
        public string plantdesc { get; set; }
    }
}