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
                }

                uninitialized = false;
            }
        }

        public RecognizedObjectResource Get(int id)
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
                return con.Query<RecognizedObjectResource, ContentResource, AssetBundle, Dll, RecognizedObjectResource >(
                    sql: sql,
                    (recognizedObject, content, assetBundle, dll) =>
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

        public void Cache(RecognizedObjectResource recognizedResource)
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
                DllId = recognizedResource.Content.Dll.Id,
                Modified = recognizedResource.Content.Modified
            };
            if (recognizedResource.Content.Dll != null)
            {
                contentModel.DllId = recognizedResource.Content.Dll.Id;
            }
            var assetBundleModel = new AssetBundle()
            {
                Id = recognizedResource.Content.AssetBundle.Id,
                Name = recognizedResource.Content.AssetBundle.Name,
                Modified = recognizedResource.Content.AssetBundle.Modified
            };

            Dll dll = null;
            if (recognizedResource.Content?.Dll != null)
            {
                dll = new Dll()
                {
                    Id = recognizedResource.Content.Dll.Id,
                    Name = recognizedResource.Content.Dll.Name,
                    Modified = recognizedResource.Content.Dll.Modified
                };
            }

            using (var con = Connection)
            {
                con.Execute(recognizedObjectTable.InsertOrReplace(recognizedModel));
                con.Execute(contentTable.InsertOrReplace(contentModel));
                con.Execute(assetBundleTable.InsertOrReplace(assetBundleModel));

                if (dll != null)
                    con.Execute(dllTable.InsertOrReplace(dll));
            }
        }
    }
}