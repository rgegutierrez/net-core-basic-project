using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreJwtExample.Models
{
    public class UserActivationCode
    {
        public string ToEmail { get; set; }
        public string UserName { get; set; }

        public string ActivationCode { get; set; }
    }
}
