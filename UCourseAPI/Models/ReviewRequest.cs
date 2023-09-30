namespace UCourseAPI.Models
{
    public class ReviewRequest
    {
        public int CourseId { get; set; }
        public string? ReviewText { get; set; }
        public int Score { get; set; }
    }
}
