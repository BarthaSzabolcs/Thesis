using DapperExtensions;
using DataAcces.Infrastructure;
using DataAcces.Repositories;
using DataAcces.Repositories.FileAcces;
using DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

namespace DemoAPI.Controllers
{
    [RoutePrefix("api/Dll")]
    public class DllController : ApiController
    {
        IConnectionFactory con = new MySqlConnectionFactory();

        public Dll Get(int id)
        {
            var repo = new SimpleRepository<Dll>(con);

            return repo.Get(id);
        }

        public IEnumerable<Dll> GetAll()
        {
            var repo = new SimpleRepository<Dll>(con);

            return repo.GetAll();
        }

        [HttpGet]
        [Route("{id}/File")]
        public HttpResponseMessage GetFile(int id)
        {
            var repo = new FileAccesRepository<Dll>(new MySqlConnectionFactory(), extension: "dll");
            var result = new HttpResponseMessage(HttpStatusCode.InternalServerError);

            var (data, name) = repo.GetFile(id);

            if (data != null)
            {
                result.Content = new ByteArrayContent(data.ToArray());
                result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                {
                    FileName = name + ".dll"
                };
                result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

                result.StatusCode = HttpStatusCode.OK;
            }
            else
            {
                result.StatusCode = HttpStatusCode.NotFound;
            }

            return result;
        }
    }
}