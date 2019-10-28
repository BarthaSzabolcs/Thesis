using System.Collections.Generic;
using DapperExtensions;
using DataAcces.Infrastructure;
using DataModels;

namespace DataAcces.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class, IDataModel
    {
        private IConnectionFactory connectionFactory;

        public GenericRepository(IConnectionFactory connectionFactory)
        {
            this.connectionFactory = connectionFactory;
        }

        public virtual T Get(int id)
        {
            using (var con = connectionFactory.Connection)
            {
                con.Open();

                return con.Get<T>(id);
            }
        }
        public virtual IEnumerable<T> GetAll()
        {
            using (var con = connectionFactory.Connection)
            {
                return con.GetList<T>();
            }
        }

        public virtual T Add(T item)
        {
            using (var con = connectionFactory.Connection)
            {
                con.Open();

                var id  =  con.Insert(item);
                item.Id = id;

                return item;
            }
        }
        public virtual bool Update(T item)
        {
            using (var con = connectionFactory.Connection)
            {
                con.Open();

                return con.Update(item);
            }
        }

        public virtual bool Delete(int id)
        {
            using (var con = connectionFactory.Connection)
            {
                con.Open();

                var predicate = Predicates.Field<T>(item => item.Id, Operator.Eq, true);
                return con.Delete<T>(predicate);
            }
        }
    }
}
