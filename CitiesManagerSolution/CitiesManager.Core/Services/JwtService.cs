using CitiesManager.Core.DTO;
using CitiesManager.Core.Identity;
using CitiesManager.Core.ServiceContracts;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace CitiesManager.Core.Services
{
    public class JwtService : IJwtService
    {
        private readonly IConfiguration _configuration;

        public JwtService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public AuthenticationResponse CreateJwtToken(ApplicationUser user)
        {
            // Eg: DateTime.UtcNow = 5:00 pm
            // + AddMinutes(10) = 5:10 pm
            // Create a DateTime object representing the token
            // expiration time by adding the number of minutes
            // specified in the configuration to the current UTC time.
           DateTime expiration = DateTime.Now.AddMinutes
                (Convert.ToDouble(_configuration["Jwt:EXPIRATION_MINUTES"]));

            // Payload - User Details
            // Create an array of Claim objects representing the 
            // user's claims, such as their ID, name, email, etc.
            Claim[] claims =
            [
                // Sub (Subject) -> User ID
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),

                // JWT unique ID - Json Web Tocken id
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),

                // Issued at (date and time of token generation)
                new Claim(JwtRegisteredClaimNames.Iat, DateTime.Now.ToString()),
                
                // Following claims are optional
                // Unique name identifier of the user (Email)
                new Claim(ClaimTypes.NameIdentifier, user.Email.ToString()),

                // Name of the user
                new Claim(ClaimTypes.Name, user.PersonName),

                new Claim(ClaimTypes.Email, user.Email)

            ];

            // I used dotnet secrets to generate the key: value pair
            // this is how it works in that way
            // Specifing the hash
            // Creating a SymetricSecurityKey object using the key
            // specified in the configuration.
            SymmetricSecurityKey securityKey =
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Key"]));

            // Specifing the hash algorithm
            // Create a SigningCredentials object with the security key
            // and the HMACSHA256 algorithm
            SigningCredentials signingCredentials =
                new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            // Setting up everything to generate the token
            // Create a JwtSecurityToken object with the given issuer,
            // audience, claims, expiration, and signing credentials.
            JwtSecurityToken tokenGenerator = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: expiration,
                signingCredentials: signingCredentials
                );

            // Generate the token
            // Create a JwtSecurityTokenHandler object and use it to 
            // write the token as a string
            JwtSecurityTokenHandler tokenHandler =
                new JwtSecurityTokenHandler();
            string token = tokenHandler.WriteToken(tokenGenerator);

            // Create and return an AuthenticationResponse object
            // containg the token, user email, user name, and token
            // expiration time.
            return new AuthenticationResponse()
            {
                Token = token,
                Email = user.Email,
                PersonName = user.PersonName,
                Expiration = expiration,
                RefreshToken = GenerateRefreshToken(),
                RefreshTokenExpirationDate = DateTime.Now.AddMinutes(Convert.ToInt32(_configuration["RefreshToken:EXPIRATION_MINUTES"]))
            };
        }

        public ClaimsPrincipal? GetPrincipalFromJwtToken(string? token)
        {
            var tokenValidationParameters = new TokenValidationParameters()
            {
                ValidateAudience = true,
                ValidAudience = _configuration["Jwt:Audience"],
                ValidateIssuer = true,
                ValidIssuer = _configuration["Jwt:Issuer"],

                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(_configuration["Key"])),

                ValidateLifetime = false
            };

            JwtSecurityTokenHandler jwtSecurityTokenHandler = 
                new JwtSecurityTokenHandler();

            // This will return information about the user from 
            // the payload
            ClaimsPrincipal principal = jwtSecurityTokenHandler
                .ValidateToken(token, tokenValidationParameters,out SecurityToken securityToken);
        
            if(securityToken is not 
                JwtSecurityToken jwtSecurityToken || 
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token");
            }

            return principal;
        }

        // Creates a refresh token
        private string GenerateRefreshToken()
        {
            byte[] bytes = new byte[64];
            var randomNumberGenerator = RandomNumberGenerator.Create();

            // fill bytes array
            randomNumberGenerator.GetBytes(bytes);

            // return in form base64 string
            return Convert.ToBase64String(bytes);

        }
    }
}
