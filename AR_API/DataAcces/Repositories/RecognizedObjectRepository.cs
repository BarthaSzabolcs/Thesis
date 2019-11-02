using DataModels;
using DataAcces.Infrastructure;
using DataResources;
using System.Collections.Generic;
using System.Linq;
using DataAcces.Mapping;

namespace DataAcces.Repositories
{
    public class RecognizedObjectRepository : CompositeRepository<RecognizedObjectResource, RecognizedObject>
    {
        public RecognizedObjectRepository(IConnectionFactory connectionFactory) : base(connectionFactory)
        {
        }

        public override IEnumerable<RecognizedObjectResource> GetAll()
        {
            string sql =
            @"SELECT *

            FROM recognizedobject ro
            LEFT JOIN content c ON ro.ContentId = c.Id
            LEFT JOIN assetbundle a ON c.AssetBundleId = a.Id

            ORDER By ro.Name";

            var mapper = new RecognizedObjectMapper(connectionFactory);

            return mapper.MappedQuery(sql);
        }

        public override RecognizedObjectResource Get(int id)
        {
            string sql =
            @"SELECT *

            FROM recognizedobject ro
            LEFT JOIN content c ON ro.ContentId = c.Id
            LEFT JOIN assetbundle a ON c.AssetBundleId = a.Id
            
            WHERE ro.Id = {0}
            
            ORDER By ro.Name";

            sql = string.Format(sql, id);

            var mapper = new RecognizedObjectMapper(connectionFactory);

            return mapper.MappedQuery(sql).FirstOrDefault();
        }

        //public IEnumerable<RecognizedObjectResource> GetAllFiltered(int id = -1)
        //{
        //    var sql = "dunno";
        //    var mapper = new RecognizedObjectMapper(connectionFactory);

        //    var parameters = new
        //    {
        //        unitId = id
        //    };

        //    return mapper.MappedQuery(sql, parameters);
        //}
    }
}
