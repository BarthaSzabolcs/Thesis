using DataAcces.DataModels;

namespace DataAcces.Resources
{
    public class RecognizedObjectResource : IDataModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ContentResource Content { get; set; }
    }
}