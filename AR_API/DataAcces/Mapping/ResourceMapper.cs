using DataAcces.Infrastructure;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
