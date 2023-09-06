using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using UCourseAPI.Authentication;
using UCourseAPI.BusinessLogic;
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
            var result =  _dbFacade.GetAllCourses(null, null, null, null, 0);
            return Ok(result);
            
        }
        
        
        [HttpGet("getcourses")]
        public IActionResult GetCourses(string? name,string? category,string? language,string? subcategory,int level)
        {
            var result = _dbFacade.GetAllCourses(name, category, language, subcategory, level);
            return Ok(result);
        }
        
        
        [HttpPost("InsertCourse")]
        [Authorize(Roles = "Author")]
        public IActionResult InsertCourse(CourseInsertRequest course)
        {
            if (!InputChecker.CourseInsertIsValid(course, out string errortext))
            {
                return BadRequest(errortext);
            }

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
                AuthorId = user.Id
            };
            
            return _dbFacade.InsertCourse(_course) == 1 ? Ok() : NoContent();
        }
        
        
        [HttpPut("updatecourse")]
        [Authorize(Roles = "Author")]
        public IActionResult UpdateCourse(CourseUpdateRequest course)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            var user = IdentityMethods.GetCurrentUser(identity);

            if (!InputChecker.CourseUpdateIsValid(course,user.Id,out string errortext))
            {
                return BadRequest(errortext);
            }
            return _dbFacade.UpdateCourse(course)==1 ? Ok() : NoContent();
            
        }
        
        
        [HttpDelete("deletecourse")]
        [Authorize(Roles = "Author")]
        public IActionResult DeleteCourse(int courseid)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            var user = IdentityMethods.GetCurrentUser(identity);
            if (!InputChecker.CourseDeleteIsValid(courseid, user.Id, out string errortext))
            {
                return BadRequest(errortext);
            }
            return _dbFacade.DeleteCourse(courseid) == 1 ? Ok() : NoContent(); 
        }
        

        [HttpPost("purchase")]
        [Authorize(Roles = "User")]
        public IActionResult PurchaseCourse(int CourseId)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            var user = IdentityMethods.GetCurrentUser(identity);

            var isPurchased = InputChecker.IsAlreadyPurchased(user.Id, CourseId, out string errormessage);
            if (isPurchased)
            {
                return BadRequest(errormessage);
            }
           
            return _dbFacade.PurchaseCourse(CourseId, user) == 1 ? Ok() : NoContent(); 
        }
        
        
        [HttpGet("getusercourses")]
        [Authorize(Roles = "User,Author")]
        public IActionResult GetUserCourses()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            var user = IdentityMethods.GetCurrentPerson(identity);
            var result = user.GetCourses();
            return result.IsNullOrEmpty() == true ? NotFound() : Ok(result);
        }
        
        
        [HttpPost("insertreview")]
        [Authorize(Roles = "User")]
        public IActionResult InsertReview(ReviewInsertRequest request)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            var _user = IdentityMethods.GetCurrentUser(identity);
            var review = new Review()
            {
                CourseId = request.CourseId,
                ReviewText = request.ReviewText,
                UserId = _user.Id                
            };
            var result = _dbFacade.InsertReview(_user, review);
            return result==1 ? Ok(result) : NoContent();
        }
        
        
        [HttpGet("coursedetails")]
        [Authorize(Roles = "Author")]
        public IActionResult GetCourseDetails(int courseId)
        {
            var courseInfo = _dbFacade.GetCourseDetails(courseId);
            var courseReviews = _dbFacade.GetCourseReviews(courseId);
            courseInfo.Review = courseReviews;
            return courseReviews.Count > 0 ? Ok(courseInfo): NotFound();    
        }
        
        
        [HttpPost("addscore")]
        [Authorize(Roles = "User")]
        public IActionResult AddScore(Score score)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            var _user = IdentityMethods.GetCurrentUser(identity);
            score.UserId = _user.Id;
            var result = _dbFacade.AddScore(score);
            return Ok(result);
        }
    }
}