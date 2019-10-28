using DataAcces.Infrastructure;
using DataModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAcces.Repositories
{
    public class FileRepository : GenericRepository<FilePath>
    {
        public FileRepository(IConnectionFactory connectionFactory) : base(connectionFactory)
        {
        }

        public bool DeleteFile(FilePath filePath)
        {
            if (File.Exists(filePath.Path))
            {
                try
                {
                    File.Delete(filePath.Path);

                    return true;
                }
                catch
                {
                    return false;
                }
            }

            return false;
        }

        public bool UpdateFile(FilePath filePath, byte[] fileData)
        {
            try
            {
                File.WriteAllBytes(filePath.Path, fileData);

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
