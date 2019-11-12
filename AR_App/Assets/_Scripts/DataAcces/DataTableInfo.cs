using DataAcces;
using DataModels;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace DataAcces
{
    public class DataTableInfo<T> where T : IDataModel, new()
    {
        public string TableName { get; private set; }
        List<KeyValuePair<PropertyInfo, DataTypes>> coloumns = new List<KeyValuePair<PropertyInfo, DataTypes>>();

        public DataTableInfo()
        {
            TableName = typeof(T).Name;

            foreach (var prop in typeof(T).GetProperties())
            {
                var type = prop.PropertyType;
                DataTypes dataBaseType;

                if (type == typeof(int))
                {
                    dataBaseType = DataTypes.INTEGER;
                }
                else if (type == typeof(float))
                {
                    dataBaseType = DataTypes.REAL;
                }
                else if (type is IDataModel)
                {
                    dataBaseType = DataTypes.ForeignKey;
                }
                else
                {
                    dataBaseType = DataTypes.TEXT;
                }

                coloumns.Add(new KeyValuePair<PropertyInfo, DataTypes>(prop, dataBaseType));
            }
        }

        public string CreateCommand()
        {
            string statement = $"CREATE TABLE IF NOT EXISTS {TableName} (";

            for (int i = 0; i < coloumns.Count; i++)
            {
                statement += $" {coloumns[i].Key.Name}  {coloumns[i].Value.ToString()}";
                if (coloumns[i].Key.Name == "Id")
                {
                    statement += " UNIQUE";
                }

                if (i < coloumns.Count - 1)
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

        public string UpsertCommand(T item)
        {
            var result = $@"
            INSERT INTO { TableName }{ ColoumNames() }
            { InsertValue(item)} 
            ON CONFLICT(Id) DO 
            {UpdateCommand(item)}";

            Debug.Log(result);

            return result;
        }
        public string InsertCommand(List<T> items)
        {
            return $"INSERT INTO { TableName }{ ColoumNames() }{ InsertValues(items) }";
        }
        private string ColoumNames()
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
        private string InsertValues(List<T> items)
        {
            var result = "";

            for (int i = 0; i < items.Count; i++)
            {

                result += InsertValue(items[i]);

                if (i < items.Count - 1)
                {
                    result += ",";
                }
            }

            return result;
        }
        private string InsertValue(T item)
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

        public string SelectAllCommand()
        {
            return $"SELECT * FROM {TableName}";
        }
        public string SelectByIdCommand(int id)
        {
            return $"SELECT * FROM {TableName} WHERE ID = {id}";
        }

        public string DeleteByIdCommand(int id)
        {
            return $"DELETE FROM { TableName } WHERE Id = { id }";
        }
        public string DeleteAllCommand()
        {
            return $"DELETE FROM { TableName }";
        }

        private string UpdateCommand(T item)
        {
            var result = $"UPDATE { TableName } SET ";

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

                if (i < coloumns.Count - 1)
                {
                    result += ", ";
                }
            }

            return result;
        }
    }
}
