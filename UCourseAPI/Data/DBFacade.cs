using UCourseAPI.Models;
using WebApi.Data;

namespace UCourseAPI.Data
{
    public class DBFacade 
    {
        //private readonly IConfiguration _configuration;
        private  readonly string _connectionstring;
        
        public DBFacade(/*IConfiguration configuration*/)
        {
            _connectionstring =   ConfigManager.GetConnectionString();
            //_configuration = configuration;
            //_connectionstring = _configuration.GetConnectionString("Test");
        }

        public List<CourseResponse> GetAllCourses(string? name, string? category, string? language, string? subcategory, int level)
        {
            var bdm = new DBConnection();
            return bdm.GetAllCourses(_connectionstring, name,category,language,subcategory,level);
        }
        public int InsertCourse( CourseDbParameters course)
        {
            var dbm = new DBConnection();
            return dbm.InsertCourse(_connectionstring, course);

        }
        public int UpdateCourse( CourseUpdateRequest course)
        {
            var dbm = new DBConnection();   
            return dbm.UpdateCourse(_connectionstring, course);

        }
        public int DeleteCourse( int id)
        {
            var dbm = new DBConnection();
            return dbm.DeleteCourse(_connectionstring, id);

        }
        public int UserRegister( User user)
        {
            var dbm = new IdentityData();
            return dbm.UserRegister(_connectionstring, user);

        }
        public int IsUserExist(LoginUser user)
        {
            var dbm = new IdentityData();
            return dbm.IsUserExist(_connectionstring, user);
        }
        public User GetUserInfo( string email)
        {
            var dbm = new IdentityData();
            return dbm.GetUserInfo(_connectionstring,email);
        }
        public int PurchaseCourse(int courseId, User user)
        {
            var dbm = new DBConnection();
            return dbm.PurchaseCourse(_connectionstring, courseId, user);
        }
        public IEnumerable<CourseResponse> GetUserCourseList( User user)
        {
            var dbm = new DBConnection();
            return dbm.GetUserCourseList(_connectionstring, user);
        }
        public IEnumerable<CourseResponse> GetAuthorCourses( User user)
        {
            var dbm = new DBConnection();
            return dbm.GetAuthorCourses(_connectionstring, user);
        }
        public int UpdateUserInfo( User oldUser, User newUser)
        {
            var dbm = new IdentityData();
            return dbm.UpdateUserInfo(_connectionstring, oldUser, newUser);
        }
        public int UpdateUserPassword(User user)
        {
            var dbm = new IdentityData();
            return dbm.UpdateUserPassword(_connectionstring, user);
        }
        public int InsertReview( User user, Review review)
        {
            var dbm = new DBConnection();
            return dbm.InsertReview(_connectionstring,review);
        }
        public CourseDetails GetCourseDetails(int courseId)
        {
            var dbm = new DBConnection();
            return dbm.GetCourseDetails(_connectionstring, courseId);
        }
        public int AddScore(Score score)
        {
            var dbm = new DBConnection();   
           return dbm.AddScore(_connectionstring, score);
        }
        public bool IsAlreadyPurchased( int UserId,int CourseId)
        {
            var dbm = new DBConnection();
            return dbm.IsAlreadyPurchased(_connectionstring, UserId, CourseId);
        }

        public List<string> GetCourseReviews(int courseId)
        {
            var dbm = new DBConnection();
            return dbm.GetCourseReviews(_connectionstring, courseId);
        }
    }
}
