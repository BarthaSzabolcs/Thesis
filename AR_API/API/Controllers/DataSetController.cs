using DataAcces.Infrastructure;
using DataAcces.Repositories;
using DataModels;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using DataAcces.Repositories.FileAcces;

namespace DemoAPI.Controllers
{
    [RoutePrefix("api/DataSet")]
    public class DataSetController : ApiController
    {
        IConnectionFactory con = new MySqlConnectionFactory();

        public DataSet Get(int id)
        {
            var repo = new SimpleRepository<DataSet>(con);

            return repo.Get(id);
        }

        public IEnumerable<DataSet> GetAll()
        {
            var repo = new SimpleRepository<DataSet>(con);

            return repo.GetAll();
        }

        [HttpGet]
        [Route("{id}/File")]
        public HttpResponseMessage GetFile(int id, bool isXml = false)
        {
            var repo = new FileAccesRepository<DataSet>(new MySqlConnectionFactory());
            var result = new HttpResponseMessage(HttpStatusCode.NotFound);

            var (data, name) = repo.GetFile(id, isXml ? "xml" : "dat");

            if (data != null)
            {
                result.Content = new ByteArrayContent(data.ToArray());
                result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                {
                    FileName = name
                };
                result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

                result.StatusCode = HttpStatusCode.OK;
            }

            return result;
        }
    }
}
