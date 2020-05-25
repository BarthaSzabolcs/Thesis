using DataModels;
using DataAcces.Infrastructure;
using System;
using System.Collections.Generic;

namespace DataAcces.Repositories
{
    public abstract class CompositeRepository<TResource, TModel> : IRepository<TResource>
        where TResource : class, IDataModel
        where TModel : class, IDataModel, new()
    {
        protected IConnectionFactory connectionFactory;

        public CompositeRepository(IConnectionFactory connectionFactory)
        {
            this.connectionFactory = connectionFactory;
        }

        public abstract TResource Get(int id);
        public abstract IEnumerable<TResource> GetAll();

        public virtual TResource Add(TResource resource)
        {
            throw new NotImplementedException();
        }
        public virtual bool Update(TResource resource)
        {
            throw new NotImplementedException();
        }
        public virtual bool Delete(int id)
        {
            var repo = new SimpleRepository<TModel>(connectionFactory);

            return repo.Delete(id);
        }
    }
}