using System.Collections;
using System.Collections.Generic;

namespace DataAcces.Repositories
{
    public interface IGenericRepository<T> where T : class
    {
        T Get(int Id);
        IEnumerable<T> GetAll();

        T Add(T entity);

        bool Update(T entity);
        
        bool Delete(int id);
    }
}
