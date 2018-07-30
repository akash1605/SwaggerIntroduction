using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace SwaggerIntroduction.Security
{
    public class TokenHelper : IHandleTokens
    {
        public string Create(string userEmail, string key)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userEmail),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };
            var encodedKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var creds = new SigningCredentials(encodedKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds,
                audience: "https://localhost:44334/api",
                issuer: "https://localhost:44334/api"
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GetEmailFromClaims(ClaimsPrincipal claims)
        {
            var userClaim = claims.Claims.Select(x => x.Subject).FirstOrDefault();

            var requiredClaim = userClaim?.Claims.FirstOrDefault(clm =>
                clm.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");

            return requiredClaim?.Value;
        }
    }

    public interface IHandleTokens
    {
        string Create(string userEmail, string key);

        string GetEmailFromClaims(ClaimsPrincipal claims);
    }
}
