using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UCourseAPI.BusinessLogic;
using UCourseAPI.Methods;
using UCourseAPI.Models;
using WebApi.Data;

namespace UCourseAPITest.BusinessLogicTest
{
    public class InputCheckerTest
    {
        [Fact]
        public void CourseInsertIsValid_EmptyName_ReturnFalse()
        {
            //arrange
            var request = new CourseInsertRequest()
            {
                Name = String.Empty,
                Price = 12,
                Categories = "Business",
                Subcategories = "Management",
                Description = "testtesttesttesttest",
                Language = "Turkish",
                Level = 1
            };
            //act
            var result = InputChecker.CourseInsertIsValid(request,out string message);
            //assert
            Assert.False(result);
        }
        [Fact]
        public void CourseInsertIsValid_LongName_ReturnFalse()
        {
            //arrange
            var request = new CourseInsertRequest()
            {
                Name = GenerateRandomString(250),
                Price = 12,
                Categories = "Business",
                Subcategories = "Management",
                Description = "testtesttesttesttest",
                Language = "Turkish",
                Level = 1
            };
            //act
            var result = InputChecker.CourseInsertIsValid(request, out string message);
            //assert
            Assert.False(result);
        }
        [Fact]
        public void CourseInsertIsValid_NegativePrice_ReturnFalse()
        {
            //arrange
            var request = new CourseInsertRequest()
            {
                Name = "Physics",
                Price = -12,
                Categories = "Business",
                Subcategories = "Management",
                Description = "testtesttesttesttest",
                Language = "Turkish",
                Level = 1
            };
            //act
            var result = InputChecker.CourseInsertIsValid(request, out string message);
            //assert
            Assert.False(result);
        }
        [Fact]
        public void CourseInsertIsValid_DescriptionEmpty_ReturnFalse()
        {
            //arrange
            var request = new CourseInsertRequest()
            {
                Name = "Physics",
                Price = 12,
                Categories = "Business",
                Subcategories = "Management",
                Description = String.Empty,
                Language = "Turkish",
                Level = 1
            };
            //act
            var result = InputChecker.CourseInsertIsValid(request, out string message);
            //assert
            Assert.False(result);
        }
        [Fact]
        public void CourseInsertIsValid_LevelOutOfRange_ReturnFalse()
        {
            //arrange
            var request = new CourseInsertRequest()
            {
                Name = "Physics",
                Price = 12,
                Categories = "Business",
                Subcategories = "Management",
                Description = "tetetetetettetetetet",
                Language = "Turkish",
                Level = 123
            };
            //act
            var result = InputChecker.CourseInsertIsValid(request, out string message);
            //assert
            Assert.False(result);
        }
        [Fact]
        public void CourseInsertIsValid_ReturnTrue()
        {
            //arrange
            var request = new CourseInsertRequest()
            {
                Name = "Physics",
                Price = 12,
                Categories = "Business",
                Subcategories = "Management",
                Description = "tetetetetettetetetet",
                Language = "Turkish",
                Level = 3
            };
            //act
            var result = InputChecker.CourseInsertIsValid(request, out string message);
            //assert
            Assert.True(result);
        }
        static string GenerateRandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            StringBuilder randomStringBuilder = new StringBuilder(length);
            Random random = new Random();

            for (int i = 0; i < length; i++)
            {
                int index = random.Next(chars.Length);
                randomStringBuilder.Append(chars[index]);
            }

            return randomStringBuilder.ToString();
        }

        [Fact]
        public void CourseUpdateIsValid_Emptyname_returnFalse()
        {
            // arrange
            int userId = 2;
            var request = new CourseUpdateRequest()
            {
                Id = 3,
                Name = String.Empty,
                Price = 32,
                Categories = "Business",
                Subcategories = "Management",
                Description = "fggsdgdg",
                Language = "Turkish",
                Level= 3
            };
            //act
            var result = InputChecker.CourseUpdateIsValid(request, userId, out string message);
            //assert
            Assert.False(result);
        }
        [Fact]
        public void CourseUpdateIsValid_CourseNotBelongToAuthor_returnFalse()
        {
            // arrange
            int userId = 1;
            var request = new CourseUpdateRequest()
            {
                Id = 3,
                Name = "Felsefe Aristo",
                Price = 32,
                Categories = "Business",
                Subcategories = "Management",
                Description = "fggsdgdg",
                Language = "Turkish",
                Level = 3
            };
            //act
            var result = InputChecker.CourseUpdateIsValid(request, userId, out string message);
            //assert
            Assert.False(result);
        }
        [Fact]
        public void CourseUpdateIsValid_PriceNegative_returnFalse()
        {
            // arrange
            int userId = 2;
            var request = new CourseUpdateRequest()
            {
                Id = 3,
                Name = "Felsefe Aristo",
                Price = -32,
                Categories = "Business",
                Subcategories = "Management",
                Description = "fggsdgdg",
                Language = "Turkish",
                Level = 3
            };
            //act
            var result = InputChecker.CourseUpdateIsValid(request, userId, out string message);
            //assert
            Assert.False(result);
        }
        [Fact]
        public void CourseUpdateIsValid_DescriptionEmpty_returnFalse()
        {
            // arrange
            int userId = 2;
            var request = new CourseUpdateRequest()
            {
                Id = 3,
                Name = "Felsefe Aristo",
                Price = 32,
                Categories = "Business",
                Subcategories = "Management",
                Description = String.Empty,
                Language = "Turkish",
                Level = 3
            };
            //act
            var result = InputChecker.CourseUpdateIsValid(request, userId, out string message);
            //assert
            Assert.False(result);
        }
        [Fact]
        public void CourseUpdateIsValid_LevelOutOfRange_returnFalse()
        {
            // arrange
            int userId = 2;
            var request = new CourseUpdateRequest()
            {
                Id = 3,
                Name = "Felsefe Aristo",
                Price = 32,
                Categories = "Business",
                Subcategories = "Management",
                Description = "DDDDFFDFDS",
                Language = "Turkish",
                Level = 10
            };
            //act
            var result = InputChecker.CourseUpdateIsValid(request, userId, out string message);
            //assert
            Assert.False(result);
        }
        [Fact]
        public void CourseUpdateIsValid_AuthorHaveSameNameCourse_returnFalse()
        {
            // arrange
            int userId = 2;
            var request = new CourseUpdateRequest()
            {
                Id = 1002,
                Name = "IT for Business",
                Price = 32,
                Categories = "Business",
                Subcategories = "Management",
                Description = "asfasfasf",
                Language = "Turkish",
                Level = 3
            };
            //act
            var result = InputChecker.CourseUpdateIsValid(request, userId, out string message);
            //assert
            Assert.False(result);
        }
        [Fact]
        public void CourseUpdateIsValid_ThereIsNotThisUser_returnFalse()
        {
            // arrange
            int userId = 2341;
            var request = new CourseUpdateRequest()
            {
                Id = 3,
                Name = "Felsefe Aristo",
                Price = 32,
                Categories = "Business",
                Subcategories = "Management",
                Description = "dsfasfas",
                Language = "Turkish",
                Level = 3
            };
            //act
            var result = InputChecker.CourseUpdateIsValid(request, userId, out string message);
            //assert
            Assert.False(result);
        }
        [Fact]
        public void CourseUpdateIsValid_ThereIsNotThisCourse_returnFalse()
        {
            // arrange
            int userId = 2;
            var request = new CourseUpdateRequest()
            {
                Id = 334,
                Name = "Felsefe Aristo",
                Price = 32,
                Categories = "Business",
                Subcategories = "Management",
                Description = String.Empty,
                Language = "Turkish",
                Level = 3
            };
            //act
            var result = InputChecker.CourseUpdateIsValid(request, userId, out string message);
            //assert
            Assert.False(result);
        }
        [Fact]
        public void CourseUpdateIsValid_returnTrue()
        {
            // arrange
            int userId = 2;
            var request = new CourseUpdateRequest()
            {
                Id = 3,
                Name = "Aristo for Students",
                Price = 42,
                Categories = "Design",
                Subcategories = "Illustration",
                Description = "sdsdsds",
                Language = "Turkish",
                Level = 3
            };
            //act
            var result = InputChecker.CourseUpdateIsValid(request, userId, out string message);
            //assert
            Assert.True(result);
        }
    }
}
