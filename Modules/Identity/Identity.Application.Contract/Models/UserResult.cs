using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Application.Contracts.Models
{
    public class UserResult
    {
        public bool Succeeded { get; set; }
        public string Token { get; set; }
        public List<string> Errors { get; set; } = new();
    }
}
