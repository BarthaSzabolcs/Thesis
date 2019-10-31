using Dapper;
using DataAcces.DataModels;
using DataAcces.Infrastructure;
using DataAcces.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace DataAcces.Repositories
{
    public class RecognizedObjectRepository
    {
        private IConnectionFactory connectionFactory;

        public RecognizedObjectRepository(IConnectionFactory connectionFactory)
        {
            this.connectionFactory = connectionFactory;
        }

        public IEnumerable<RecognizedObjectResource> GetAll()
        {
            string sql = 
                "SELECT * FROM recognizedobject ro " +
                "LEFT JOIN content c ON ro.ContentId = c.Id " +
                "LEFT JOIN fileinfo fi ON c.FileInfoId = fi.Id "+
                "ORDER By ro.Name";

            using (var connection = connectionFactory.Connection)
            {
                return connection.Query<RecognizedObjectResource, ContentResource, FileInfo, RecognizedObjectResource>(
                        sql,
                        (recognizedObject, contentResource, fileInfo) =>
                        {
                            if (contentResource != null)
                            {
                                contentResource.FileInfo = fileInfo;
                            }
                            recognizedObject.Content = contentResource;

                            return recognizedObject;
                        });
            }
        }

        public RecognizedObjectResource Get(int id)
        {
            string sql =
                "SELECT * FROM recognizedobject ro " +
                "LEFT JOIN content c ON ro.ContentId = c.Id " +
                "LEFT JOIN fileinfo fi ON c.FileInfoId = fi.Id " +
                "WHERE ro.Id = " + id;

            using (var connection = connectionFactory.Connection)
            {
                return connection.Query<RecognizedObjectResource, ContentResource, FileInfo, RecognizedObjectResource>(
                        sql,
                        (recognizedObject, contentResource, fileInfo) =>
                        {
                            if (contentResource != null)
                            {
                                contentResource.FileInfo = fileInfo;
                            }
                            recognizedObject.Content = contentResource;

                            return recognizedObject;
                        }).
                        FirstOrDefault();
            }
        }
    }
}
