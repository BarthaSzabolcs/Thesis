namespace DataModels
{
    public class FilePath : IDataModel
    {
        public int Id { get; set; }
        public string Path { get; set; }
        public int FileTypeId { get; set; }
    }
}
