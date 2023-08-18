using Microsoft.AspNetCore.Mvc;
using UCourseAPI.Models;
using UCourseAPI.Methods;
using Microsoft.AspNetCore.Authorization;
using UCourseAPI.Data;

namespace UCourseAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class IdentityController : Controller
    {
        public static User user = new User();
        public readonly IConfiguration _configuration;

        public IdentityController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("register")]
        
        public int Register(DtoUser dtouser)
        {
            IdentityMethods.CreatePasswordHash(dtouser.Password,out byte[] passwordHash,out byte[] passwordSalt);

            user.Name = dtouser.Name;
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            return IdentityData.UserRegister(user);


        }
        [HttpPost("login")]
        public IActionResult Login(DtoUser dtouser)
        {
            if(IdentityData.IsUserExist(dtouser) != 1)
            {
                return BadRequest("User Not Exist.");
            }

            if(!IdentityMethods.IsPasswordCorrect(dtouser, out byte[] phash, out byte[] psalt ))
            {
                return BadRequest("Password is incorrect");
            }
            User user = IdentityData.GetUserInfo(dtouser.Name); 

            var token = IdentityMethods.CreateToken(user,_configuration);
            return Ok(token);


        }
    }
}
