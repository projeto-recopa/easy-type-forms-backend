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
        private readonly IDocumentosRepository _documentosRepository;

        public DocumentController(ILogger<UploadController> logger,
            IDocumentosRepository documentosRepository,
            DocumentService documentService)
        {
            _logger = logger;
            _documentService = documentService;
            _documentosRepository = documentosRepository;

        }

        // GET: api/<DocumentController>
        [HttpGet]
        public IEnumerable<Document> Get()
        {
            return _documentService.ListarDocumentos();
        }

        // GET api/<DocumentController>/5
        [HttpGet("{id}")]
        public Document Get(string id)
        {
            return _documentosRepository.ObterDocumento<Document>(id);
        }

        [HttpGet("/process/{id}")]
        public string Process(int id)
        {
            /*
                        var jsonpah = @"C:\Users\a.de.melo.pinheiro\Documents\CESAR School\projeto-recopa\api-auth\API Project-64e82001381f.json";
                        //Authenticate to the service by using Service Account
                        var credential = GoogleCredential.FromFile(jsonpah).CreateScoped(ImageAnnotatorClient.DefaultScopes);
                        var channel = new Grpc.Core.Channel(ImageAnnotatorClient.DefaultEndpoint.ToString(), credential.ToChannelCredentials());
            */

            var client = ImageAnnotatorClient.Create();
            var filePath = @"C:\dev\1598451609587.gray.png";
            // Load an image from a local file.
            var image = Image.FromFile(filePath);
            var response = client.DetectDocumentText(image);
            foreach (var page in response.Pages)
            {
                foreach (var block in page.Blocks)
                {
                    foreach (var paragraph in block.Paragraphs)
                    {
                        Console.WriteLine(string.Join("\n", paragraph.Words));
                    }
                }
            }
            return response.Text;
        }


        // POST api/<DocumentController>
        [HttpPost]
        public void Post([FromBody] Document value)
        {
            _documentosRepository.SalvarOuAtualizarDocumento(value);
            /*
            var filePath = @"C:\Users\a.de.melo.pinheiro\Documents\CESAR School\projeto-recopa\poc-processamento-cloudvision\preprocessamento;";
            // Load an image from a local file.
            var image = Image.FromFile(filePath);
            var client = ImageAnnotatorClient.Create();
            var response = client.DetectDocumentText(image);
            foreach (var page in response.Pages)
            {
                foreach (var block in page.Blocks)
                {
                    foreach (var paragraph in block.Paragraphs)
                    {
                        Console.WriteLine(string.Join("\n", paragraph.Words));
                    }
                }
            }
            */
        }

        // PUT api/<DocumentController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<DocumentController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
