using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using UCourseAPI.Models;
using Dapper;
using System.Data.Common;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using WebApi.Data;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using UCourseAPI.BusinessLogic;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using System.Reflection.Metadata;
using Document = UCourseAPI.Models.Document;

namespace UCourseAPI.Data;
public class DBConnection 
{
    
    //private string connectionString = "Server=LAPTOP-D8QC5NMV;Database=test;Integrated Security=True;";  
    public List<CourseResponse> GetAllCourses(string connectionString,string? name, string? category,string? language, string? subcategory, int level) 
    { 
        using (IDbConnection dbConnection = new SqlConnection(connectionString))
        {
            var parameters = new { name ='%'+name+'%', category,language, subcategory,level };
            string query = @"
            declare @categoriesnumeric int;
            declare @subcategoriesnumeric int;
            declare @languagenumeric  int;
             
            set @categoriesnumeric = (select top 1 parno from parameters where name='categories' and explanation=@category)
            set @subcategoriesnumeric =(select top 1 parno from parameters where name='subcategories' and explanation=@subcategory)
            set @languagenumeric = (select top 1 parno from parameters where name='language' and explanation=@language)
            
             select 
             c.id,c.name,c.price,p1.explanation categories,p2.explanation subcategories,
             
             case c.level when 1 then 'Begginer' when 2 then 'Intermediate' when 3 then 'Advanced' end level,
             c.description,p3.explanation language, c.date,s.avgscore Score, pa.name author,c.date
             
             from course c
             left join person pa on pa.id =c.authorId 
             left join parameters p1 on p1.name='categories' and p1.parno=categories
             left join parameters p2 on p2.name='subcategories' and p2.parno=subcategories
             left join parameters p3 on p3.name='language' and p3.parno=language
             left join (select courseId,avg(star) avgscore from star group by courseId) s on s.courseId= c.id
             where 
             c.name like isnull(@name,c.name) and
             categories= isnull(nullif(@categoriesnumeric,0),categories)  and
             subcategories=isnull(nullif(@subcategoriesnumeric,0),subcategories) and
             level=isnull(nullif(@level,0),level) and
             language = isnull(nullif(@languagenumeric,0),language) and
             c.approved=1 
            ";
          
            List<CourseResponse> courses = dbConnection.Query<CourseResponse>(query,parameters).AsList();
            return courses;
        }
    }
    public int InsertCourse( string connectionString, CourseDbParameters course)
    {
        using (IDbConnection dbConnection = new SqlConnection(connectionString))
        {
            string query = @"
             declare @categoriesnumeric int;
             declare @subcategoriesnumeric int;
             declare @languagenumeric int;
             
             set @categoriesnumeric = (select top 1 parno from parameters where name='categories' and explanation=@categories)
             set @subcategoriesnumeric =(select top 1 parno from parameters where name='subcategories' and explanation=@subcategories)
             set @languagenumeric = (select top 1 parno from parameters where name='language' and explanation=@language)
             
             if(@categoriesnumeric is not null or @languagenumeric is not null or @subcategoriesnumeric is not null)
             insert into course (name,authorId,price,categories,subcategories,level,description,language,date)
             values(@name,@authorId,@price,@categoriesnumeric,@subcategoriesnumeric,@level,@description,@languagenumeric,GETDATE())
             insert into document values('PDF',@document,null,1,GETDATE(),1)
            
            ";
            try
            {
                return dbConnection.Execute(query, course);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                
            }
            return 0;
        }
    }
    public int UpdateCourse(string connectionString, CourseUpdateRequest course)
    {
        using (IDbConnection dbConnection = new SqlConnection(connectionString))
        {
            string query = @"

            declare @categoriesnumeric int;
            declare @subcategoriesnumeric int;
            declare @languagenumeric int;
            
            set @categoriesnumeric = (select top 1 parno from parameters where name='categories' and explanation=@categories)
            set @subcategoriesnumeric =(select top 1 parno from parameters where name='subcategories' and explanation=@subcategories)
            set @languagenumeric = (select top 1 parno from parameters where name='language' and explanation=@language)
            
            if(@categoriesnumeric is not null or @languagenumeric is not null or @subcategoriesnumeric is not null)
            update course set
            name=@name,
            price=@price,
            categories=@categoriesnumeric,
            subcategories=@subcategoriesnumeric,
            level=@level,
            description=@description,
            language=@languagenumeric
            where id=@Id
            ";
            return dbConnection.Execute(query, course);
        }

    }
    public int DeleteCourse(string connectionString, int id)
    {
        var parameters = new { id };
        using (IDbConnection dbConnection = new SqlConnection(connectionString))
        {
            string query = @"
            delete from course where id=@id
            ";
            return dbConnection.Execute(query, parameters);
        }
    }
    public int PurchaseCourse(string connectionString, int courseId , UserResponse user)
    {
        using (IDbConnection dbConnection = new SqlConnection(connectionString))
        {
            var parameters = new { courseId, user.Id};
            string query = @"
                        
                declare @price numeric(20,2)
                select @price = price from course where id=@courseId
                
                if(@Id is not null and @courseId is not null and @price is not null )
                insert into acquisition values(@Id,@courseId,@price,GETDATE())
            ";
            return dbConnection.Execute(query, parameters);
        }
    }
    public IEnumerable<CourseResponse> GetUserCourseList(string connectionString, int Id)
    {
        using (IDbConnection dbConnection = new SqlConnection(connectionString))
        {
            var parameters = new {Id};
            string query = @"

            
           select c.id,c.name,a.price,c.description,p1.explanation categories , p2.explanation subcategories, p3.explanation language
            ,case c.level when 1 then 'Begginer' when 2 then 'Intermediate' when 3 then 'Advanced' end level,p.name Author
            ,c.date,star.score
            from acquisition a
            inner  join course c on c.id= a.courseId
			inner join person p on p.id = c.authorId
            LEFT join (select courseId,AVG(star) score from star group by courseId) star on star.courseId= c.id
             left join parameters p1 on p1.name='categories' and p1.parno=c.categories
             left join parameters p2 on p2.name='subcategories' and p2.parno=c.subcategories
             left join parameters p3 on p3.name='language' and p3.parno=c.language
            where userId=@Id
            and c.approved=1
                ";
            return dbConnection.Query<CourseResponse>(query, parameters);
        }
    }
    public IEnumerable<CourseResponse> GetAuthorCourses(string connectionString, int Id)
    {
        var parmeters = new  { Id };
        using (IDbConnection dbConnection = new SqlConnection(connectionString))
        {
            var query = @"
             
             select  c.id,c.name,author.name author ,c.price,c.description,p1.explanation categories , p2.explanation subcategories, p3.explanation language
             ,case c.level when 1 then 'Begginer' when 2 then 'Intermediate' when 3 then 'Advanced' end level,c.date , a.number_of_purchase NumberOfPurchase,
             case c.approved when 1 then 'Approved' when 2 then 'Denied' else 'In Approval' end approved
             from course c 
             left join parameters p1 on p1.name='categories' and p1.parno=c.categories
             left join parameters p2 on p2.name='subcategories' and p2.parno=c.subcategories
             left join parameters p3 on p3.name='language' and p3.parno=c.language
             left join person author on author.id=c.authorId 
             left join (select courseId,COUNT(*) number_of_purchase from acquisition group by courseId) a on a.courseId=c.id
             where authorId=@Id
            ";
            return dbConnection.Query<CourseResponse>(query, parmeters);
        }        
    }
    public int InsertReview(string connectionString,Review review)
    {
        using (IDbConnection dbConnection = new SqlConnection(connectionString))
        {
            
            var query = @"

              insert into review (userId,review,courseId,date)
              values(@userId,@ReviewText,@courseId,GETDATE())
             ";
            return dbConnection.Execute(query, review);
        }
    }
    public CourseDetails GetCourseDetails(string connectionString,int courseId)
    {
        using (IDbConnection dbConnection = new SqlConnection(connectionString))
        {
            var paremeters = new { courseId };
            var query = @"
             select  c.id,c.name,author.name author ,c.price,c.description,p1.explanation categories , p2.explanation subcategories, p3.explanation language
               ,case c.level when 1 then 'Begginer' when 2 then 'Intermediate' when 3 then 'Advanced' end level,c.date
               from course c 
               left join parameters p1 on p1.name='categories' and p1.parno=c.categories
               left join parameters p2 on p2.name='subcategories' and p2.parno=c.subcategories
               left join parameters p3 on p3.name='language' and p3.parno=c.language
               left join person author on author.id=c.authorId 
               where c.id=@CourseId
             ";
            return dbConnection.QueryFirstOrDefault<CourseDetails>(query, paremeters);
        }

    }
    public int AddScore(string connectionString,Score score)
    {
        using (IDbConnection dbConnection = new SqlConnection(connectionString))
        {
            var query = @"
            insert into star (userId,courseId,star)
            values(@userId,@courseId,@star)
             ";
            return dbConnection.Execute(query, score);
        }
    }
    public bool IsAlreadyPurchased(string connectionString, int UserId,int CourseId)
    {
        var parameters = new { UserId, CourseId };
        using (IDbConnection dbConnection= new SqlConnection(connectionString))
        {
            var query = @" select 1 from acquisition where courseId=@CourseId and userId=@UserId";
            return dbConnection.Query<bool>(query, parameters).FirstOrDefault();
        }
    }
    public List<string> GetCourseReviews(string connectionString, int courseId)
    {
        using (IDbConnection dbConnection = new SqlConnection(connectionString))
        {
            var paremeters = new { courseId };
            var query = @"
             select review from review where courseId=@courseId
             ";
            return dbConnection.Query<string>(query, paremeters).AsList();
        }
    }
    public bool IsCourseBelongToAuthor(string connectionString,int CourseId , int UserId)
    {
        using (IDbConnection dbConnection = new SqlConnection(connectionString))
        {
            var parameters = new { CourseId, UserId };
            var query = @"if
            ( @UserId = (select authorId from course where id=@CourseId) )
            select 1
            else select 0";
            return dbConnection.QueryFirstOrDefault<int>(query, parameters) ==1 ?  true : false;
        }
    }
    public bool IsAuthorHasCourseSameName(string connectionstring, string courseName, int userId)
    {
        using (IDbConnection dbConnection = new SqlConnection(connectionstring))
        {
            var parameters = new { courseName, userId };
            var query = @"if
            (select 1 from course where authorId=@userId and name =@courseName )=1
            select 1
            else select 0";
            return dbConnection.QueryFirstOrDefault<int>(query, parameters) == 1 ? true : false;
        }
    }
    public int Approve(string connectionstring, int courseId, bool approved)
    {
        using (IDbConnection dbConnection = new SqlConnection(connectionstring))
        {
            var parameters = new { courseId, approved = approved ? 1 : 2 };
            var query = @"update course set approved=@approved where id=@courseId";
            return dbConnection.Execute(query, parameters);
        }
    }
    public List<Document> GetCoursesById(string connectionstring,int courseId)
    {
        using (IDbConnection dbConnection = new SqlConnection(connectionstring))
        {
            var parameters = new { courseId };
            var query = @"select id,style type,document documentdata,lecture from document where courseId=@courseId";
            return dbConnection.Query<Document>(query, parameters).ToList();
        }
    }

    public CourseResponse GetCourseById(string connectionstring, int courseId)
    {
        using (IDbConnection dbConnection = new SqlConnection(connectionstring))
        {
            var parameters = new { courseId };
            var query = @"select  c.id,c.name,author.name author ,c.price,c.description,p1.explanation categories , p2.explanation subcategories, p3.explanation language
 ,case c.level when 1 then 'Begginer' when 2 then 'Intermediate' when 3 then 'Advanced' end level,c.level levelInt,c.date , a.number_of_purchase
 from course c 
 left join parameters p1 on p1.name='categories' and p1.parno=c.categories
 left join parameters p2 on p2.name='subcategories' and p2.parno=c.subcategories
 left join parameters p3 on p3.name='language' and p3.parno=c.language
 left join person author on author.id=c.authorId 
 left join (select courseId,COUNT(*) number_of_purchase from acquisition group by courseId) a on a.courseId=c.id
 where 
  c.id =@courseId";
            return dbConnection.Query<CourseResponse>(query, parameters).FirstOrDefault();
        }
    }

    public int AddReview(string connectionstring, ScoreReview review)
    {
        using (IDbConnection dbConnection = new SqlConnection(connectionstring))
        {
            var query = @"
                insert into review(userId,review,courseId,date)
                values(@UserId,@ReviewText,@CourseId,GETDATE())

                insert into star(userId, courseId, star)
                values(@UserId, @CourseId, @score);
                ";
            return dbConnection.Execute(query, review);
        }

    }

    internal List<ReviewResponse> GetReviews(string connectionstring, int courseId)
    {
        using (IDbConnection dbConnection = new SqlConnection(connectionstring))
        {
            var parameter = new { courseId };
            var query = @"  select p.name UserName,review,convert(varchar,r.date,103) date from review  r
                        inner join person p on p.id=r.userId where courseId=@courseId";
               
            return dbConnection.Query<ReviewResponse>(query, parameter).ToList();
        }
    }

    internal List<CourseResponse> InApproval(string connectionstring)
    {
        using (IDbConnection dbConnection = new SqlConnection(connectionstring))
        {


            var query = @" select 
 c.id,c.name,c.price,p1.explanation categories,p2.explanation subcategories,
 
 case c.level when 1 then 'Begginer' when 2 then 'Intermediate' when 3 then 'Advanced' end level,
 c.description,p3.explanation language, c.date,s.avgscore Score, pa.name author
 
 from course c
 left join person pa on pa.id =c.authorId 
 left join parameters p1 on p1.name='categories' and p1.parno=categories
 left join parameters p2 on p2.name='subcategories' and p2.parno=subcategories
 left join parameters p3 on p3.name='language' and p3.parno=language
 left join (select courseId,avg(star) avgscore from star group by courseId) s on s.courseId= c.id
 where 
 c.approved=0 or approved is null";
        return dbConnection.Query<CourseResponse>(query).ToList();

        }
    }

    internal int ChangeRole(string connectionstring, RoleChangeRequest request)
    {
        using (IDbConnection dbConnection = new SqlConnection(connectionstring))
        {
            var query = @"
update person set role=@Role where id=@UserId
";
             return dbConnection.Execute(query, request);
        }
    }

    internal List<UserRoleResponse> GetUserRoles(string connectionstring)
    {
        using (IDbConnection dbConnection = new SqlConnection(connectionstring))
        {
            var query = @"
select id  userId,name,email,role from person
";
           return dbConnection.Query<UserRoleResponse>(query).ToList();
        }


    }
}
