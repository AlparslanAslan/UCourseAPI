using Microsoft.AspNetCore.Mvc;
using UCourseAPI.Models;
using UCourseAPI.Methods;
using Microsoft.AspNetCore.Authorization;
using UCourseAPI.Data;
using System.Security.Claims;
using UCourseAPI.BusinessLogic;

namespace UCourseAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class IdentityController : ControllerBase  
    {
        public static UserResponse user = new UserResponse();
        public readonly IConfiguration _configuration;
        private readonly DBFacade _dBFacade;
        public IdentityController(IConfiguration configuration,DBFacade dBFacade)
        {
            _configuration = configuration;
            _dBFacade = dBFacade;
        }
       
        [HttpPost("register")]
        public IActionResult Register(UserRegister dtouser)
        {
            if(!InputChecker.IsUserRegisterInputValid(dtouser, out string errormessage))
            {
                return BadRequest(errormessage);
            }
            IdentityMethods.CreatePasswordHash(dtouser.Password,out byte[] passwordHash,out byte[] passwordSalt);
            var _user = new UserResponse()
            {
                Name = dtouser.Name,
                Description = dtouser.Description,
                Role = dtouser.Role,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                Email = dtouser.Email
            };
            try
            {
               _dBFacade.UserRegister(_user);
                return Ok(IdentityMethods.CreateToken(_user, _configuration));

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
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
            UserResponse user = _dBFacade.GetUserInfo( loginUser.Email); 

            var token = IdentityMethods.CreateToken(user,_configuration);
            return Ok(token);


        }
       
        
        [HttpGet("UserInfo")]
        [Authorize(Roles = "User,Author")]
        public IActionResult GetUserInfo()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            var user = IdentityMethods.GetCurrentUser(identity);
            return Ok(_dBFacade.GetUserInfo(user.Email));
        }

      
        [HttpPost("UpdateUserInfo")]
        public IActionResult UpdateUserInfo(UserInfoRequest dtouser)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            var _user = IdentityMethods.GetCurrentUser(identity);
            if(!InputChecker.IsUserInfoUpdateValid(dtouser,_user.Role,out string errormessage))
            {
                return BadRequest(errormessage);
            }
            var newuser = new UserResponse()
            {
                Name = dtouser.Name,
                Email = dtouser.Email,
                Description = dtouser.Description
            };
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
            if(! InputChecker.IsPasswordValid(userPassword.NewPassword,out string errormessage))
            {
                return BadRequest(errormessage);
            }
            IdentityMethods.CreatePasswordHash(userPassword.NewPassword, out byte[] passwordHash, out byte[] passwordSalt);

            var newuser = new UserResponse();
            newuser.Email = dtoUser.Email;
            newuser.PasswordHash = passwordHash;
            newuser.PasswordSalt = passwordSalt;

            var result = _dBFacade.UpdateUserPassword(newuser);

            return Ok(result);
        }    

    }
}
