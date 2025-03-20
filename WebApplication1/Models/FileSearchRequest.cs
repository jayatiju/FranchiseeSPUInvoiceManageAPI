using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
	public class FileSearchRequest
	{
        public int Year { get; set; }
        public string Month { get; set; }
        public string Segment { get; set; }
        public string VendorCode { get; set; }
        public string DigitallySigned { get; set; }
    }
}