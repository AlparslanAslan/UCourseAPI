namespace UCourseAPI.Models
{
    public class Score
    {
        public int Id { get; set; }
        public decimal Star { get; set; }
        public int CourseId { get; set; }
        public int UserId { get; set; }
    }
}
