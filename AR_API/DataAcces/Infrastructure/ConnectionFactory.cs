using DapperExtensions.Sql;
using MySql.Data.MySqlClient;
using System.Data;

namespace DataAcces.Infrastructure
{
    public class MySqlConnectionFactory : IConnectionFactory
    {
        public MySqlConnectionFactory()
        {
            DapperExtensions.DapperExtensions.SqlDialect = new DapperExtensions.Sql.MySqlDialect();
        }

        public IDbConnection Connection
        {
            get
            {
                const string connectionString = "datasource=127.0.0.1; port=3306; username=root; password=;database=RecognizedObjectInfo";
                return new MySqlConnection(connectionString);
            }
        }
    }
}
