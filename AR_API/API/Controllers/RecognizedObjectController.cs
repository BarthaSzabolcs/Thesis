using DataAcces.DataModels;
using DataAcces.Infrastructure;
using DataAcces.Repositories;
using DataAcces.Resources;
using System.Collections.Generic;
using System.Web.Http;

namespace DemoAPI.Controllers
{
    public class RecognizedObjectController : ApiController //GenericController<RecognizedObject>
    {
        public IEnumerable<RecognizedObjectResource> Get()
        {
            var repo = new RecognizedObjectRepository(new MySqlConnectionFactory());

            return repo.GetAll();
        }

        public RecognizedObjectResource Get(int id)
        {
            var repo = new RecognizedObjectRepository(new MySqlConnectionFactory());

            return repo.Get(id);
        }
    }
}
