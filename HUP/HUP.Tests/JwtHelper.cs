using HUP.Core.Entities.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace HUP.Tests
{
    public static class JwtHelper
    {
        public static string GenerateTokenForUser(User user, string roleName)
        {
            var key = "this_is_a_very_long_secret_key_for_testing_12345";
            var issuer = "http://localhost:5000";
            var audience = "http://localhost:5000";

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.FullName ?? "Test"),
                new Claim(ClaimTypes.Role, roleName)
            };

            var permissions = HUP.Core.Constants.AppPermissions.GetAll();
            foreach (var perm in permissions)
            {
                claims.Add(new Claim("Permission", perm));
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(1),
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
