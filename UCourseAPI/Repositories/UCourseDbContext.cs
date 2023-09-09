using System.Data;
using System.Data.SqlClient;

namespace UCourseAPI.Repositories
{
    public class UCourseDbContext
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionstring;

        public UCourseDbContext(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionstring = _configuration.GetConnectionString("Test");
        }
        public IDbConnection CreateConnection()
        {
            return new SqlConnection(_connectionstring);
        }
    }
}
