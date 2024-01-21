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
        public CourseController(ILogger<CourseController> logger, DBFacade dbFacade, ICourseRepo courseRepo)
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
        public IActionResult GetCourses(string? name, string? category, string? language, string? subcategory, int level)
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
        [Authorize(Roles = "Author")]
        public async Task<IActionResult> InsertCourseAsync([FromForm] CourseInsertRequest course)
        {
            // VALIDATION CHECK
            if (!InputChecker.CourseInsertIsValid(course, out string errortext))
            {
                return BadRequest(errortext);
            }
            
            //CREATE INSTANCE
            var _course = new CourseDbParameters()
            {
                Name = course.Name,
                Categories = course.Categories,
                Subcategories = course.Subcategories,
                Description = course.Description,
                Language = course.Language,
                Level = course.Level,
                Price = course.Price,
                AuthorId = IdentityMethods.GetCurrentUser(HttpContext).Id,
                Document = IdentityMethods.GetFileBytes(course.Files).Result
            };

            //INSERT TO DB
            return _dbFacade.InsertCourse(_course) == 1 ? Ok() : NoContent();
        }


        [HttpPost("updatecourse")]
        [Authorize(Roles = "Author")]
        public IActionResult UpdateCourse(CourseUpdateRequest course)
        {
            var user = IdentityMethods.GetCurrentUser(HttpContext);

            if (!InputChecker.CourseUpdateIsValid(course, user.Id, out string errortext))
            {
                return BadRequest(errortext);
            }
            return _dbFacade.UpdateCourse(course) == 1 ? Ok() : NoContent();

        }

        [HttpPost("addreview")]
        //[Authorize(Roles = "Author")]
        public IActionResult AddReview(ReviewRequest review)
        {

            var user = IdentityMethods.GetCurrentUser(HttpContext);
            var _review = new ScoreReview()
            {
                CourseId = review.CourseId,
                ReviewText = review.ReviewText,
                UserId = user.Id,
                Id = 0,
                Score = review.Score
            };

            return _dbFacade.AddReview(_review) == 1 ? Ok() : NoContent();

        }


        [HttpDelete("deletecourse")]
        [Authorize(Roles = "Author")]
        public IActionResult DeleteCourse(int courseid)
        {

            var user = IdentityMethods.GetCurrentUser(HttpContext);
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
            var user = IdentityMethods.GetCurrentUser(HttpContext);

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

            var user = IdentityMethods.GetCurrentPerson(HttpContext);
            var result = user.GetCourses();
            return result.IsNullOrEmpty() ? NoContent() : Ok(result);
        }

        [HttpGet("document")]
        [Authorize(Roles = "User")]
        public IActionResult GetUserCourseDocument(int courseId)
        {
            var user = IdentityMethods.GetCurrentPerson(HttpContext);
            
            List<Document> documents = new List<Document>();
            
            if (InputChecker.IsAlreadyPurchased(user.Id, courseId, out string message))
                documents = _dbFacade.GetCourseDocuments(courseId);
            
            return documents.IsNullOrEmpty() == true ? NotFound() : Ok(documents);
        }


        [HttpPost("insertreview")]
        [Authorize(Roles = "User")]
        public IActionResult InsertReview(ReviewInsertRequest request)
        {

            var _user = IdentityMethods.GetCurrentUser(HttpContext);
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

            var _user = IdentityMethods.GetCurrentUser(HttpContext);
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

        //[HttpPost("approve")]
        //[Authorize(Roles = "Admin")]
        //public IActionResult ApproveCourse(int courseId, bool approved)
        //{
        //    try
        //    {
        //        return _dbFacade.Approve(courseId, approved) == 1 ? Ok() : NoContent();
        //    }
        //    catch (Exception ex)
        //    {
        //        Log.Error(ex.Message);
        //        return StatusCode(500, ex.Message);
        //    }
        //}

        [HttpGet("getreviews")]
        // [Authorize(Roles = "Admin")]
        public IActionResult GetReviews(int courseId)
        {
            try
            {
                return _dbFacade.GetReviews(courseId) != null ? Ok(_dbFacade.GetReviews(courseId)) : NoContent();
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("approve")]
        public IActionResult Approve(Approve approve)
        {
            try
            {
                return _dbFacade.Approve(approve.CourseId, approve.Approved) == 1 ? Ok() : NotFound();

            }
            catch (Exception ex)
            {

                System.Diagnostics.Trace.WriteLine(ex.Message);
                return BadRequest(ex.Message);
            }

        }

        [HttpGet("inapproval")]
        public IActionResult InApproval()
        {
            try
            {
                return Ok(_dbFacade.InApproval());

            }
            catch (Exception ex)
            {

                System.Diagnostics.Trace.WriteLine(ex.Message);
                return BadRequest(ex.Message);
            }

        }

        [HttpPost("rolechange")]
        [Authorize(Roles ="Admin")]
        
        public IActionResult RoleChange(RoleChangeRequest request)
        {
            try
            {
                _dbFacade.ChangeRole(request);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
               
            }
        }

        [HttpGet("getuserrole")]
        [Authorize(Roles ="Admin")]
        public IActionResult GetUserRole()
        {
           var result =  _dbFacade.GetUserRoles();
            return Ok(result);
        }
    }
}