using UCourseAPI.Data;
using UCourseAPI.Models;

namespace UCourseAPI.BusinessLogic
{
    public static class InputChecker
    {
        public static bool CourseInsertIsValid(CourseInsertRequest insertRequest ,out string errormessage)
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
        public static bool CourseUpdateIsValid(CourseUpdateRequest request , out string errormessage)
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
    }
}
