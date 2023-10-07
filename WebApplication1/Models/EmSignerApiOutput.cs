using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class EmSignerApiOutput
    {
        public string encryptedSessionKey { get; set; }
        public string encryptedJsonData { get; set; }

        public string encryptedHash { get; set; }
    }
}