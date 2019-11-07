using System;

namespace DataModels
{
    public class AssetBundle : IDataModel, IFileModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime Modified { get; set; }
    }
}
