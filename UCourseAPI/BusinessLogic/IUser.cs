using UCourseAPI.Models;

namespace UCourseAPI.BusinessLogic
{
    public interface IUser
    {
        public int Id { get; set; }
        public string Email { get; set; }

        public List<CourseResponse> GetCourses();
        
    }
}
