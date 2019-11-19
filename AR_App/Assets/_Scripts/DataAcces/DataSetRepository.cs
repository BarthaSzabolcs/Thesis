using Dapper;
using DataModels;
using System.Collections.Generic;
using System.Linq;

namespace DataAcces
{
    public class DataSetRepository
    {
        DataTableInfo<DataSet> dataSetTable = new DataTableInfo<DataSet>();

        public DataSetRepository()
        {
            var sql = dataSetTable.CreateCommand();

            using (var con = ConnectionManager.Instance.CacheConnection)
            {
                con.Execute(sql);
            }
        }

        public void Cache(DataSet dataSet)
        {
            var sql = dataSetTable.InsertOrReplace(dataSet);

            using (var con = ConnectionManager.Instance.CacheConnection)
            {
                con.Execute(sql);
            }
        }

        public IEnumerable<DataSet> GetAll()
        {
            var sql = $"SELECT * FROM { dataSetTable.TableName }";

            using (var con = ConnectionManager.Instance.CacheConnection)
            {
                return con.Query<DataSet>(sql);
            }
        }
    }
}
