﻿using Microsoft.AspNetCore.Identity;

namespace Identity.Domain
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }
        public string Address { get; set; }
    }
}
