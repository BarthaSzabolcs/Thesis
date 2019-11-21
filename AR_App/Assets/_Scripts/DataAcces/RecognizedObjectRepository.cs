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
        private static bool uninitialized = true;

        private DataTableInfo<RecognizedObject> recognizedObjectTable = new DataTableInfo<RecognizedObject>();
        private DataTableInfo<Content> contentTable = new DataTableInfo<Content>();
        private DataTableInfo<AssetBundle> assetBundleTable = new DataTableInfo<AssetBundle>();
        private DataTableInfo<Dll> dllTable = new DataTableInfo<Dll>();
        private DataTableInfo<DataModels.DataSet> dataSetTable = new DataTableInfo<DataModels.DataSet>();

        public RecognizedObjectRepository()
        {
            if (uninitialized)
            {
                using (var con = ConnectionManager.Instance.CacheConnection)
                {
                    con.Execute(recognizedObjectTable.CreateCommand());
                    con.Execute(contentTable.CreateCommand());
                    con.Execute(assetBundleTable.CreateCommand());
                    con.Execute(dllTable.CreateCommand());
                    con.Execute(dataSetTable.CreateCommand());
                }

                uninitialized = false;
            }
        }

        public RecognizedObjectResource GetRecognizedObject(int id)
        {
            var sql =
            $@"SELECT *

            FROM RecognizedObject ro
            LEFT JOIN Content c ON ro.ContentId = c.Id
            LEFT JOIN AssetBundle a ON c.AssetBundleId = a.Id
            LEFT JOIN Dll ON Dll.Id = c.DllId

            WHERE ro.Id = {id}"; ;

            using (var con = Connection)
            {
                return con.Query<RecognizedObjectResource, ContentResource, Dll, AssetBundle, RecognizedObjectResource>(
                    sql: sql,
                    (recognizedObject, content, dll, assetBundle) =>
                    {
                        if (content != null)
                        {
                            content.AssetBundle = assetBundle;
                            content.Dll = dll;
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

            var contentModel = new Content()
            {
                Id = recognizedResource.Content.Id,
                Name = recognizedResource.Content.Name,
                AssetBundleId = recognizedResource.Content.AssetBundle.Id,
                Modified = recognizedResource.Content.Modified,
            };

            var assetBundleModel = new AssetBundle()
            {
                Id = recognizedResource.Content.AssetBundle.Id,
                Name = recognizedResource.Content.AssetBundle.Name,
                Modified = recognizedResource.Content.AssetBundle.Modified
            };

            using (var con = Connection)
            {
                con.Execute(recognizedObjectTable.InsertOrReplace(recognizedModel));
                con.Execute(contentTable.InsertOrReplace(contentModel));
                con.Execute(assetBundleTable.InsertOrReplace(assetBundleModel));
            }
        }
    }
}
