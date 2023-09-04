using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UCourseAPI.Authentication;
using UCourseAPI.Data;
using UCourseAPI.Methods;
using UCourseAPI.Models;

namespace UCourseAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CourseController : ControllerBase
    {
        private readonly ILogger<CourseController> _logger;
        private readonly DBFacade _dbFacade;
        public CourseController(ILogger<CourseController> logger, DBFacade dbFacade)
        {
            _logger = logger;
            _dbFacade = dbFacade;
        }

        //[ServiceFilter(typeof(ApiAuthFilter))]
        [HttpGet("getallcourses")]
        public IActionResult GetALlCourses()
        {
            var result =  _dbFacade.GetAllCourses(null, null, null, null, 0, 0);
            return Ok(result);
            
        }
        
        
        [HttpGet("getcourses")]
        [Authorize(Roles ="User")]
        public IActionResult GetCourses(string? name,string? category,string? language,string? subcategory,int level,int orderby)
        {
            var result = _dbFacade.GetAllCourses(name, category, language, subcategory, level, orderby);
            return Ok(result);
        }
        
        
        [HttpPost("InsertCourse")]
        [Authorize(Roles = "Author")]
        public IActionResult InsertCourse(CourseInsertRequest course)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            var user = IdentityMethods.GetCurrentUser(identity);
            var _course = new CourseDbParameters()
            {
                Name = course.Name,
                Categories = course.Categories,
                Subcategories = course.Subcategories,
                Description = course.Description,
                Language = course.Language,
                Level = course.Level,
                Price = course.Price,
                AuthorEmail = user.Email
            };
            var result = _dbFacade.InsertCourse(_course);
            
            return Ok(result);
        }
        
        
        [HttpPut("updatecourse")]
        [Authorize(Roles = "Author")]
        public IActionResult UpdateCourse(CourseUpdateRequest course)
        {
            var result = _dbFacade.UpdateCourse(course);
            return Ok(result);
        }
        
        
        [HttpDelete("deletecourse")]
        [Authorize(Roles = "Author")]
        public IActionResult DeleteCourse(int courseid)
        {
            var result = _dbFacade.DeleteCourse(courseid);
            return Ok(result);
        }
        
        
        [HttpPost("purchase")]
        [Authorize(Roles = "User")]
        public IActionResult PurchaseCourse(int CourseId)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            var user = IdentityMethods.GetCurrentUser(identity);
            var result = _dbFacade.PurchaseCourse(CourseId, user);
            return Ok(result);

        }
        
        
        [HttpGet("getusercourses")]
        [Authorize(Roles = "User,Author")]
        public IActionResult GetUserCourses()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            var user = IdentityMethods.GetCurrentUser(identity);
            IEnumerable<CourseResponse> result = new List<CourseResponse>();
            if(user.Role=="User")
            {
                 result = _dbFacade.GetUserCourseList(user);
            }
            else if(user.Role == "Author")
            {
                 result = _dbFacade.GetAuthorCourses(user);
            }
            return Ok(result);
        }
        
        
        [HttpPost("insertreview")]
        [Authorize(Roles = "User")]
        public IActionResult InsertReview(Review review)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            var _user = IdentityMethods.GetCurrentUser(identity);
            review.UserEmail = _user.Email;
            var result = _dbFacade.InsertReview(_user, review);
            return Ok(result);
        }
        
        
        [HttpGet("coursedetails")]
        [Authorize(Roles = "Author")]
        public IActionResult GetCourseDetails(int courseId)
        {
            var result =  _dbFacade.GetCourseDetails(courseId);
            return Ok(result);
        }
        
        
        [HttpPost("addscore")]
        [Authorize(Roles = "User")]
        public IActionResult AddScore(Score score)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            var _user = IdentityMethods.GetCurrentUser(identity);
            score.UserEmail = _user.Email;
            var result = _dbFacade.AddScore(score);
            return Ok(result);
        }
    }
}