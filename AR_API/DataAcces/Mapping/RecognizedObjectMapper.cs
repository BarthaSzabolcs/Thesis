using Dapper;
using DataModels;
using DataAcces.ExtensionMethods;
using DataAcces.Infrastructure;
using DataAcces.Mapping.ExtensionMethods;
using DataResources;
using System.Collections.Generic;
using System.Data;

namespace DataAcces.Mapping
{
    public class RecognizedObjectMapper : ResourceMapper<RecognizedObjectResource>
    {
        public RecognizedObjectMapper(IConnectionFactory connectionFactory) : base(connectionFactory)
        {
        }

        public override IEnumerable<RecognizedObjectResource> MappedQuery(string sql, object param = null, IDbTransaction transaction = null)
        {
            using (var con = connectionFactory.Connection)
            {
                var contentDict = new Dictionary<int, ContentResource>();
                var assetBundleDict = new Dictionary<int, AssetBundle>();

                return con.Query<RecognizedObjectResource, ContentResource, AssetBundle, RecognizedObjectResource>(
                        sql,
                        (recognizedObject, content, assetBundle) =>
                        {
                            assetBundle = assetBundleDict.GetCachedModel(assetBundle);
                            content = contentDict.GetCachedModel(content.Map(assetBundle));

                            return recognizedObject.Map(content, assetBundle);
                        },
                        param: param,
                        transaction: transaction);
            }
        }
    }
}
