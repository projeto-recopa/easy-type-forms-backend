using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Vision.V1;
using Grpc.Auth;
using image_cloud_processor.Models;
using image_cloud_processor.Repository;
using image_cloud_processor.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace image_cloud_processor.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocumentController : ControllerBase
    {
        private readonly ILogger<UploadController> _logger;
        private readonly DocumentService _documentService;
        private readonly IDocumentosRepository<Document> _documentosRepository;

        public DocumentController(ILogger<UploadController> logger,
            IDocumentosRepository<Document> documentosRepository,
            DocumentService documentService)
        {
            _logger = logger;
            _documentService = documentService;
            _documentosRepository = documentosRepository;

        }

        // GET: api/<DocumentController>
        [HttpGet]
        public IEnumerable<Document> Get([FromQuery(Name = "status")] int status = -1)
        {
            if (status > -1)
            {
                if (!Enum.IsDefined(typeof(StatusDocumento), status))
                {
                    throw new ArgumentException("Parâmetro inválido", "Status do documento");
                }
                return _documentService.ListarDocumentos((StatusDocumento)status);

            }
            return _documentService.ListarDocumentos();
        }

        // GET api/<DocumentController>/5
        [HttpGet("{id}")]
        public Document Get(string id)
        {
            //return _documentosRepository.ObterDocumento(id);
            return _documentService.Get(id);
        }

        [HttpGet("/process/{id}")]
        public string Process(int id)
        {
            // TODO: Mover para cloud image processor 
            return "Not implemented;";
        }


        // POST api/<DocumentController>
        [HttpPost]
        public Document Post([FromBody] Document value)
        {
            return _documentService.SalvarOuAtualizarDocumento(value);
        }

        // PUT api/<DocumentController>/5
        [HttpPut("{id}")]
        public async Task<Document> PutAsync([FromBody] Document value)
        {
            return await _documentService.AtualizarDocumentoAsync(value);
        }

        // DELETE api/<DocumentController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
