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
        public static RecognizedObjectResource Map(this RecognizedObjectResource recognizedObject, ContentResource content, AssetBundle fileInfo)
        {
            if (recognizedObject == null)
                return null;

            recognizedObject.Content = content;

            return recognizedObject;
        }

        public static ContentResource Map(this ContentResource content, AssetBundle assetBundle)
        {
            if (content == null)
                return null;

            content.AssetBundle = assetBundle;
            return content;
        }
    }
}
