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
        public int Register(UserRegister dtouser)
        {
            IdentityMethods.CreatePasswordHash(dtouser.Password,out byte[] passwordHash,out byte[] passwordSalt);
            var _user = new User()
            {
                Name = dtouser.Name,
                Description = dtouser.Description,
                Role = dtouser.Role,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                Email = dtouser.Email
            };
            //user.Name = dtouser.Name;
            //user.Description = dtouser.Description;
            //user.Role = dtouser.Role;
            //user.PasswordHash = passwordHash;
            //user.PasswordSalt = passwordSalt;
            //user.Email = dtouser.Email;
            
            return _dBFacade.UserRegister(_user);

        }
        [HttpPost("login")]
        public IActionResult Login(LoginUser loginUser)
        {
            if(_dBFacade.IsUserExist(loginUser) != 1)
            {
                return BadRequest("Email is Incorrect.");
            }

            if(!IdentityMethods.IsPasswordCorrect(_dBFacade, loginUser, out byte[] phash, out byte[] psalt ))
            {
                return BadRequest("Password is incorrect");
            }
            User user = _dBFacade.GetUserInfo( loginUser.Email); 

            var token = IdentityMethods.CreateToken(user,_configuration);
            return Ok(token);


        }
        [HttpGet("UserInfo")]
        public IActionResult GetUserInfo()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            var user = IdentityMethods.GetCurrentUser(identity);
            return Ok(_dBFacade.GetUserInfo(user.Email));
        }

        [HttpPost("UpdateUserInfo")]
        public IActionResult UpdateUserInfo(UserInfoRequest dtouser)
        {
            var newuser = new User()
            {
                Name = dtouser.Name,
                Email = dtouser.Email,
                Description = dtouser.Description
            };
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            var _user = IdentityMethods.GetCurrentUser(identity);
            int result = _dBFacade.UpdateUserInfo(_user, newuser);
            var token = IdentityMethods.CreateToken(newuser, _configuration);
            return Ok(token);
        }
        [HttpPost("updateuserpassword")]
        public IActionResult UpdateUserPassword(UserPasswordRequest userPassword)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            var user = IdentityMethods.GetCurrentUser(identity);
            var dtoUser = new LoginUser
            {
                Email = user.Email,
                Password = userPassword.Password
            };
            
            if (!IdentityMethods.IsPasswordCorrect(_dBFacade, dtoUser, out byte[] phash, out byte[] psalt))
            {
                return BadRequest("Password is incorrect");
            }

            IdentityMethods.CreatePasswordHash(userPassword.NewPassword, out byte[] passwordHash, out byte[] passwordSalt);

            var newuser = new User();
            newuser.Email = dtoUser.Email;
            newuser.PasswordHash = passwordHash;
            newuser.PasswordSalt = passwordSalt;

            var result = _dBFacade.UpdateUserPassword(newuser);

            return Ok(result);
        }    

    }
}
