using Identity.Application.Configuration;
using Identity.Application.Interfaces;
using Identity.Domain;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;


namespace Identity.Infrastructure.Services
{
    public class JwtTokenService : IJwtTokenService
    {
        private readonly IOptions<TokenConfiguration> _tokenConfig;

        public JwtTokenService(IOptions<TokenConfiguration> tokenConfig)
        {
            _tokenConfig = tokenConfig;
        }

        public string GenerateJwtToken(ApplicationUser user)
        {
            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.Role, "User"), // یا بر اساس نقش‌های کاربر
            // سایر Claims دلخواه
        };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_tokenConfig.Value.SecretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _tokenConfig.Value.Issuer,
                audience: _tokenConfig.Value.Audience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(_tokenConfig.Value.ExpiryDurationInMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
