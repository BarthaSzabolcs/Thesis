//using DataModels;
//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.Linq;
//using System.Reflection;
//using System.Text;
//using System.Threading.Tasks;

//namespace DataAcces
//{
//    public enum DataTypes
//    {
//        INTEGER,
//        REAL,
//        TEXT,
//        ForeignKey
//    }

//    public class DataTableManager
//    {
//        private static DataTableManager instance;
//        public static DataTableManager Instance
//        {
//            get
//            {
//                if (instance == null)
//                    instance = new DataTableManager();

//                return instance;
//            }
//        }

//        private Dictionary<Type, List<KeyValuePair<PropertyInfo, DataTypes>>> TableMaps =
//            new Dictionary<Type, List<KeyValuePair<PropertyInfo, DataTypes>>>();

//        public List<KeyValuePair<PropertyInfo, DataTypes>> GetColoumns<T>()
//        {
//            if (TableMaps.TryGetValue(typeof(T), out var coloumns))
//            {
//                return coloumns;
//            }
//            else
//            {
//                return MapType<T>();
//            }
//        }
//        private List<KeyValuePair<PropertyInfo, DataTypes>> MapType<T>()
//        {
//            var coloumns = new List<KeyValuePair<PropertyInfo, DataTypes>>();

//            foreach (var prop in typeof(T).GetProperties())
//            {
//                var type = prop.PropertyType;
//                DataTypes dataBaseType;

//                if (type == typeof(int))
//                {
//                    dataBaseType = DataTypes.INTEGER;
//                }
//                else if (type == typeof(float))
//                {
//                    dataBaseType = DataTypes.REAL;
//                }
//                else if (type is IDataModel)
//                {
//                    dataBaseType = DataTypes.ForeignKey;
//                }
//                else
//                {
//                    dataBaseType = DataTypes.TEXT;
//                }

//                coloumns.Add(new KeyValuePair<PropertyInfo, DataTypes>(prop, dataBaseType));
//            }

//            TableMaps.Add(typeof(T), coloumns);

//            return coloumns;
//        }
//    }

//    public class DataTable<T> where T : IDataModel, new()
//    {
//        private string tableName;
//        private List<KeyValuePair<PropertyInfo, DataTypes>> coloumns;
//        //private List<IDataTableInfo> joinedTables = new List<IDataTableInfo>();

//        public DataTable()
//        {
//            tableName = typeof(T).Name;
//            coloumns = DataTableManager.Instance.GetColoumns<T>();
//        }

//        public string CreateCommand()
//        {
//            var result = $"CREATE TABLE IF NOT EXISTS {tableName} (";

//            for (int i = 0; i < coloumns.Count; i++)
//            {
//                result += $" {coloumns[i].Key.Name}  {coloumns[i].Value}";
//                if (i < coloumns.Count - 1)
//                {
//                    result += ",";
//                }
//                else
//                {
//                    result += ")";
//                }
//            }

//            return result;
//        }

//        public string Coloumns()
//        {
//            string result = "(";
//            for (int i = 0; i < coloumns.Count; i++)
//            {
//                result += $" {coloumns[i].Key.Name}";
//                if (i < coloumns.Count - 1)
//                {
//                    result += ",";
//                }
//                else
//                {
//                    result += ")";
//                }
//            }

//            return result;
//        }
//        public string ValuesToString(List<T> items)
//        {
//            var result = "";

//            for (int i = 0; i < items.Count; i++)
//            {
//                result += ValueToString(items[i]);

//                if (i < items.Count - 1)
//                {
//                    result += ",";
//                }
//            }

//            return result;
//        }
//        public string ValueToString(T item)
//        {
//            string result = "(";

//            for (int j = 0; j < coloumns.Count; j++)
//            {
//                var value = coloumns[j].Key.GetValue(item);

//                if (coloumns[j].Key.PropertyType == typeof(int) || coloumns[j].Key.PropertyType == typeof(float))
//                {
//                    result += $"{ value.ToString() }";
//                }
//                else
//                {
//                    result += $"'{ value.ToString() }'";
//                }

//                if (j < coloumns.Count - 1)
//                {
//                    result += ",";
//                }
//                else
//                {
//                    result += ")";
//                }
//            }

//            return result;
//        }

//        public string UpdateCommand(T item)
//        {
//            var result = $"UPDATE { tableName } SET ";

//            for (int i = 0; i < coloumns.Count; i++)
//            {
//                if (coloumns[i].Key.PropertyType == typeof(float) ||
//                    coloumns[i].Key.PropertyType == typeof(int))
//                {
//                    result += $"{ coloumns[i].Key.Name } = { coloumns[i].Key.GetValue(item).ToString() }";
//                }
//                else
//                {
//                    result += $"{ coloumns[i].Key.Name } = '{ coloumns[i].Key.GetValue(item).ToString() }'";
//                }

//                if (i < coloumns.Count - 1)
//                {
//                    result += ", ";
//                }
//            }

//            result += $" WHERE Id = { item.Id }";

//            return result;
//        }

//        public string DeleteCommand()
//        {
//            return $"DELETE FROM { tableName }";
//        }
//    }

//}
