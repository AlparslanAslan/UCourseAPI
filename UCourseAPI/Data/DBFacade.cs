using UCourseAPI.Models;

namespace UCourseAPI.Data
{
    public class DBFacade
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionstring;

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
        public int InsertCourse(CourseInsertRequest course)
        {
            var dbm = new DBConnection();
            return dbm.InsertCourse(course);

        }
        public int UpdateCourse(CourseUpdateRequest course)
        {
            var dbm = new DBConnection();   
            return dbm.UpdateCourse(course);

        }
        public int DeleteCourse(int id)
        {
            var dbm = new DBConnection();
            return dbm.DeleteCourse(id);

        }
        public int UserRegister(User user)
        {
            return IdentityData.UserRegister(user);

        }
        public int IsUserExist(DtoUser user)
        {
            return IdentityData.IsUserExist(user);
        }
        public User GetUserInfo(string name)
        {
            return IdentityData.GetUserInfo(name);
        }


    }
}
