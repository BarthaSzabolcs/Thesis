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
        public static string FilePath(this DataAcces.DataModels.FileInfo fileInfo)
        {
            var path = ConfigurationManager.AppSettings[fileInfo.Type.ToString()];

            return Path.Combine(path, fileInfo.Name);
        }
    }
}