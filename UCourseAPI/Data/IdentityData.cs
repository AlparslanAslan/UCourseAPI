using Dapper;
using Microsoft.AspNetCore.Identity;
using System.Data;
using System.Data.SqlClient;
using UCourseAPI.Models;

namespace UCourseAPI.Data
{
    public  class IdentityData
    {
        //private   string connectionString = "Server=LAPTOP-D8QC5NMV;Database=test;Integrated Security=True;";

        public   int UserRegister(string connectionString, User user)
        {
            using (IDbConnection dbConnection = new SqlConnection(connectionString))
            {
                string query = @"insert into person (name,email,passwordhash,passwordsalt) values(@Name,@Email,@PasswordHash,@PasswordSalt) ";
                return dbConnection.Execute(query,user);
            }
        }
        public   int IsUserExist(string connectionString, DtoUser user)
        {
            using (IDbConnection dbConnection = new SqlConnection(connectionString))
            {
                string query = @"select 1 from person where name=@Name ";
                return dbConnection.QueryFirstOrDefault<int>(query, user);
            }
        } 
        public   User GetUserInfo(string connectionString, string name,string email)
        {
            var parameters = new { name,email };
            using (IDbConnection dbConnection = new SqlConnection(connectionString))
            {
                string query = @"select id,name,passwordhash,passwordsalt,role Role,email Email from person where name=@name and email=@email ";
                return dbConnection.QueryFirstOrDefault<User>(query, parameters);
            }
        }
        public int UpdateUserInfo(string connectionString,User oldUser, User NewUser)
        {
            var parameters = new { oldname = oldUser.Name, oldemail=oldUser.Email, 
                newname=NewUser.Name, newemail=NewUser.Email,description=NewUser.Desciption
            };
            using (IDbConnection dbConnection = new SqlConnection(connectionString))
            {
                var query = @"update person
                set name =@newname, email=@newemail,description=@description
                where name=@oldname and email= @oldemail";

                return dbConnection.Execute(query, parameters );
            }
        }
        public int UpdateUserPassword(string connectionString, User user)
        {
            using (IDbConnection dbConnection = new SqlConnection(connectionString))
            {
                var query = @"
                update person set passwordhash=@PasswordHash, passwordsalt=@PasswordSalt where name=@Name and email= @Email
                ";
                return dbConnection.Execute(query, user);
            }

        }
    }
}
