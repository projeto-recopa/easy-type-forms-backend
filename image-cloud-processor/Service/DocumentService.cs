using image_cloud_processor.Models;
using image_cloud_processor.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace image_cloud_processor.Service
{
    public class DocumentService
    {
        private readonly ILogger<DocumentService> _logger;
        private readonly IDocumentosRepository<Document> _documentosRepository;
        public DocumentService(ILogger<DocumentService> logger,
            IDocumentosRepository<Document> documentosRepository)
        {
            _logger = logger;
            _documentosRepository = documentosRepository;
        }

        public IEnumerable<Document> ListarDocumentos()
        {
            return _documentosRepository.ListarDocumentos();
        }


        public IEnumerable<Document> ListarDocumentos(StatusDocumento status)
        {
            return _documentosRepository.ListarDocumentos(status);
        }

        public Document SalvarOuAtualizarDocumento(Document documento)
        {
            return _documentosRepository.SalvarOuAtualizarDocumento(documento);
        }
        public async Task<Document> AtualizarDocumentoAsync(Document documento)
        {
            return await _documentosRepository.AtualizarDocumentoAsync(documento);
        }

        public Document Get(string id)
        {
            var doc= _documentosRepository.ObterDocumento(id);

            //ProcessarDadosML(doc);
            return doc;
        }

        public void ProcessarDadosML(Document documento)
        {
            var endpoint = @"https://localhost:5002/Predict/";

            var prediction = Get<MLModels.ModelOutput>($"{endpoint}{documento.Id}");
            documento.Sexo = prediction.Prediction;
        }
        private T Get<T>(string serviceRoute)
        {
            serviceRoute = serviceRoute.Replace(Environment.NewLine, string.Empty).Replace("\"", "'");

            ///Calls the service following the route 
            HttpWebRequest request = null;
            dynamic response = null;

            ///Building the Request
            request = (HttpWebRequest)WebRequest.Create(serviceRoute);
            //MakeUri(serviceRoute));
            request.Method = "GET";
            request.ContentType = "application/json; encoding='utf-8'";


            ///Gets the Response from API
            response = (HttpWebResponse)request.GetResponse();
            if (typeof(T) == typeof(HttpWebResponse))
            {
                return response;
            }
            else
            {
                ///Converts te Response in text to return
                string responseText;
                using (var reader = new System.IO.StreamReader(response.GetResponseStream(), Encoding.UTF8))
                {
                    responseText = reader.ReadToEnd();
                }

                ///Now load the JSON Document
                return JToken.Parse(responseText).ToObject<T>();
            }
        }

    }
}
