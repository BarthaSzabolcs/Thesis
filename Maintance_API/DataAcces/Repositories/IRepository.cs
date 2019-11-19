using System.Collections;
using System.Collections.Generic;

namespace DataAcces.Repositories
{
    public interface IRepository<TModel>
    {
        TModel Get(int Id);
        IEnumerable<TModel> GetAll();

        TModel Add(TModel entity);

        bool Update(TModel entity);

        bool Delete(int id);
    }
}
