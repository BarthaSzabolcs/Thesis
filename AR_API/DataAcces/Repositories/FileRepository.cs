using DataAcces.Infrastructure;
using DataAcces.DataModels;
using DataAcces.Resources;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAcces.ExtensionMethods;

namespace DataAcces.Repositories
{
    public class FileRepository : GenericRepository<DataModels.FileInfo>
    {

        public FileRepository(IConnectionFactory connectionFactory) : base(connectionFactory)
        {
        }

        public bool AddFile(DataModels.FileInfo fileInfo, byte[] fileData)
        {
            var path = fileInfo.FilePath();

            if (File.Exists(path) == false)
            {
                try
                {
                    File.WriteAllBytes(path, fileData);

                    return true;
                }
                catch
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public bool DeleteFile(DataModels.FileInfo fileInfo)
        {
            var path = fileInfo.FilePath();

            if (File.Exists(path))
            {
                try
                {
                    File.Delete(path);

                    return true;
                }
                catch
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public bool UpdateFile(DataModels.FileInfo fileInfo, byte[] fileData)
        {
            var path = fileInfo.FilePath();

            try
            {
                File.WriteAllBytes(path, fileData);

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
