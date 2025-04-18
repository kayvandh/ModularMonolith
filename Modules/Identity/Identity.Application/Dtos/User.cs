using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Application.Dtos
{
    public class User
    {
        public bool Succeeded { get; set; }
        public string Token { get; set; }
    }
}
