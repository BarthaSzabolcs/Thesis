﻿using DataAcces.Infrastructure;
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
            var repo = new FileAccesRepository<DataSet>(new MySqlConnectionFactory(), extension: isXml ? "xml" : "dat");
            var result = new HttpResponseMessage(HttpStatusCode.InternalServerError);

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
                result.StatusCode = HttpStatusCode.NotFound;
            }

            return result;
        }
    }
}
