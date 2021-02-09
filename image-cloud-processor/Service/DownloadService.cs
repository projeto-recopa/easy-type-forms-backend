using Google.Cloud.Vision.V1;
using image_cloud_processor.Models;
using image_cloud_processor.Repository;
using image_cloud_processor.Utils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using Newtonsoft.Json.Linq;
using recopa_types;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace image_cloud_processor.Service
{
    public class DownloadService
    {
        private readonly ILogger<UploadService> _logger;
        private readonly IDocumentosRepository<Document> _documentosRepository;
        private readonly ImageService _imageService;
        //private readonly PredictionMLService _predictionMLService;
        private readonly string PredictMLEndpoint;
        private static readonly HttpClient client = new HttpClient();

        public DownloadService(ILogger<UploadService> logger,
            ImageService imageService,
            //PredictionMLService predictionMLService, 
            IConfiguration configuration,
            IDocumentosRepository<Document> documentosRepository)
        {
            _logger = logger;
            //_predictionMLService = predictionMLService;
            _documentosRepository = documentosRepository;
            _imageService = imageService;
            PredictMLEndpoint = configuration.GetValue<string>("PredictSexoEndPoint");
        }

        public byte[] DownloadOptionsImage(string id, int field)
        {
            var document = this._documentosRepository.ObterDocumentoById(MongoDB.Bson.ObjectId.Parse(id));
            if (document != null)
            {
                var option = (OptionsField)field;
                if (document.CropedOptionsFields.ContainsKey(option))
                {
                    return this._documentosRepository.DownloadFile(MongoDB.Bson.ObjectId.Parse(document.CropedOptionsFields[option]));
                }
            }
            return null;
        }

    }
}
