﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using UCourseAPI.Models;
using Dapper;
using System.Data.Common;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using WebApi.Data;

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
             c.description,p3.explanation language, c.date,s.avgscore Score, pa.name author
             
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
             language = isnull(nullif(@languagenumeric,0),language)
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
            
            ";
            return dbConnection.Execute(query, course);
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
    public int PurchaseCourse(string connectionString, int courseId , User user)
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
    public IEnumerable<CourseResponse> GetUserCourseList(string connectionString, User user)
    {
        using (IDbConnection dbConnection = new SqlConnection(connectionString))
        {
            string query = @"

            
            select c.id,c.name,a.price,c.description,p1.explanation categories , p2.explanation subcategories, p3.explanation language
            ,case c.level when 1 then 'Begginer' when 2 then 'Intermediate' when 3 then 'Advanced' end level
            from acquisition a
            left join course c on c.id= a.courseId 
             left join parameters p1 on p1.name='categories' and p1.parno=c.categories
             left join parameters p2 on p2.name='subcategories' and p2.parno=c.subcategories
             left join parameters p3 on p3.name='language' and p3.parno=c.language
            where userId=@Id
                ";
            return dbConnection.Query<CourseResponse>(query,user);
        }
    }
    public IEnumerable<CourseResponse> GetAuthorCourses(string connectionString, User user)
    {
        using (IDbConnection dbConnection = new SqlConnection(connectionString))
        {
            var query = @"
             
             select  c.id,c.name,author.name author ,c.price,c.description,p1.explanation categories , p2.explanation subcategories, p3.explanation language
             ,case c.level when 1 then 'Begginer' when 2 then 'Intermediate' when 3 then 'Advanced' end level,c.date
             from course c 
             left join parameters p1 on p1.name='categories' and p1.parno=c.categories
             left join parameters p2 on p2.name='subcategories' and p2.parno=c.subcategories
             left join parameters p3 on p3.name='language' and p3.parno=c.language
             left join person author on author.id=c.authorId 
             where authorId=@Id
        
           
            ";
            return dbConnection.Query<CourseResponse>(query, user);
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
}
