using DataAcces.Infrastructure;
using DataAcces.Repositories;
using DataResources;
using System.Collections.Generic;
using System.Web.Http;

namespace DemoAPI.Controllers
{
    public class RecognizedObjectController : ApiController
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
