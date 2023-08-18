using Microsoft.AspNetCore.Identity;
using System.Security.Cryptography;
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
        public static bool IsPasswordCorrect(DtoUser user, out byte[] passwordHash, out byte[] passwordSalt)
        {
            var _user = IdentityData.GetUserInfo(user.Name);
            using (var hmac = new HMACSHA512(_user.PasswordSalt))
            {
                passwordSalt = _user.PasswordSalt;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(user.Password));
            }
            return passwordHash.SequenceEqual(_user.PasswordHash);
        }
    }
}
