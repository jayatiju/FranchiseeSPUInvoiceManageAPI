using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    [Serializable]
    public class JsonDataEmSigner
    {
        public string FileType { get; set; } //d
        public string File { get; set; } //d
        public string ReferenceNumber { get; set; }
        public string Name { get; set; }
        public string AuthToken { get; set; }
        public int SignatureType { get; set; }
        public string SelectPage { get; set; }
        public string SignaturePosition { get; set; }
        public string SUrl { get; set; }
        public string FUrl { get; set; }
        public string CUrl { get; set; }
        public bool IsGSTN { get; set; }
        public bool IsGSTN3B { get; set; }       
    }
}