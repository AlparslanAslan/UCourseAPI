using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UCourseAPI.Authentication;
using UCourseAPI.Data;
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

        [HttpPost]
        public int InsertCourse(CourseInsertRequest course)
        {
            var _dbFacade = new DBConnection();
            return _dbFacade.InsertCourse(course);
        }
        [HttpPut]
        public int UpdateCourse(CourseUpdateRequest course)
        {
            var dbmethode = new DBConnection();
            return _dbFacade.UpdateCourse(course);
        }
        [HttpDelete]
        public int DeleteCourse(int courseid)
        {
            var dbmethode = new DBConnection();
            return _dbFacade.DeleteCourse(courseid);
        }



    }
}