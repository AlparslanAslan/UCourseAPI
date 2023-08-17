using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using UCourseAPI.Models;
using Dapper;

namespace UCourseAPI.Data;
public class DBConnection
{
    private string connectionString = "Server=LAPTOP-D8QC5NMV;Database=test;Integrated Security=True;";

    
    public List<Course> GetAllCourses(string? name, string? category,string? language, string? subcategory, int level, int orderby)
    {
        using (IDbConnection dbConnection = new SqlConnection(connectionString))
        {
            var parameters = new { name ='%'+name+'%', category,language, subcategory,level, orderby };
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
             c.duration,c.description,p3.explanation language, c.date
             
             from course c
             left join parameters p1 on p1.name='categories' and p1.parno=categories
             left join parameters p2 on p2.name='subcategories' and p2.parno=subcategories
             left join parameters p3 on p3.name='language' and p3.parno=language
             where 
             c.name like isnull(@name,c.name) and
             categories= isnull(nullif(@categoriesnumeric,0),categories)  and
             subcategories=isnull(nullif(@subcategoriesnumeric,0),subcategories) and
             level=isnull(nullif(@level,0),level) and
             language = isnull(nullif(@languagenumeric,0),language)
            ";
            if(orderby != 0)
            {
                query += "order by " + orderby;
            }
            List<Course> courses = dbConnection.Query<Course>(query,parameters).AsList();
            return courses;
        }
    }
    public int InsertCourse(CourseInsertRequest course)
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
            insert into course values(@name,@price,@categoriesnumeric,@subcategoriesnumeric,@level,null,@description,@languagenumeric,GETDATE())";
            
            return dbConnection.Execute(query, course);
        }
    }

    public int UpdateCourse(CourseUpdateRequest course)
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

    public int DeleteCourse(int id)
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

}
