using image_cloud_processor.MLModels;
using image_cloud_processor.Models;
using image_cloud_processor.Repository;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.ML;
using MongoDB.Bson;
using recopa_types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace image_cloud_processor.Service
{

    public class PredictionMLService
    {
        private readonly PredictionEnginePool<ModelInput, ModelOutput> _predictionEnginePool;
        private readonly IDocumentosRepository<Document> _documentosRepository;
        private readonly ILogger<PredictionMLService> _logger;
        public PredictionMLService(PredictionEnginePool<ModelInput, ModelOutput> predictionEnginePool,
            IDocumentosRepository<Document> documentosRepository, ILogger<PredictionMLService> logger)
        {
            _logger = logger;
            _predictionEnginePool = predictionEnginePool;
            _documentosRepository = documentosRepository;
        }

        public ModelOutput Predict(ModelInput input)
        {
            return Predict(input, "Field_SexoModel");
        }
        public ModelOutput Predict(ModelInput input, string modelName)
        {
            try
            {

                ModelOutput prediction = _predictionEnginePool.GetPredictionEngine(modelName).Predict(example: input);

                return prediction;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro no motor de predição {ex.Message}");
                throw;
            }
        }

        public ModelOutput PredictSexoForDocument(string id)
        {
            var documento = _documentosRepository.ObterDocumento(id);
            _logger.LogInformation($"Get Prediction for Document found");
            return AplicarModelosML(documento, DocumentField.SEXO, "Field_SexoModel");
        }

        public ModelOutput PredictResultadoForDocument(string id)
        {
            var documento = _documentosRepository.ObterDocumento(id);
            _logger.LogInformation($"Get Prediction for Document found");
            return AplicarModelosML(documento, DocumentField.RESULTADO_TESTE, "Field_ResultadoTesteModel");
        }

        public ModelOutput PredictSintomaFebreForDocument(string id)
        {
            var documento = _documentosRepository.ObterDocumento(id);
            _logger.LogInformation($"Get Prediction for Document found");
            return AplicarModelosML(documento, DocumentField.SINTOMAS, "Field_SintomaFebreModel");
        }

        private MLModels.ModelOutput AplicarModelosML(Document documento, DocumentField field, string model)
        {
            if (documento.CropedFields.ContainsKey(field))
            {
                var id = ObjectId.Parse(documento.CropedFields[field]);
                var bytes = _documentosRepository.DownloadFile(id);
                string path = Path.GetTempFileName();

                using (var ms = new MemoryStream(bytes))
                {
                    using (var fs = new FileStream(path, FileMode.Create))
                    {
                        ms.WriteTo(fs);
                    }
                }

                _logger.LogWarning($"Croped boxes loaded!");

                var result = this.Predict(new MLModels.ModelInput
                {
                    ImageSource = path
                }, model);
                return result;
            }
            _logger.LogWarning($"No croped boxes found!");
            return null;
        }


    }
}
