using UCourseAPI.Models;

namespace UCourseAPI.Repositories
{
    public interface ICourseRepo
    {
        List<CourseResponse> GetAll();
    }
}
