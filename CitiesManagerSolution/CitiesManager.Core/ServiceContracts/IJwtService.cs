﻿using CitiesManager.Core.DTO;
using CitiesManager.Core.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CitiesManager.Core.ServiceContracts
{
    public interface IJwtService
    {
        AuthenticationResponse CreateJwtToken(ApplicationUser user);

        // ClaimsPrincipal represents the user details
        ClaimsPrincipal? GetPrincipalFromJwtToken(string? token);
    }
}
