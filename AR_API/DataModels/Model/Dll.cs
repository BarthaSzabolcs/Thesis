using System;

namespace DataModels
{
    public class Dll : IDataModel, IFileModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime Modified { get; set; }
    }
}
