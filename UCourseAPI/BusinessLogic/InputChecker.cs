using UCourseAPI.Models;

namespace UCourseAPI.BusinessLogic
{
    public class InputChecker
    {
        public bool CourseInsertIsValid(CourseInsertRequest insertRequest ,out string errormessage)
        {
            if (insertRequest.Name.Length > 200)
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
    }
}
