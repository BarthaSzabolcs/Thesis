using DataAcces.Infrastructure;
using DataAcces.Repositories;
using DataModels;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace Maintance_API.Controllers
{
    public abstract class GenericController<T> : ApiController where T : class, IDataModel, new()
    {
        private IConnectionFactory connectionFactory = new MySqlConnectionFactory();

        public IEnumerable<T> Get()
        {
            var repo = new SimpleRepository<T>(connectionFactory);

            return repo.GetAll();
        }

        public T Get(int id)
        {
            var repo = new SimpleRepository<T>(connectionFactory);

            var item = repo.Get(id);

            if (item != null)
            {
                return item;
            }
            else
            {
                throw new HttpResponseException(System.Net.HttpStatusCode.NotFound);
            }
        }

        public T Post(T item)
        {
            var repo = new SimpleRepository<T>(connectionFactory);

            item = repo.Add(item);

            if (item != null)
            {
                return item;
            }
            else
            {
                throw new HttpResponseException(System.Net.HttpStatusCode.InternalServerError);
            }
        }

        public void Put(int id, T item)
        {
            var repo = new SimpleRepository<T>(connectionFactory);

            if (repo.Update(item) == false)
            {
                throw new HttpResponseException(System.Net.HttpStatusCode.NotFound);
            }
        }

        public void Delete(int id)
        {
            var repo = new SimpleRepository<T>(connectionFactory);

            if (repo.Delete(id) == false)
            {
                throw new HttpResponseException(System.Net.HttpStatusCode.NotFound);
            }
        }
    }
}
