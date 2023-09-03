using UCourseAPI.Models;

namespace UCourseAPI.Data
{
    public class DBFacade
    {
        private readonly IConfiguration _configuration;
        private  readonly string _connectionstring;

        public DBFacade(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionstring = _configuration.GetConnectionString("Test");
        }

        public List<Course> GetAllCourses(string? name, string? category, string? language, string? subcategory, int level, int orderby)
        {
            var bdm = new DBConnection();
            return bdm.GetAllCourses(_connectionstring, name,category,language,subcategory,level,orderby);
        }
        public int InsertCourse( CourseInsertRequest course)
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
        public IEnumerable<Course> GetUserCourseList( User user)
        {
            var dbm = new DBConnection();
            return dbm.GetUserCourseList(_connectionstring, user);
        }
        public IEnumerable<Course> GetAuthorCourses( User user)
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
        public List<string> GetCourseDetails(int courseId)
        {
            var dbm = new DBConnection();
            return dbm.GetCourseDetails(_connectionstring, courseId);
        }
        public int AddScore(Score score)
        {
            var dbm = new DBConnection();   
           return dbm.AddScore(_connectionstring, score);
        }

    }
}
