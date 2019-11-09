using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAcces
{
    public interface IDataTableInfo
    {
        string TableName { get; }
        string CreateCommand { get; }


        string Delete();
    }
}