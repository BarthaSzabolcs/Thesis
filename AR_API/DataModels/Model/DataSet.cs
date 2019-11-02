namespace DataModels
{
    public class DataSet : IDataModel, IFileModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
