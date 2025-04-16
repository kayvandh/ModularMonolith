using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Application.Contracts.Models
{
    public class LoginModel
    {
        [Required]
        public string UserName { get; set; }  // یا ایمیل

        [Required]
        [MinLength(6)]
        public string Password { get; set; }
    }
}
