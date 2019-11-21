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

            FROM RecognizedObject ro
            LEFT JOIN Content c ON ro.ContentId = c.Id
            LEFT JOIN AssetBundle a ON c.AssetBundleId = a.Id
            LEFT JOIN Dll ON Dll.Id = c.DllId

            ORDER By ro.Name";

            var mapper = new RecognizedObjectMapper(connectionFactory);

            return mapper.MappedQuery(sql);
        }

        public override RecognizedObjectResource Get(int id)
        {
            string sql =
            $@"SELECT *

            FROM RecognizedObject ro
            LEFT JOIN Content c ON ro.ContentId = c.Id
            LEFT JOIN AssetBundle a ON c.AssetBundleId = a.Id
            LEFT JOIN Dll ON Dll.Id = c.DllId

            WHERE ro.Id = {id}";
            
            var mapper = new RecognizedObjectMapper(connectionFactory);

            return mapper.MappedQuery(sql).FirstOrDefault();
        }
    }
}
