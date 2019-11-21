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
        public HttpResponseMessage GetFile(int id, Platform platform)
        {
            var repo = new FileAccesRepository<AssetBundle>(new MySqlConnectionFactory(), subFolder: platform.ToString());
            var result = new HttpResponseMessage(HttpStatusCode.NotFound);

            if (Enum.IsDefined(typeof(Platform), platform) == false)
                return result;

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

        [HttpGet]
        public IEnumerable<AssetBundle> GetAssetBundleByContentId(int contentId)
        {
            var repo = new SimpleRepository<AssetBundle>(con);

            var predicate = Predicates.Field<AssetBundle>(r => r.Id, Operator.Eq, contentId);

            return repo.GetAll(predicate);
        }
    }
}

public enum Platform
{
    Standalone,
    Android
}