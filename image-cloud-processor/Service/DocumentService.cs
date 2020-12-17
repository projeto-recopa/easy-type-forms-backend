using image_cloud_processor.Models;
using image_cloud_processor.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace image_cloud_processor.Service
{
    public class DocumentService
    {
        private readonly ILogger<DocumentService> _logger;
        private readonly IDocumentosRepository _documentosRepository;
        public DocumentService(ILogger<DocumentService> logger,
            IDocumentosRepository documentosRepository)
        {
            _logger = logger;
            _documentosRepository = documentosRepository;
        }

        public IEnumerable<Document> ListarDocumentos()
        {
            return _documentosRepository.ListarDocumentos<Document>();
        }
    }
}
