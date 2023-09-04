namespace UCourseAPI.Models
{
    public class Review
    {
        public int Id{ get; set; }
        public string ReviewText { get; set; }
        public int CourseId { get; set; }
        public int UserId { get; set; }
    }
}
