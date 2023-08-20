using Microsoft.AspNetCore.Mvc;
using UCourseAPI.Models;
using UCourseAPI.Methods;
using Microsoft.AspNetCore.Authorization;
using UCourseAPI.Data;
using System.Security.Claims;

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

            if(! IdentityMethods.IsPasswordCorrect(_dBFacade,dtouser, out byte[] phash, out byte[] psalt ))
            {
                return BadRequest("Password is incorrect");
            }
            User user = _dBFacade.GetUserInfo(dtouser.Name,dtouser.Email); 

            var token = IdentityMethods.CreateToken(user,_configuration);
            return Ok(token);


        }
        [HttpGet]
        public IActionResult GetUserInfo()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            var user = IdentityMethods.GetCurrentUser(identity);
            return Ok(_dBFacade.GetUserInfo(user.Name,user.Email));
        }

        [HttpPost("UpdateUserInfo")]
        public IActionResult UpdateUserInfo(UserInfoRequest dtouser) 
        {
           
            var newuser = new User();
            newuser.Name = dtouser.Name;
            newuser.Email = dtouser.Email;
            newuser.Desciption = dtouser.Description;
            
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            var _user = IdentityMethods.GetCurrentUser(identity);
            int result = _dBFacade.UpdateUserInfo(_user, newuser);
            return Ok(result);
        }
        [HttpPost("updateuserpassword")]
        public IActionResult UpdateUserPassword(UserPasswordRequest userPassword)
        {
            if (userPassword.Password != userPassword.PasswordAgain)
                return BadRequest("Old Password is Wrong.");

            var identity = HttpContext.User.Identity as ClaimsIdentity;
            var user = IdentityMethods.GetCurrentUser(identity);
            var dtoUser = new DtoUser
            {
                Name = user.Name,
                Email = user.Email,
                Password = userPassword.Password
            };
            
            if (!IdentityMethods.IsPasswordCorrect(_dBFacade, dtoUser, out byte[] phash, out byte[] psalt))
            {
                return BadRequest("Password is incorrect");
            }

            IdentityMethods.CreatePasswordHash(dtoUser.Password, out byte[] passwordHash, out byte[] passwordSalt);

            var newuser = new User();
            newuser.Name = dtoUser.Name;
            newuser.Email = dtoUser.Email;
            newuser.PasswordHash = passwordHash;
            newuser.PasswordSalt = passwordSalt;

            var result = _dBFacade.UpdateUserPassword(newuser);

            return Ok(result);
        }

        

    }
}
