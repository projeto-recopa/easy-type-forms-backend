using image_cloud_processor.Models;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using Document = image_cloud_processor.Models.Document;

namespace image_cloud_processor.Repository
{
    public class DocumentosRepository : IDocumentosRepository<Document>
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

        public IEnumerable<Document> ListarDocumentos()
        {
            IMongoDatabase db = getDatabase();
            var filter = Builders<Document>.Filter.Empty;

            return db.GetCollection<Document>("Forms").Find(filter).ToEnumerable<Document>();
        }

        public IEnumerable<Document> ListarDocumentos(StatusDocumento status)
        {
            IMongoDatabase db = getDatabase();
            var filter = Builders<Document>.Filter.Eq(x => x.Status, status);

            return db.GetCollection<Document>("Forms").Find(filter).ToEnumerable<Document>();
        }

        public Document ObterDocumento(string codigo)
        {
            IMongoDatabase db = getDatabase();

            return Find(codigo, db);
        }

        private static Document Find(string codigo, IMongoDatabase db)
        {
            var filter = Builders<Document>.Filter.Eq("_id", MongoDB.Bson.ObjectId.Parse(codigo));

            return db.GetCollection<Document>("Forms")
                .Find(filter)
                .FirstOrDefault();
        }

        public Document SalvarOuAtualizarDocumento(Document documento)
        {
            try
            {
                MongoClient client = new MongoClient(
                       _configuration.GetConnectionString("MongoDB"));
                IMongoDatabase db = client.GetDatabase("et_forms");

                db.GetCollection<Document>("Forms").InsertOne(documento);

                return documento;
            }
            catch (Exception e)
            {

                throw e;
            }
        }

        public async Task<Document> AtualizarDocumentoAsync(Document documento)
        {
            try
            {
                MongoClient client = new MongoClient(
                       _configuration.GetConnectionString("MongoDB"));
                IMongoDatabase db = client.GetDatabase("et_forms");

                var original = Find(documento?.Id, db);
                
                // Manter dados privados
                documento.CropedFields = original.CropedFields;
                documento.DadosOriginais = original.DadosOriginais;

                var filter = Builders<Document>.Filter.Eq("_id", MongoDB.Bson.ObjectId.Parse(documento?.Id));

                //var filter = Builders<Document>.Filter.Eq(s => s.Id, id);
                var result = await db.GetCollection<Document>("Forms").ReplaceOneAsync(filter, documento);

                return documento;
            }
            catch (Exception e)
            {

                throw e;
            }
        }

        public byte[] DownloadFile(MongoDB.Bson.ObjectId id)
        {
            var bucket = new GridFSBucket(getDatabase());
            var file = bucket.DownloadAsBytes(id);
            return file;
        }

        public Document ObterDocumentoById(MongoDB.Bson.ObjectId id)
        {
            IMongoDatabase db = getDatabase();

            var filter = Builders<Document>.Filter.Eq("_id", id);

            return db.GetCollection<Document>("Forms")
                .Find(filter)
                .FirstOrDefault();
        }
    }
}
