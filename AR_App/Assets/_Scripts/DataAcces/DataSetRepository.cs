﻿using Dapper;
using Mono.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace DataAcces
{
    public class DataSetRepository
    {
        IDbConnection Connection { get; set; }
        DataTableInfo<DataModels.DataSet> dataSetTable = new DataTableInfo<DataModels.DataSet>();

        public DataSetRepository(SqliteConnection connection)
        {
            Connection = connection;
        }

        public void CacheDataSet(DataModels.DataSet dataSet)
        {
            var sql = dataSetTable.UpsertCommand(dataSet);

            using (var con = Connection)
            {
                con.Execute(sql);
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

        public IEnumerable<DataModels.DataSet> GetDataSets()
        {
            var sql = $"SELECT * FROM { dataSetTable.TableName }";

            using (var con = Connection)
            {
                return con.Query<DataModels.DataSet>(sql: sql);
            }
        }
    }
}
