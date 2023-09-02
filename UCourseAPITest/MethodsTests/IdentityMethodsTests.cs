using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UCourseAPI.Methods;

namespace UCourseAPITest.MethodsTests
{
    public class IdentityMethodsTests
    {
        [Fact]
        public void CreatePasswordHash_returnhash_password()
        {
            //arrange
            string password = "alp";
            //act
            IdentityMethods.CreatePasswordHash(password,out byte[] passwordHash,out byte[] passwordSalt);
            //assert

            Assert.NotNull(passwordSalt);
        }

    }
}
