﻿using Identity.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Application.Interfaces
{
    public interface IJwtTokenService
    {
        string GenerateJwtToken(ApplicationUser user);
    }
}
