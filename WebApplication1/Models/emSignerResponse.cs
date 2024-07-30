using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Models
{
    public class emSignerResponse
    {
        public Status Status { get; set; }
        public string ErrorMessage { get; set; }
    }

    public enum Status
    {
        Failed = 2,
        Success = 1
    }
}
