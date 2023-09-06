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
        public   int UserRegister(string connectionString, UserResponse user)
        {
            using (IDbConnection dbConnection = new SqlConnection(connectionString))
            {
                string query = @"
                 insert into person 
                 (name,email,description,role,passwordhash,passwordsalt,date) values
                (@Name,@Email,@Description,@Role,@PasswordHash,@PasswordSalt,getdate()) ";
                return dbConnection.Execute(query,user);
            }
        }
        public int IsUserExist(string connectionString, LoginUser user)
        {
            using (IDbConnection dbConnection = new SqlConnection(connectionString))
            {
                string query = @"select 1 from person where email=@Email ";
                return dbConnection.QueryFirstOrDefault<int>(query, user);
            }
        } 
        public  UserResponse GetUserInfo(string connectionString, string email)
        {
            var parameters = new { email };
            using (IDbConnection dbConnection = new SqlConnection(connectionString))
            {
                string query = @"select id,name,passwordhash,passwordsalt,role Role,email Email,description Description from person where email=@email ";
                return dbConnection.QueryFirstOrDefault<UserResponse>(query, parameters);
            }
        }
        public int UpdateUserInfo(string connectionString,UserResponse oldUser, UserResponse NewUser)
        {
            var parameters = new { oldname = oldUser.Name, oldemail=oldUser.Email, 
                newname=NewUser.Name, newemail=NewUser.Email,description=NewUser.Description
            };
            using (IDbConnection dbConnection = new SqlConnection(connectionString))
            {
                var query = @"update person
                set name =@newname, email=@newemail,description=@description
                where name=@oldname and email= @oldemail";

                return dbConnection.Execute(query, parameters );
            }
        }
        public int UpdateUserPassword(string connectionString, UserResponse user)
        {
            using (IDbConnection dbConnection = new SqlConnection(connectionString))
            {
                var query = @"
                update person set passwordhash=@PasswordHash, passwordsalt=@PasswordSalt where email= @Email
                ";
                return dbConnection.Execute(query, user);
            }

        }
    }
}
