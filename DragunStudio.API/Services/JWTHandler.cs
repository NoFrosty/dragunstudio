using DragunStudio.API.Models;
using DragunStudio.Common;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DragunStudio.API.Services
{
    public class JWTHandler
    {
        private readonly ApplicationSettings applicationSettings;

        public JWTHandler(ApplicationSettings applicationSettings)
        {
            this.applicationSettings = applicationSettings;

        }
        public SigningCredentials GetSigningCredentials()
        {
            var key = Encoding.ASCII.GetBytes(applicationSettings.ApplicationSecretKey);
            var secret = new SymmetricSecurityKey(key);
            return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
        }
        public List<Claim> GetClaims(ApplicationUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.FirstName),
                new Claim(ClaimTypes.Surname, user.LastName)
            };
            return claims;
        }
        public JwtSecurityToken GenerateTokenOptions(SigningCredentials signingCredentials, List<Claim> claims)
        {
            var tokenOptions = new JwtSecurityToken(
                //issuer: _jwtSettings.GetSection("validIssuer").Value,
                //audience: _jwtSettings.GetSection("validAudience").Value,
                claims: claims,
                expires: DateTime.UtcNow.AddDays(7),
                signingCredentials: signingCredentials);
            return tokenOptions;
        }
    }
}
