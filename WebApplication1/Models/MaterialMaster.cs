using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class MaterialMaster
    {
        public string materialcode { get; set; }
        public string materialcat { get; set; }
        public string categoryhierarchyid { get; set; }
        public string taxtariffcode { get; set; }
        public string taxtype { get; set; }
        public string materialdesc { get; set; }
        public string uom { get; set; }
        public string productid { get; set; }
        public string groupid { get; set; }
        public string isactive { get; set; }

    }
}