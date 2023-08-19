using Microsoft.AspNetCore.Mvc;
using UCourseAPI.Models;
using UCourseAPI.Methods;
using Microsoft.AspNetCore.Authorization;
using UCourseAPI.Data;

namespace UCourseAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class IdentityController : ControllerBase
    {
        public static User user = new User();
        public readonly IConfiguration _configuration;
        private readonly DBFacade _dBFacade;

        public IdentityController(IConfiguration configuration,DBFacade dBFacade)
        {
            _configuration = configuration;
            _dBFacade = dBFacade;
        }

        [HttpPost("register")]
        
        public int Register(DtoUser dtouser)
        {
            IdentityMethods.CreatePasswordHash(dtouser.Password,out byte[] passwordHash,out byte[] passwordSalt);

            user.Name = dtouser.Name;
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
            
            return _dBFacade.UserRegister(user);
            //return _dbFacade.UserRegister(user);


        }
        [HttpPost("login")]
        public IActionResult Login(DtoUser dtouser)
        {
            if(_dBFacade.IsUserExist(dtouser) != 1)
            {
                return BadRequest("User Not Exist.");
            }

            if(! IdentityMethods.IsPasswordCorrect(dtouser, out byte[] phash, out byte[] psalt ))
            {
                return BadRequest("Password is incorrect");
            }
            User user = _dBFacade.GetUserInfo(dtouser.Name); 

            var token = IdentityMethods.CreateToken(user,_configuration);
            return Ok(token);


        }
    }
}
