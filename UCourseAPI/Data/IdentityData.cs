using Dapper;
using Microsoft.AspNetCore.Identity;
using System.Data;
using System.Data.SqlClient;
using UCourseAPI.Models;

namespace UCourseAPI.Data
{
    public static class IdentityData
    {
        private static string connectionString = "Server=LAPTOP-D8QC5NMV;Database=test;Integrated Security=True;";

        public static int UserRegister(User user)
        {
            using (IDbConnection dbConnection = new SqlConnection(connectionString))
            {
                string query = @"insert into person values(@Name,@PasswordHash,@PasswordSalt) ";
                return dbConnection.Execute(query,user);
            }
        }
        public static int IsUserExist(DtoUser user)
        {
            using (IDbConnection dbConnection = new SqlConnection(connectionString))
            {
                string query = @"select 1 from person where name=@Name ";
                return dbConnection.QueryFirstOrDefault<int>(query, user);
            }
        }
        
        public static User GetUserInfo(string name)
        {
            var parameters = new { name };
            using (IDbConnection dbConnection = new SqlConnection(connectionString))
            {
                string query = @"select name,passwordhash,passwordsalt,role Role from person where name=@name ";
                return dbConnection.QueryFirstOrDefault<User>(query, parameters);
            }
        }

    }
}
