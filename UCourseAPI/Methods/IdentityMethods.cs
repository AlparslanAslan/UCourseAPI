using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using UCourseAPI.Data;
using UCourseAPI.Models;

namespace UCourseAPI.Methods
{
    public static class IdentityMethods
    {
        public static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }
        public static bool IsPasswordCorrect(DBFacade dBFacade,LoginUser user, out byte[] passwordHash, out byte[] passwordSalt)
        {
            var _user = dBFacade.GetUserInfo(user.Email);
            if (_user == null)
            {
                passwordSalt = null;
                passwordHash = null;
                return false;
            }
            using (var hmac = new HMACSHA512(_user?.PasswordSalt))
            {
                passwordSalt = _user.PasswordSalt;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(user.Password));
            }
            return passwordHash.SequenceEqual(_user.PasswordHash);
        }
        public static string CreateToken(User user,IConfiguration configuration)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email,user.Email),
                new Claim(ClaimTypes.Role,user.Role ?? "User" ),
                new Claim(ClaimTypes.Name,user.Name)
            };
            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(
                configuration.GetSection("JwtSettings:Key").Value));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            var token = new JwtSecurityToken
                (
                configuration["JwtSetting:Issuer"],
                configuration["JwtSetting:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds
                );
            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return jwt;
            
        }
        public static User GetCurrentUser(ClaimsIdentity identity)
        {
            if(identity != null)
            {
                var claims = identity.Claims;
                return new User
                {
                    Name = claims.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value,
                    Email = claims.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value,
                    Role = claims.FirstOrDefault(x => x.Type == ClaimTypes.Role)?.Value
                };
            }
            return null;
        }
    }
}
