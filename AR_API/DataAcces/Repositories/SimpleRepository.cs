using System.Collections.Generic;
using DapperExtensions;
using DataModels;
using DataAcces.Infrastructure;

namespace DataAcces.Repositories
{
    public class SimpleRepository<TModel> : IRepository<TModel> where TModel : class, IDataModel, new()
    {
        protected IConnectionFactory connectionFactory;

        public SimpleRepository(IConnectionFactory connectionFactory)
        {
            this.connectionFactory = connectionFactory;
        }

        public virtual TModel Get(int id)
        {
            using (var con = connectionFactory.Connection)
            {
                return con.Get<TModel>(id);
            }
        }

        public virtual IEnumerable<TModel> GetAll()
        {
            using (var con = connectionFactory.Connection)
            {
                return con.GetList<TModel>();
            }
        }
        public virtual IEnumerable<TModel> GetAll(IFieldPredicate predicate)
        {
            using (var con = connectionFactory.Connection)
            {
                return con.GetList<TModel>(predicate);
            }
        }

        public virtual TModel Add(TModel item)
        {
            using (var con = connectionFactory.Connection)
            {
                var id = con.Insert(item);
                item.Id = id;

                return item;
            }
        }

        public virtual bool Update(TModel item)
        {
            using (var con = connectionFactory.Connection)
            {
                return con.Update(item);
            }
        }

        public virtual bool Delete(int id)
        {
            using (var con = connectionFactory.Connection)
            {
                var predicate = Predicates.Field<TModel>(r => r.Id, Operator.Eq, id);

                return con.Delete<TModel>(predicate);
            }
        }
        public virtual bool Delete(IFieldPredicate predicate)
        {
            using (var con = connectionFactory.Connection)
            {
                return con.Delete<TModel>(predicate);
            }
        }
    }
}
