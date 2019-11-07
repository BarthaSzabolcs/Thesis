using DataAcces.Infrastructure;
using System.Collections.Generic;
using System.Data;

namespace DataAcces.Mapping
{
    public abstract class ResourceMapper<T>
    {
        protected IConnectionFactory connectionFactory;

        public ResourceMapper(IConnectionFactory connectionFactory)
        {
            this.connectionFactory = connectionFactory;
        }

        public abstract IEnumerable<T> MappedQuery(string sql, object param = null, IDbTransaction transaction = null);
    }
}
