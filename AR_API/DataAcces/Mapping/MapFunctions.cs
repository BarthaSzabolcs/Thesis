using DataModels;
using DataAcces.ExtensionMethods;
using DataResources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataAcces.Mapping.ExtensionMethods
{
    public static class MapFunctions
    {
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

        public static RecognizedObjectResource Map(this RecognizedObjectResource recognizedObject, ContentResource content)
        {
            if (recognizedObject == null)
                return null;

            recognizedObject.Content = content;

            return recognizedObject;
        }

        public static ContentResource Map(this ContentResource content, AssetBundle assetBundle, Dll dll)
        {
            if (content == null)
                return null;

            content.AssetBundle = assetBundle;
            content.Dll = dll;

            return content;
        }
    }
}
