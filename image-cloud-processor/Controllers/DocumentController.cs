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

namespace image_cloud_processor.Controllers
{
    /// <summary>
    /// Endpoints relacionados ao ciclo de vida do documento.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class DocumentController : ControllerBase
    {
        private readonly ILogger<UploadController> _logger;
        private readonly DocumentService _documentService;

        public DocumentController(ILogger<UploadController> logger,
            DocumentService documentService)
        {
            _logger = logger;
            _documentService = documentService;
        }

        /// <summary>
        /// Lista os documentos de um deterninado status. Se nenhum parâmetro é passado retornar todos os documentos.
        /// </summary>
        /// <param name="status">Código do status</param>
        /// <returns></returns>
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
