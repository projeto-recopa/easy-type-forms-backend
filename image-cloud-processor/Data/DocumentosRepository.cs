using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;

namespace image_cloud_processor.Repository
{
    public class DocumentosRepository : IDocumentosRepository
    {
        private IConfiguration _configuration;
        private IMongoDatabase getDatabase()
        {
            MongoClient client = new MongoClient(
                _configuration.GetConnectionString("MongoDB"));
            IMongoDatabase db = client.GetDatabase("et_forms");
            return db;
        }

        public MongoDB.Bson.ObjectId AttachFile(byte[] source)
        {
            var bucket = new GridFSBucket(getDatabase());
            //var id = bucket.UploadFromStream("filename", source);
            var id = bucket.UploadFromBytes("filename", source);
            return id;

        }

        public DocumentosRepository(IConfiguration config)
        {
            _configuration = config;
        }

        public IEnumerable<T> ListarDocumentos<T>()
        {
            IMongoDatabase db = getDatabase();
            var filter = Builders<T>.Filter.Empty;

            return db.GetCollection<T>("Forms").Find(filter).ToEnumerable<T>();
        }

        public T ObterDocumento<T>(string codigo)
        {
            IMongoDatabase db = getDatabase();

            var filter = Builders<T>.Filter.Eq("Codigo", codigo);

            return db.GetCollection<T>("Forms")
                .Find(filter)
                .FirstOrDefault();
        }

        public T SalvarOuAtualizarDocumento<T>(T documento)
        {
            MongoClient client = new MongoClient(
                _configuration.GetConnectionString("MongoDB"));
            IMongoDatabase db = client.GetDatabase("et_forms");

            db.GetCollection<T>("Forms").InsertOne(documento);
            
            return documento;
        }

        public byte[] DownloadFile(MongoDB.Bson.ObjectId id)
        {
            //return await bucket.DownloadAsBytesAsync(id);
            return null;
        }
    }
}
