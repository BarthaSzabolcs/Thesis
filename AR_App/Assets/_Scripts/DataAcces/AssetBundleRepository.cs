using Dapper;
using DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAcces
{
    public class AssetBundleRepository
    {
        DataTableInfo<AssetBundle> assetBundleTable = new DataTableInfo<AssetBundle>();

        public AssetBundleRepository()
        {
            var sql = assetBundleTable.CreateCommand();

            using (var con = ConnectionManager.Instance.CacheConnection)
            {
                con.Execute(sql);
            }
        }

        public void Cache(AssetBundle model)
        {
            var sql = assetBundleTable.InsertOrReplace(model);

            using (var con = ConnectionManager.Instance.CacheConnection)
            {
                con.Execute(sql);
            }
        }

        public IEnumerable<AssetBundle> GetAll()
        {
            var sql = assetBundleTable.SelectAllCommand();

            using (var con = ConnectionManager.Instance.CacheConnection)
            {
                return con.Query<AssetBundle>(sql);
            }
        }
    }
}
