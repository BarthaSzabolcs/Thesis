using DataModels;
using System;

namespace DataResources
{
    public class ContentResource : IDataModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public AssetBundle AssetBundle { get; set; }
        public Dll Dll { get; set; }
        public DateTime Modified { get; set; }
    }
}
