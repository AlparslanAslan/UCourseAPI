namespace UCourseAPI.Models
{
    public class CourseInsertRequest
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Categories { get; set; }
        public string Subcategories { get; set; }
        public int Level { get; set; }
        public string Description { get; set; }
        public string Language { get; set; }
    }
}