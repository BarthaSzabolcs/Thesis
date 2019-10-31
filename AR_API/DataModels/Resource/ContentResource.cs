using DataAcces.DataModels;

namespace DataAcces.Resources
{
    public class ContentResource : IDataModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public FileInfo FileInfo { get; set; }
    }
}
