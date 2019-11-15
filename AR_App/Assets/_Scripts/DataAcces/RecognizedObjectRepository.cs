using Dapper;
using DataModels;
using DataResources;
using System.Data;
using System.Linq;

namespace DataAcces
{
    public class RecognizedObjectRepository
    {
        private IDbConnection Connection => ConnectionManager.Instance.CacheConnection;

        private DataTableInfo<RecognizedObject> recognizedObjectTable = new DataTableInfo<RecognizedObject>();
        private DataTableInfo<Content> contentTable = new DataTableInfo<Content>();
        private DataTableInfo<AssetBundle> assetBundleTable = new DataTableInfo<AssetBundle>();
        private DataTableInfo<DataModels.DataSet> dataSetTable = new DataTableInfo<DataModels.DataSet>();

        public RecognizedObjectRepository()
        {
            using (var con = ConnectionManager.Instance.CacheConnection)
            {
                con.Execute(recognizedObjectTable.CreateCommand());
                con.Execute(contentTable.CreateCommand());
                con.Execute(assetBundleTable.CreateCommand());
                con.Execute(dataSetTable.CreateCommand());
            }
        }

        public RecognizedObjectResource GetRecognizedObject(int id)
        {
            var sql =
            $@"SELECT * FROM RecognizedObject ro 
            LEFT JOIN Content c ON c.Id = ro.ContentId
            LEFT JOIN AssetBundle ab ON ab.Id = c.AssetBundleId
            WHERE Id = { id }";

            using (var con = Connection)
            {
                return con.Query<RecognizedObjectResource, ContentResource, AssetBundle, RecognizedObjectResource>(
                    sql: sql,
                    (recognizedObject, content, assetBundle) =>
                    {
                        if (content != null)
                        {
                            content.AssetBundle = assetBundle;
                        }
                        recognizedObject.Content = content;

                        return recognizedObject;
                    })
                    .FirstOrDefault();
            }
        }

        public void CacheRecognizedObject(RecognizedObjectResource recognizedResource)
        {
            var recognizedModel = new RecognizedObject()
            {
                Id = recognizedResource.Id,
                Name = recognizedResource.Name,
                ContentId = recognizedResource.Content.Id,
                Modified = recognizedResource.Modified
            };

            var sql = recognizedObjectTable.InsertOrReplace(recognizedModel);

            using (var con = Connection)
            {
                con.Execute(sql);
            }
        }
    }
}
