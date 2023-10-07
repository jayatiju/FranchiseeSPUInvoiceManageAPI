using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class User
    {
        public string userid { get; set; }
        public string usertype { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string password { get; set; }
        public string refid { get; set; }
        public string regioncode { get; set; }
        public string email { get; set; }
        public string phnum { get; set; }
        public string isactive { get; set; }
        //public string newPassword { get; set; }
        public string region_desc { get; set; }
        
        //public string reference_desc { get; set; }
        
       
        public string branchcode { get; set; }
        public string segment { get; set; }
    }
}