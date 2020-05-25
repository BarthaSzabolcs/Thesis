using Dapper;
using DataModels;
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
                var dllDict = new Dictionary<int, Dll>();

                return con.Query<RecognizedObjectResource, ContentResource, AssetBundle, Dll, RecognizedObjectResource>(
                        sql,
                        (recognizedObject, content, assetBundle, dll) =>
                        {
                            dll = dllDict.GetCachedModel(dll);
                            assetBundle = assetBundleDict.GetCachedModel(assetBundle);
                            content = contentDict.GetCachedModel(content.Map(assetBundle, dll));

                            return recognizedObject.Map(content);
                        },
                        param: param,
                        transaction: transaction);
            }
        }
    }
}
