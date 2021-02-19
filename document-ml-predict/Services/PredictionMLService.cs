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
                string path = CreateLocalFile(id);

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

        private string CreateLocalFile(ObjectId id)
        {
            var bytes = _documentosRepository.DownloadFile(id);
            string path = Path.GetTempFileName();

            using (var ms = new MemoryStream(bytes))
            {
                using (var fs = new FileStream(path, FileMode.Create))
                {
                    ms.WriteTo(fs);
                }
            }

            return path;
        }

        public async Task PredictOptionsFieldsAsync(string id)
        {
            var documento = _documentosRepository.ObterDocumento(id);

            if(documento.CropedOptionsFields?.Count > 0)
            {
                foreach (var key in documento.CropedOptionsFields.Keys)
                {
                    var cropId = documento.CropedOptionsFields[key];
                    string path = CreateLocalFile(ObjectId.Parse(cropId));

                    _logger.LogWarning($"Croped boxes {key} loaded!");

                    var result = this.Predict(new MLModels.ModelInput
                    {
                        ImageSource = path
                    }, "OptionsField");

                    UpdateOptionField(documento, key, result.Prediction);
                }
            }

            await _documentosRepository.AtualizarDocumentoAsync(documento);
        }

        private void UpdateOptionField(Document documento, OptionsField field, string prediction)
        {
                    bool selecao = (prediction.ToLower() == "marcado");
            switch (field)
            {
                case OptionsField.TEM_CPF_SIM:
                    documento.PossuiCPF = selecao;
                    break;
                case OptionsField.TEM_CPF_NAO:
                    documento.PossuiCPF = !selecao;
                    break;
                case OptionsField.ESTRANGEIRO_SIM:
                    documento.Estrangeiro = selecao;
                    break;
                case OptionsField.ESTRANGEIRO_NAO:
                    documento.Estrangeiro = !selecao;
                    break;
                case OptionsField.SEXO_MASC:
                    documento.Sexo = selecao ? "Masculino" : "Feminino";
                    break;
                case OptionsField.SEXO_FEM:
                    documento.Sexo = !selecao ? "Masculino" : "Feminino";
                    break;
                case OptionsField.EVOLUCAO_CASO_CANCELADO:
                    documento.EvolucaoCaso = documento.EvolucaoCaso ?? new EvolucaoCaso { };
                    documento.EvolucaoCaso.Cancelado = selecao;
                    break;
                case OptionsField.EVOLUCAO_CASO_CURA:
                    documento.EvolucaoCaso = documento.EvolucaoCaso ?? new EvolucaoCaso { };
                    documento.EvolucaoCaso.Cura = selecao;
                    break;
                case OptionsField.EVOLUCAO_CASO_IGNORADO:
                    documento.EvolucaoCaso = documento.EvolucaoCaso ?? new EvolucaoCaso { };
                    documento.EvolucaoCaso.Ignorado = selecao;
                    break;
                case OptionsField.EVOLUCAO_CASO_INTERNADO:
                    documento.EvolucaoCaso = documento.EvolucaoCaso ?? new EvolucaoCaso { };
                    documento.EvolucaoCaso.Internado = selecao;
                    break;
                case OptionsField.EVOLUCAO_CASO_OBITO:
                    documento.EvolucaoCaso = documento.EvolucaoCaso ?? new EvolucaoCaso { };
                    documento.EvolucaoCaso.Obito = selecao;
                    break;
                case OptionsField.EVOLUCAO_CASO_TRATAMENTO:
                    documento.EvolucaoCaso = documento.EvolucaoCaso ?? new EvolucaoCaso { };
                    documento.EvolucaoCaso.TratamentoDomiciliar = selecao;
                    break;
                case OptionsField.EVOLUCAO_CASO_UTI:
                    documento.EvolucaoCaso = documento.EvolucaoCaso ?? new EvolucaoCaso { };
                    documento.EvolucaoCaso.InternadoUTI = selecao;
                    break;
                case OptionsField.RESULTADO_TESTE_POSITIVO:
                    documento.ResultadoTeste = selecao ? "Positivo" : "Negativo";
                    break;
                case OptionsField.RESULTADO_TESTE_NEGATIVO:
                    documento.ResultadoTeste = !selecao ? "Positivo" : "Negativo";
                    break;
                case OptionsField.RACA_AMARELA:
                    documento.Raca = selecao ? "Amarela" : documento.Raca;
                    break;
                case OptionsField.RACA_BRANCA:
                    documento.Raca = selecao ? "Branca" : documento.Raca;
                    break;
                case OptionsField.RACA_INDIGENA:
                    documento.Raca = selecao ? "Indigena" : documento.Raca;
                    break;
                case OptionsField.RACA_PARDA:
                    documento.Raca = selecao ? "Parda" : documento.Raca;
                    break;
                case OptionsField.RACA_PRETA:
                    documento.Raca = selecao ? "Preta" : documento.Raca;
                    break;
                case OptionsField.SINTOMAS_DISPNEIA:
                    documento.Sintomas = documento.Sintomas ?? new Sintomas { };
                    documento.Sintomas.Dispneia = selecao;
                    break;
                case OptionsField.SINTOMAS_DOR_GARGANTA:
                    documento.Sintomas = documento.Sintomas ?? new Sintomas { };
                    documento.Sintomas.DorGarganta = selecao;
                    break;
                case OptionsField.SINTOMAS_FEBRE:
                    documento.Sintomas = documento.Sintomas ?? new Sintomas { };
                    documento.Sintomas.Febre = selecao;
                    break;
                case OptionsField.SINTOMAS_OUTROS:
                    documento.Sintomas = documento.Sintomas ?? new Sintomas { };
                    documento.Sintomas.Outros = selecao;
                    break;
                case OptionsField.SINTOMAS_TOSSE:
                    documento.Sintomas = documento.Sintomas ?? new Sintomas { };
                    documento.Sintomas.Tosse = selecao;
                    break;
                case OptionsField.CONDICOES_DIABETES:
                    documento.Condicoes = documento.Condicoes ?? new Condicoes { };
                    documento.Condicoes.Diabetes = selecao;
                    break;
                case OptionsField.CONDICOES_DOENCAS_CARDIACAS:
                    documento.Condicoes = documento.Condicoes ?? new Condicoes { };
                    documento.Condicoes.DoencasCardiacas = selecao;
                    break;
                case OptionsField.CONDICOES_DOENCAS_CROMOSSOMICA:
                    documento.Condicoes = documento.Condicoes ?? new Condicoes { };
                    documento.Condicoes.DoencasCromossomicas = selecao;
                    break;
                case OptionsField.CONDICOES_DOENCAS_RENAIS:
                    documento.Condicoes = documento.Condicoes ?? new Condicoes { };
                    documento.Condicoes.DoencasRenais = selecao;
                    break;
                case OptionsField.CONDICOES_DOENCAS_RESPIRATORIAS:
                    documento.Condicoes = documento.Condicoes ?? new Condicoes { };
                    documento.Condicoes.DoencasRespiratorias = selecao;
                    break;
                case OptionsField.CONDICOES_GESTANTE:
                    documento.Condicoes = documento.Condicoes ?? new Condicoes { };
                    documento.Condicoes.Gestantes = selecao;
                    break;
                case OptionsField.CONDICOES_IMUNOSSUPRESSAO:
                    documento.Condicoes = documento.Condicoes ?? new Condicoes { };
                    documento.Condicoes.Imunossupressao = selecao;
                    break;
                default:
                    break;
            }
        }

    }
}
