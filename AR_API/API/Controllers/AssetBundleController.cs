using DataAcces.Infrastructure;
using DataAcces.Repositories;
using DataAcces.Repositories.FileAcces;
using DataModels;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

namespace DemoAPI.Controllers
{
    [RoutePrefix("api/AssetBundle")]
    public class AssetBundleController : ApiController
    {
        IConnectionFactory con = new MySqlConnectionFactory();

        public AssetBundle Get(int id)
        {
            var repo = new SimpleRepository<AssetBundle>(con);

            return repo.Get(id);
        }

        public IEnumerable<AssetBundle> GetAll()
        {
            var repo = new SimpleRepository<AssetBundle>(con);

            return repo.GetAll();
        }

        [HttpGet]
        [Route("{id}/File")]
        public HttpResponseMessage GetFile(int id)
        {
            var repo = new FileAccesRepository<AssetBundle>(new MySqlConnectionFactory());
            var result = new HttpResponseMessage(HttpStatusCode.BadRequest);

            var (data, name) = repo.GetFile(id);

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
            else
            {
                result.StatusCode = HttpStatusCode.InternalServerError;
            }

            return result;
        }
    }
}
