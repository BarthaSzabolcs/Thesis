using DataModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;

namespace DataAcces.ExtensionMethods
{
    public static class DataAccesExtension
    {
        public static string FilePath(this IFileModel fileModel, string subFolder = null, string extension = null)
        {
            var path = ConfigurationManager.AppSettings[fileModel.GetType().Name];

            if (subFolder != null)
            {
                path = Path.Combine(path, subFolder);
            }

            var fileName = extension is null ? 
                fileModel.Name :
                fileModel.Name + "." + extension;

            return Path.Combine(path, fileName);
        }

        public static T GetCachedModel<T>(this Dictionary<int, T> dictionary, T dataModel) where T : IDataModel
        {
            if (dataModel != null)
            {
                if (dictionary.TryGetValue(dataModel.Id, out var cahcedDataModel))
                {
                    dataModel = cahcedDataModel;
                }
                else
                {
                    dictionary.Add(dataModel.Id, dataModel);
                }
            }

            return dataModel;
        }
    }
}