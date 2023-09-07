using Azure.Core;
using System.Text.RegularExpressions;
using UCourseAPI.Data;
using UCourseAPI.Models;

namespace UCourseAPI.BusinessLogic
{
    public static class InputChecker
    {
        public static bool CourseInsertIsValid(CourseInsertRequest insertRequest ,out string errormessage)
        {
            if (String.IsNullOrEmpty(insertRequest.Name))
            {
                errormessage = "Course name can not be empty! ";
                return false;
            }
            else if (String.IsNullOrEmpty(insertRequest.Description))
            {
                errormessage = "Course name can not be empty! ";
                return false;
            }
            else if (insertRequest.Name.Length > 200)
            {
                errormessage = "Course name couldn't be longer than 200 letters!";
                return false;
            }
            else if(insertRequest.Price < 0)
            {
                errormessage = "Price can not be negatif!";
                return false;
            }
            else if (!new List<int>() {1,2,3}.Contains(insertRequest.Level))
            {
                errormessage = "The level is out of range!";
                return false;
            }
            else
            {
                errormessage = "";
                return true;
            }
        }
        public static bool CourseUpdateIsValid(CourseUpdateRequest request ,int userId, out string errormessage)
        {
            if (request.Name.Length > 200)
            {
                errormessage = "Course name couldn't be longer than 200 letters!";
                return false;
            }
            else if (request.Price < 0)
            {
                errormessage = "Price can not be negatif!";
                return false;
            }
            else if (!new List<int>() { 1, 2, 3 }.Contains(request.Level))
            {
                errormessage = "The level is out of range!";
                return false;
            }
            else if (!IsCourseBelongToAuthor(request.Id, userId))
            {
                errormessage = "Author can not update the course that doesn't belong to him!";
                return false;
            }
            else if (IsAuthorHasCourseSameName(request.Name, userId))
            {
                errormessage = String.Concat("There is already a course named ", request.Name);
                return false;
            }
            else
            {
                errormessage = "";
                return true;
            }

        }
        public static bool CourseDeleteIsValid(int CourseId, int userId, out string errormessage)
        {
            if(!IsCourseBelongToAuthor(CourseId, userId))
            {
                errormessage = "Auhor can not delete the course doesn't belong to him.";
                return false;
            }
            else
            {
                errormessage = "";
                return true;
            }
        }
        public static bool IsAlreadyPurchased(int UserId,int CourseId,out string errormessage)
        {
            var result = new DBFacade().IsAlreadyPurchased(UserId, CourseId);
            if (result)
            {
                errormessage = "This course already purchased!";
            }
            errormessage = "";
            return result;
        }
        public static bool IsCourseBelongToAuthor(int courseId ,int userId)
        {
            var dmf = new DBFacade();
            return dmf.IsCourseBelongToAuthor(courseId, userId);
        }
        public static bool IsAuthorHasCourseSameName(string courseName , int userId)
        {
            var dmf = new DBFacade();
            return dmf.IsAuthorHasCourseSameName(courseName, userId);
        }
        public static bool IsUserRegisterInputValid(UserRegister user , out string errormessage)
        {
            if (String.IsNullOrEmpty(user.Email))
            {
                errormessage = "Email cannot be empty!";
                return false;
            }
            else if (String.IsNullOrEmpty(user.Name))
            {
                errormessage = "Email cannot be empty!";
                return false;
            }
            else if (String.IsNullOrEmpty(user.Password))
            {
                errormessage = "Password cannot be empty !";
                return false;
            }
            else if (!IsStrongPassword(user.Password))
            {
                errormessage = @"
                                Use a minimum of 8 characters.
                                Include at least one uppercase letter (A-Z).
                                Include at least one lowercase letter (a-z).
                                Include at least one digit (0-9).
                                Consider using special characters, such as !, @, #, $, etc., for added security.
                                Avoid using easily guessable information like names, birthdays, or common words.
                                Do not use common patterns like 'password123' or '12345678'. ";
                return false;
            }
            else if(user.Role == "Author" && String.IsNullOrEmpty(user.Description))
            {
                errormessage = "Author must have a description!";
                return false;
            }
            else if (user.Name.Length > 100)
            {
                errormessage = "Name cannot be longer than 100 characters !";
                return false;
            }
            else
            {
                errormessage = String.Empty;
                return true;
            }
           
        }
        public static bool IsPasswordValid(string Password,out string errormessage)
        {
            if (String.IsNullOrEmpty(Password))
            {
                errormessage = "Password cannot be empty !";
                return false;
            }
            else if (!IsStrongPassword(Password))
            {
                errormessage = @"
                                Use a minimum of 8 characters.
                                Include at least one uppercase letter (A-Z).
                                Include at least one lowercase letter (a-z).
                                Include at least one digit (0-9).
                                Consider using special characters, such as !, @, #, $, etc., for added security.
                                Avoid using easily guessable information like names, birthdays, or common words.
                                Do not use common patterns like 'password123' or '12345678'. ";
                return false;
            }
            else
            {
                errormessage = String.Empty;
                return true;
            }
        }
        public static bool IsUserInfoUpdateValid(UserInfoRequest userInfo ,string userRole, out string errormessage)
        {
            if (String.IsNullOrEmpty(userInfo.Email))
            {
                errormessage = "Email cannot be empty!";
                return false;
            }
            else if (String.IsNullOrEmpty(userInfo.Name))
            {
                errormessage = "Email cannot be empty!";
                return false;
            }
            
            else if (userRole == "Author" && String.IsNullOrEmpty(userInfo.Description))
            {
                errormessage = "Author must have a description!";
                return false;
            }
            else if (userInfo.Name.Length > 100)
            {
                errormessage = "Name cannot be longer than 100 characters !";
                return false;
            }
            else
            {
                errormessage = String.Empty;
                return true;
            }
        }
        public static bool IsStrongPassword(string password)
        {
            string pattern = @"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d).{8,}$";
            Regex regex = new Regex(pattern);
            return regex.IsMatch(password);
        }
    }
}
