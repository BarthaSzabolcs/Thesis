using DataAcces.Infrastructure;
using DataAcces.Repositories;
using DataAcces.DataModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;
using DataAcces.ExtensionMethods;

namespace DemoAPI.Controllers
{
    [RoutePrefix("Api/File")]
    public class FileController : ApiController
    {
        [HttpGet]
        public HttpResponseMessage GetFile(int id)
        {
            var repo = new FileRepository(new MySqlConnectionFactory());
            var result = new HttpResponseMessage(HttpStatusCode.OK);

            var fileInfo = repo.Get(id);
            var path = fileInfo.FilePath();
            if (fileInfo != null && File.Exists(path))
            {
                var dataBytes = File.ReadAllBytes(path);
                var dataStream = new MemoryStream(dataBytes);

                result.Content = new ByteArrayContent(dataStream.ToArray());

                result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                {
                    FileName = fileInfo.Name
                };
                result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            }
            else
            {
                result.StatusCode = HttpStatusCode.NotFound;
            }
            
            return result;
        }
    }
}
