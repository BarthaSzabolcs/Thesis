using Dapper;
using DataModels;
using DataResources;
using Mono.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace DataAcces
{
    public class CachedTableManager
    {
        private IDbConnection Connection => new SqliteConnection("Data Source=" + Path.Combine(Application.persistentDataPath, "Cache.db"));

        private DataTableInfo<RecognizedObject> recognizedObjectTable = new DataTableInfo<RecognizedObject>();
        private DataTableInfo<Content> contentTable = new DataTableInfo<Content>();
        private DataTableInfo<DataModels.AssetBundle> assetBundleTable = new DataTableInfo<DataModels.AssetBundle>();
        private DataTableInfo<DataModels.DataSet> dataSetTable = new DataTableInfo<DataModels.DataSet>();

        public void CreateTables()
        {
            using (var con = Connection)
            {
                con.Execute(contentTable.CreateCommand());
                con.Execute(assetBundleTable.CreateCommand());
                con.Execute(dataSetTable.CreateCommand());
            }
        }

        public IEnumerable<RecognizedObjectResource> GetRecognizedObjects()
        {
            var sql =
            @"SELECT * FROM RecognizedObject ro 
            LEFT JOIN Content c ON c.Id = ro.ContentId
            LEFT JOIN AssetBundle ab ON ab.Id = c.AssetBundleId";

            using (var con = Connection)
            {
                return con.Query<RecognizedObjectResource, ContentResource, DataModels.AssetBundle, RecognizedObjectResource>(
                    sql: sql,
                    (recognizedObject, content, assetBundle) =>
                    {
                        if (content != null)
                        {
                            content.AssetBundle = assetBundle;
                        }
                        recognizedObject.Content = content;

                        return recognizedObject;
                    });
            }
        }

        public IEnumerable<DataModels.DataSet> GetDataSets()
        {
            var sql = $"SELECT * FROM { dataSetTable.TableName }";

            using (var con = Connection)
            {
                return con.Query<DataModels.DataSet>(sql: sql);
            }
        }

        public IEnumerable<DataModels.DataSet> FilterOutdatedDatasets(IEnumerable<DataModels.DataSet> onlineDataSets)
        {
            var result = new List<DataModels.DataSet>();
            var cachedDataSets = GetDataSets();

            foreach (var onlineSet in onlineDataSets)
            {
                var cachedSet = cachedDataSets.First(x => x.Id == onlineSet.Id);

                if (cachedSet?.Modified != onlineSet.Modified)
                {
                    result.Add(onlineSet);
                }
            }

            return result;
        }

        public IEnumerable<DataModels.AssetBundle> FilterOutdatedAssetBundles(IEnumerable<DataModels.AssetBundle> onlineBundles)
        {
            var result = new List<DataModels.AssetBundle>();
            var cachedBundles = GetDataSets();

            foreach (var onlineBundle in onlineBundles)
            {
                var cachedBundle = cachedBundles.First(x => x.Id == onlineBundle.Id);

                if (cachedBundle?.Modified != onlineBundle.Modified)
                {
                    result.Add(onlineBundle);
                }
            }

            return result;
        }

        public void CacheDataSet(DataModels.DataSet dataSet)
        {
            var sql = dataSetTable.UpsertCommand(dataSet);

            using (var con = Connection)
            {
                con.Execute(sql);
            }
        }
        public void CacheAssetBundle(DataModels.AssetBundle assetBundle)
        {
            var sql = assetBundleTable.UpsertCommand(assetBundle);

            using (var con = Connection)
            {
                con.Execute(sql);
            }
        }
        public void CacheRecognizedObject(RecognizedObjectResource recognizedResource)
        {
            // ToDo - ?
            var recognizedModel = new RecognizedObject()
            {
                Id = recognizedResource.Id,
                Name = recognizedResource.Name,
                ContentId = recognizedResource.Content.Id,
                Modified = recognizedResource.Modified
            };

            var sql = recognizedObjectTable.UpsertCommand(recognizedModel);

            using (var con = Connection)
            {
                con.Execute(sql);
            }
        }
        //public IEnumerable<DataModels.DataSet> DownloadedDataSets(IEnumerable<DataModels.DataSet> dataSets)
        //{
        //    var result = new List<DataModels.DataSet>();

        //    foreach (var dataSet in dataSets)
        //    {
        //        var pathXml = Path.Combine(CachePath, dataSet.Name + ".xml");
        //        var pathDat = Path.Combine(CachePath, dataSet.Name + ".dat");

        //        if (File.Exists(pathXml) && File.Exists(pathDat))
        //        {
        //            result.Add(dataSet);
        //        }
        //    }

        //    return result;
        //}
    }
}