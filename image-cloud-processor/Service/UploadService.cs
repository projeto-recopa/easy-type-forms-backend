using image_cloud_processor.Models;
using image_cloud_processor.Repository;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace image_cloud_processor.Service
{
    public class UploadService
    {
        private readonly ILogger<UploadService> _logger;
        private readonly IDocumentosRepository _documentosRepository;
        public UploadService(ILogger<UploadService> logger,
            IDocumentosRepository documentosRepository)
        {
            _logger = logger;
            _documentosRepository = documentosRepository;
        }

        public ObjectId CreateDocumentFromFile(byte[] streamedFileContent)
        {
            var attachmentID = this._documentosRepository.AttachFile(streamedFileContent);

            var doc = _documentosRepository.SalvarOuAtualizarDocumento(new Document
            {
                Status = StatusDocumento.UPLOAD,
                attachmentId = attachmentID
            });

            if (doc != null && doc._id != null) return doc._id;
            return ObjectId.Empty;
        }

    }
}
