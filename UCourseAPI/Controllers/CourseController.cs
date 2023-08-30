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
        //[Authorize(Roles ="Administrator")]
        [HttpGet("getallcourses")]
        public IEnumerable<Course> GetALlCourses()
        {
            return _dbFacade.GetAllCourses(null, null, null, null, 0, 0);
            //var dbmethode = new DBConnection();
            //return dbmethode.GetAllCourses(name,category,language,subcategory,level,orderby);
        }
        [HttpGet("getcourses")]
       
        public IEnumerable<Course> GetCourses(string? name,string? category,string? language,string? subcategory,int level,int orderby)
        {
            return _dbFacade.GetAllCourses(name, category, language, subcategory, level, orderby);
            //var dbmethode = new DBConnection();
            //return dbmethode.GetAllCourses(name,category,language,subcategory,level,orderby);
        }
        [HttpPost("InsertCourse")]
        public int InsertCourse(CourseInsertRequest course)
        {
            return _dbFacade.InsertCourse(course);
        }
        [HttpPut("updatecourse")]
        public int UpdateCourse(CourseUpdateRequest course)
        {
            return _dbFacade.UpdateCourse(course);
        }
        [HttpDelete("deletecourse")]
        public int DeleteCourse(int courseid)
        {
            return _dbFacade.DeleteCourse(courseid);
        }
        [HttpPost("purchase")]
        public int PurchaseCourse(int CourseId)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            var user = IdentityMethods.GetCurrentUser(identity);
            return _dbFacade.PurchaseCourse(CourseId, user);

        }
        [HttpGet("getusercourses")]
        public IEnumerable<Course> GetUserCourses()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            var user = IdentityMethods.GetCurrentUser(identity);
            if(user.Role=="User")
            {
                return  _dbFacade.GetUserCourseList(user);
            }
            else if(user.Role == "Author")
            {
                return _dbFacade.GetAuthorCourses(user);
            }
            return null;
            

        }
        [HttpPost("insertreview")]
        public IActionResult InsertReview(string review, int courseId)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            var _user = IdentityMethods.GetCurrentUser(identity);

            var result = _dbFacade.InsertReview(_user, review, courseId);
            return Ok(result);
        }
        [HttpGet("coursedetails")]
        public IActionResult GetCourseDetails(int courseId)
        {
            var result =  _dbFacade.GetCourseDetails(courseId);
            return Ok(result);
        }
        [HttpPost("addscore")]
        public IActionResult AddScore(decimal score,int courseId)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            var _user = IdentityMethods.GetCurrentUser(identity);
            var result = _dbFacade.AddScore(score, _user, courseId);
            return Ok(result);
        }
    }
}