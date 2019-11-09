using DataModels;
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

namespace Repository
{
    public class ResourceRepository<T> : IRepository 
        where T : IDataModel, new()
    {
        private string tableName;
        private List<KeyValuePair<PropertyInfo, string>> coloumns = new List<KeyValuePair<PropertyInfo, string>>();
        private IDbConnection con;

        public ResourceRepository()
        {
            con = new SqliteConnection("Data Source=" + Path.Combine(Application.persistentDataPath, "Cache.db"));

            tableName = typeof(T).Name;

            var properties = typeof(T).GetProperties();
            foreach (var prop in properties)
            {
                var type = prop.PropertyType;
                string dataBaseType;

                if (type == typeof(int))
                {
                    dataBaseType = "INTEGER";
                }
                else if(type == typeof(float))
                {
                    dataBaseType = "REAL";
                }
                else
                {
                    dataBaseType = "TEXT";
                }

                coloumns.Add(new KeyValuePair<PropertyInfo, string>(prop, dataBaseType));
            }

            CreateTable();
        }

        #region Create

        public void CreateTable()
        {
            using (con)
            {
                con.Open();
                IDbCommand command = con.CreateCommand();
                command.CommandText = CreateCommand();

                command.ExecuteNonQuery();
            }
        }
        private string CreateCommand()
        {
            string statement = $"CREATE TABLE IF NOT EXISTS {tableName} (";
            
            for (int i = 0; i < coloumns.Count; i++)
            {
                statement += $" {coloumns[i].Key.Name}  {coloumns[i].Value}";
                if (i < coloumns.Count -1)
                {
                    statement += ",";
                }
                else
                {
                    statement += ")";
                }
            }

            return statement;
        }

        #endregion
        #region Insert

        public string Add(List<T> items)
        {
            return $"INSERT INTO { tableName }{ Coloumns() }{ Values(items) }";
        }
        private string Coloumns()
        {
            string result = "(";
            for (int i = 0; i < coloumns.Count; i++)
            {
                result += $" {coloumns[i].Key.Name}";
                if (i < coloumns.Count - 1)
                {
                    result += ",";
                }
                else
                {
                    result += ")";
                }
            }

            return result;
        }
        private string Values(List<T> items)
        {
            var result = "";

            for (int i = 0; i < items.Count; i++)
            {

                result += Value(items[i]);

                if (i < items.Count - 1)
                {
                    result += ",";
                }
            }

            return result;
        }
        private string Value(T item)
        {
            string result = "(";

            for (int j = 0; j < coloumns.Count; j++)
            {
                var value = coloumns[j].Key.GetValue(item);

                if (coloumns[j].Key.PropertyType == typeof(int) || coloumns[j].Key.PropertyType == typeof(float))
                {
                    result += $"{ value.ToString() }";
                }
                else
                {
                    result += $"'{ value.ToString() }'";
                }

                if (j < coloumns.Count - 1)
                {
                    result += ",";
                }
                else
                {
                    result += ")";
                }
            }

            return result;
        }

        #endregion 
        #region Select

        public IEnumerable<T> GetAll()
        {
            var sql = $"SELECT * FROM {tableName}";

            return ReadTable(sql);
        }
        private IEnumerable<T> ReadTable(string sql)
        {
            using (con)
            {
                con.Open();
                IDbCommand command = con.CreateCommand();
                command.CommandText = sql;
                IDataReader reader = command.ExecuteReader();

                var items = new List<T>();
                while (reader.Read())
                {
                    items.Add(ReadItem(reader));    
                }

                return items;
            }
        }
        private T ReadItem(IDataReader reader)
        {
            var item = new T();
            for (int i = 0; i < coloumns.Count; i++)
            {
                var value = Convert.ChangeType(reader[i], coloumns[i].Key.PropertyType);

                if (value != null)
                {
                    coloumns[i].Key.SetValue(item, value);
                }
            }
            return item;
        }

        #endregion
        #region Delete

	    public int Delete(int id)
        {
            using (con)
            {
                con.Open();
                IDbCommand command = con.CreateCommand();
                command.CommandText = $"DELETE FROM { tableName } WHERE Id = { id }";

                return command.ExecuteNonQuery();
            }
        }
        public int DeleteAll()
        {
            using (con)
            {
                con.Open();
                IDbCommand command = con.CreateCommand();
                command.CommandText = $"DELETE FROM { tableName }";

                return command.ExecuteNonQuery();
            }
        }

        #endregion
        #region Update

        public int Update(T item)
        {
            using (con)
            {
                con.Open();
                IDbCommand command = con.CreateCommand();
                command.CommandText = UpdateCommand(item);

                return command.ExecuteNonQuery();
            }
        }
        private string UpdateCommand(T item)
        {
            var result = $"UPDATE { tableName } SET ";

            for (int i = 0; i < coloumns.Count; i++)
            {
                if (coloumns[i].Key.PropertyType == typeof(float) ||
                    coloumns[i].Key.PropertyType == typeof(int))
                {
                    result += $"{ coloumns[i].Key.Name } = { coloumns[i].Key.GetValue(item).ToString() }";
                }
                else
                {
                    result += $"{ coloumns[i].Key.Name } = '{ coloumns[i].Key.GetValue(item).ToString() }'";
                }

                if (i < coloumns.Count -1)
                {
                    result += ", ";
                }
            }

            result += $" WHERE Id = { item.Id }";

            return result;
        }
        
        #endregion
    }
}


//public static class Utility
//{
//    public static TModel ToDataModel<TResource, TModel>(this TResource resource)
//        where TModel : class, IDataModel, new()
//    {
//        var modelProperties = typeof(TModel).GetProperties();
//        var model = new TModel();

//        foreach (var modelProp in modelProperties)
//        {
//            PropertyInfo resourceProp = typeof(TResource).GetProperty(modelProp.Name);

//            if (resourceProp != null /*&& resourceProp.GetType() == modelProp.GetType()*/)
//            {
//                modelProp.SetValue(model, resourceProp.GetValue(resource));
//            }
//            else if (modelProp.Name.Substring(modelProp.Name.Length - 2) == "Id")
//            {
//                var resourcePropertyName = modelProp.Name.Substring(0, modelProp.Name.Length - 2);
//                PropertyInfo resourceProperty = typeof(TResource).GetProperty(resourcePropertyName);

//                if (resourceProperty != null)
//                {
//                    int value = (resourceProperty.GetValue(resource) as IDataModel).Id;
//                    modelProp.SetValue(model, value);
//                }
//            }
//            else
//            {
//                throw new Exception("Could not convert the Resource to the given DataModel.");
//            }
//        }

//        return model;
//    }

//}