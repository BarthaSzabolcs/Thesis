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

namespace DemoAPI.Controllers
{
    public class FileInfoController : ApiController
    {
        public IEnumerable<DataAcces.DataModels.FileInfo> Get()
        {
            var con = new MySqlConnectionFactory();
            var repo = new GenericRepository<DataAcces.DataModels.FileInfo>(con);

            return repo.GetAll();
        }
    }
}
