namespace UCourseAPI.Models
{
    public class UserPasswordRequest
    {
        
        public string Password { get; set; }
        public string PasswordAgain { get; set; }
        public string NewPassword { get; set; }
    }
}
