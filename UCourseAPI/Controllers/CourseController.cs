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
        [HttpGet]
        //[ServiceFilter(typeof(ApiAuthFilter))]
        //[Authorize(Roles ="Administrator")]
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
        [HttpPut]
        public int UpdateCourse(CourseUpdateRequest course)
        {
            return _dbFacade.UpdateCourse(course);
        }
        [HttpDelete]
        public int DeleteCourse(int courseid)
        {
            return _dbFacade.DeleteCourse(courseid);
        }
        [HttpPost("purchase")]
        public int PurchaseCourse(int courseId)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            var user = IdentityMethods.GetCurrentUser(identity);
            return _dbFacade.PurchaseCourse(courseId, user);

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

    }
}