using DataModels;

namespace DataResources
{
    public class ContentResource : IDataModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public AssetBundle AssetBundle { get; set; }
    }
}
