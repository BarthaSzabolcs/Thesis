namespace DataModels
{
    public class RecognizedObject : IDataModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int ContentId { get; set; }
    }
}