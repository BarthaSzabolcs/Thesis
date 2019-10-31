namespace DataAcces.DataModels
{
    public class FileInfo : IDataModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public FileType Type { get; set; }
    }

    public enum FileType { AssetBundle, DataSet }
}
