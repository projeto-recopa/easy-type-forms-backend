using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using image_cloud_processor.MLModels;
using image_cloud_processor.Models;
using image_cloud_processor.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace document_ml_predict.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PredictController : ControllerBase
    {

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly PredictionMLService _predictionMLService;

        public PredictController(
            PredictionMLService predictionMLService, ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
            _predictionMLService = predictionMLService;
        }

        [HttpGet("sexo/{id}")]
        public ModelOutput GetSexoPrediction(string id)
        {
            try
            {
                _logger.LogInformation($"Get Sexo Prediction for Document: {id}");
                return _predictionMLService.PredictSexoForDocument(id);
                //return new ModelOutput
                //{
                //    Prediction = "Teste"
                //};
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Eror Prediction for Document: {id} - {ex.Message}");
                throw ex;
            }
        }

        [HttpGet("resultado/{id}")]
        public ModelOutput GetResultadoPrediction(string id)
        {
            try
            {
                _logger.LogInformation($"Get Resultado Prediction for Document: {id}");
                return _predictionMLService.PredictResultadoForDocument(id);
                //return new ModelOutput
                //{
                //    Prediction = "Teste"
                //};
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Eror Prediction for Document: {id} - {ex.Message}");
                throw ex;
            }
        }

        [HttpGet("sintomas/{id}")]
        public Sintomas GetSintomasPrediction(string id)
        {
            try
            {
                _logger.LogInformation($"Get Sintomas Prediction for Document: {id}");
                var value = _predictionMLService.PredictSintomaFebreForDocument(id);

                //return new ModelOutput
                //{
                //    Prediction = "Teste"
                //};
                return new Sintomas
                {
                    Febre = (value.Prediction == "SIM")

                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Eror Prediction for Document: {id} - {ex.Message}");
                throw ex;
            }
        }
    }
}
