using Microsoft.AspNetCore.Mvc;
using UCourseAPI.Data;
using UCourseAPI.Models;

namespace UCourseAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CourseController : ControllerBase
    {
        private readonly ILogger<CourseController> _logger;
        public CourseController(ILogger<CourseController> logger)
        {
            _logger = logger;
        }
        [HttpGet]
        public IEnumerable<Course> GetCourses(string? name,string? category,string? language,string? subcategory,int level,int orderby)
        {
            var dbmethode = new DBConnection();
            return dbmethode.GetAllCourses(name,category,language,subcategory,level,orderby);
        }

        [HttpPost]
        public int InsertCourse(CourseInsertRequest course)
        {
            var dbmethode = new DBConnection();
            return dbmethode.InsertCourse(course);
        }
        [HttpPut]
        public int UpdateCourse(CourseUpdateRequest course)
        {
            var dbmethode = new DBConnection();
            return dbmethode.UpdateCourse(course);
        }
        [HttpDelete]
        public int DeleteCourse(int courseid)
        {
            var dbmethode = new DBConnection();
            return dbmethode.DeleteCourse(courseid);
        }



    }
}