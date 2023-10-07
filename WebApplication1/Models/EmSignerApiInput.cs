using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class EmSignerApiInput
    {
        public List<string> FileLocations { get; set; }
        public string vendorcode { get; set; }
        public string vendorname { get; set; }
        public List<string> OrderId { get; set; }
    }
}