using DataAcces.Infrastructure;
using DataModels;
using System.IO;
using DataAcces.ExtensionMethods;

namespace DataAcces.Repositories.FileAcces
{
    public class FileAccesRepository<TModel> : SimpleRepository<TModel> 
        where TModel : class, IDataModel, IFileModel, new()
    {
        public FileAccesRepository(IConnectionFactory connectionFactory) : base(connectionFactory)
        {
        }

        public virtual (byte[] data, string name) GetFile(int id, string subFolder = null, string extension = null)
        {
            (byte[] data, string name) result = (null, null);

            var fileInfo = Get(id);
            if (fileInfo is null) return (null, null);

            var path = fileInfo.FilePath(subFolder: subFolder, extension: extension);
            
            if (File.Exists(path))
            {
                var dataBytes = File.ReadAllBytes(path);
                var dataStream = new MemoryStream(dataBytes);

                result.name += extension is null ? fileInfo.Name : fileInfo.Name + "." + extension;
                result.data = dataStream.ToArray();
            }

            return result;
        }

        public virtual bool AddFile(TModel fileModel, byte[] fileData)
        {
            var path = fileModel.FilePath();

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

        public virtual bool DeleteFile(TModel bundle)
        {
            var path = bundle.FilePath();

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

        public virtual bool UpdateFile(TModel bundle, byte[] fileData)
        {
            var path = bundle.FilePath();

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
