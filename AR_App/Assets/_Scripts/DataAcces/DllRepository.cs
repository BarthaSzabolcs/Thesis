using Dapper;
using DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAcces
{
    public class DllRepository
    {
        DataTableInfo<Dll> dllTable = new DataTableInfo<Dll>();

        public DllRepository()
        {
            var sql = dllTable.CreateCommand();

            using (var con = ConnectionManager.Instance.CacheConnection)
            {
                con.Execute(sql);
            }
        }

        public void Cache(Dll model)
        {
            var sql = dllTable.InsertOrReplace(model);

            using (var con = ConnectionManager.Instance.CacheConnection)
            {
                con.Execute(sql);
            }
        }

        public IEnumerable<Dll> GetAll()
        {
            var sql = dllTable.SelectAllCommand();

            using (var con = ConnectionManager.Instance.CacheConnection)
            {
                return con.Query<Dll>(sql);
            }
        }
    }
}