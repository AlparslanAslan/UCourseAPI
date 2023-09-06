using UCourseAPI.Data;
using UCourseAPI.Models;

namespace UCourseAPI.BusinessLogic
{
    public class User : IUser
    {
        public int Id { get ; set; }
        public string Email { get; set; }

        public List<CourseResponse> GetCourses()
        {
            var dbm = new DBFacade();
            return dbm.GetUserCourseList(Id).ToList();
        }
    }
}
