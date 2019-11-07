using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModels
{
    public interface IDataModel
    {
        int Id { get; set; }
        DateTime Modified { get; set; }
    }
}
