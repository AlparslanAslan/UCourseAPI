using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Reflection;
using System.Security.Claims;
using UCourseAPI.Authentication;
using UCourseAPI.BusinessLogic;
using UCourseAPI.Data;
using UCourseAPI.Methods;
using UCourseAPI.Models;
using Serilog;
using Microsoft.EntityFrameworkCore;
using UCourseAPI.Repositories;
using System.Data.Common;

namespace UCourseAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CourseController : ControllerBase
    {
        private readonly ILogger<CourseController> _logger;
        private readonly DBFacade _dbFacade;
        private readonly ICourseRepo _courseRepo;
        public CourseController(ILogger<CourseController> logger, DBFacade dbFacade,ICourseRepo courseRepo)
        {
            _courseRepo = courseRepo;
            _logger = logger;
            _dbFacade = dbFacade;
        }

        //[ServiceFilter(typeof(ApiAuthFilter))]
        [HttpGet("getallcourses")]
        public IActionResult GetALlCourses()
        {
            try
            {
                var result = _courseRepo.GetAll();
                Log.Information<List<CourseResponse>>("{@result}", result);
                return Ok(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return StatusCode(500, ex.Message);
            }
        }


        [HttpGet("getcourses")]
        public IActionResult GetCourses(string? name,string? category,string? language,string? subcategory,int level)
        {
            try
            {
                var result = _dbFacade.GetAllCourses(name, category, language, subcategory, level);
                return Ok(result);

            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return StatusCode(500, ex.Message);
            }
        }
        [HttpGet("getcoursebyid")]
        public IActionResult GetCoursesById(int CourseId)
        {
            try
            {
                var result = _dbFacade.GetCoursesById(CourseId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return StatusCode(500, ex.Message);
            }

        }


        [HttpPost("InsertCourse")]
      //  [Authorize(Roles = "Author")]
        public async Task<IActionResult> InsertCourseAsync([FromForm] CourseInsertRequest course)
        {
            byte[] fileData = null;
            using (var memoryStream = new MemoryStream())
            {
                await course.Files.CopyToAsync(memoryStream);
                fileData = memoryStream.ToArray();
            }

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
                //AuthorId = user.Id,
                Document = fileData
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
            if (!user.CourseDeleteIsValid(courseid, user.Id, out string errortext))
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

        [HttpGet("document")]
        [Authorize(Roles = "User")]
        public IActionResult GetUserCourseDocument(int courseId)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            var user = IdentityMethods.GetCurrentPerson(identity);
            List<Document> documents = new List<Document>();
            if(InputChecker.IsAlreadyPurchased(user.Id, courseId, out string message))
                documents = _dbFacade.GetCourseDocuments(courseId);
            return documents.IsNullOrEmpty() == true ? NotFound() : Ok(documents);
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
            try
            {
                var result = _dbFacade.InsertReview(_user, review);
                return result == 1 ? Ok(result) : NoContent();
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return StatusCode(500, ex.Message);
            }
        }
        
        
        [HttpGet("coursedetails")]
        [Authorize(Roles = "Author")]
        public IActionResult GetCourseDetails(int courseId)
        {
            try
            {
                var courseInfo = _dbFacade.GetCourseDetails(courseId);
                var courseReviews = _dbFacade.GetCourseReviews(courseId);
                if (courseInfo != null && courseReviews != null)
                    courseInfo.Review = courseReviews;
                return courseInfo != null ? Ok(courseInfo) : NotFound();
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return StatusCode(500, ex.Message);
            }
        }
        
        
        [HttpPost("addscore")]
        [Authorize(Roles = "User")]
        public IActionResult AddScore(Score score)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            var _user = IdentityMethods.GetCurrentUser(identity);
            score.UserId = _user.Id;
            try
            {
                var result = _dbFacade.AddScore(score);
                return Ok(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("approve")]
        [Authorize(Roles = "Admin")]
        public IActionResult ApproveCourse(int courseId,bool approved)
        {
            try
            {
                return _dbFacade.Approve(courseId, approved) == 1 ? Ok() : NoContent();
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return StatusCode(500, ex.Message);
            }
        }
    }
}