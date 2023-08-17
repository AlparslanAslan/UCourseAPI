namespace UCourseAPI.Models
{
    public class Course
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Categories { get; set; }
        public string Subcategories { get; set; }
        public string Level { get; set; }
        public int Duration { get; set; }
        public string Description { get; set; }
        public string Language { get; set; }
        public DateTime Date { get; set; }
    }
}