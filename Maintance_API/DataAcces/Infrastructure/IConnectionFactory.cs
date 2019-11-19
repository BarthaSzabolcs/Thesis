using System.Data;

namespace DataAcces.Infrastructure
{
    public interface IConnectionFactory
    {
        IDbConnection Connection { get; }
    }
}
