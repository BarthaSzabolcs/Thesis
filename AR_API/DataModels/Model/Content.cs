namespace DataModels
{
    public class Content : IDataModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int AssetBundleId { get; set; }
    }
}
