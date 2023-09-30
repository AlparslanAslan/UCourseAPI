using Dapper;
using System.Data;
using UCourseAPI.Models;

namespace UCourseAPI.Repositories
{
    public class UCourseRepo : ICourseRepo
    {
        private readonly UCourseDbContext _context;
        public UCourseRepo(UCourseDbContext context)
        {
            this._context = context;
        }

        public List<CourseResponse> GetAll()
        {
            using (IDbConnection dbConnection = _context.CreateConnection())
            {
                string query = @"
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
where c.approved=1
            
            ";
                List<CourseResponse> courses = dbConnection.Query<CourseResponse>(query).AsList();
                return courses;
            }
        }
    }
}
