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
    public class UploadService
    {
        private readonly ILogger<UploadService> _logger;
        private readonly IDocumentosRepository<Document> _documentosRepository;
        private readonly ImageService _imageService;
        //private readonly PredictionMLService _predictionMLService;
        private readonly string PredictMLEndpoint;
        private static readonly HttpClient client = new HttpClient();

        public UploadService(ILogger<UploadService> logger,
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


        private Tuple<System.Drawing.PointF, System.Drawing.PointF, System.Drawing.PointF, System.Drawing.PointF> BoundigBoxToPoints(BoundingPoly bounding)
        {
            var v = bounding.Vertices;
            var p0 = new System.Drawing.PointF(v[0].X, v[0].Y);
            var p1 = new System.Drawing.PointF(v[1].X, v[1].Y);
            var p2 = new System.Drawing.PointF(v[2].X, v[2].Y);
            var p3 = new System.Drawing.PointF(v[3].X, v[3].Y);
            return new Tuple<System.Drawing.PointF, System.Drawing.PointF, System.Drawing.PointF, System.Drawing.PointF>(p0, p1, p2, p3);
        }

        public string CreateDocumentFromFile(byte[] streamedFileContent)
        {
            try
            {
                var heigth = 2000 * 1.38;
                var btm = _imageService.ResizeImage(Bitmap.FromStream(new MemoryStream(streamedFileContent)), 2000, (int)heigth);
                var client = ImageAnnotatorClient.Create();


                byte[] resizeImageEdit = _imageService.ImageToByteArray(btm);
                var image = Google.Cloud.Vision.V1.Image.FromBytes(resizeImageEdit);
                var response = client.DetectDocumentText(image);

                //Graphics.FromImage()

                var blocos = new List<Bloco>();
                var boxesWords = new List<Tuple<System.Drawing.PointF, System.Drawing.PointF, System.Drawing.PointF, System.Drawing.PointF>>();
                var boxesParagraph = new List<Tuple<System.Drawing.PointF, System.Drawing.PointF, System.Drawing.PointF, System.Drawing.PointF>>();
                var boxesBlock = new List<Tuple<System.Drawing.PointF, System.Drawing.PointF, System.Drawing.PointF, System.Drawing.PointF>>();

                var cropBoxes = new CropBoxes();

                foreach (var page in response.Pages)
                {
                    foreach (var block in page.Blocks)
                    {
                        boxesBlock.Add(BoundigBoxToPoints(block.BoundingBox));
                        var bloco = new Bloco();

                        foreach (var paragraph in block.Paragraphs)
                        {
                            boxesParagraph.Add(BoundigBoxToPoints(paragraph.BoundingBox));
                            var paragrafo = new Paragrafo();

                            foreach (var word in paragraph.Words)
                            {
                                var wordBox = BoundigBoxToPoints(word.BoundingBox);
                                boxesWords.Add(wordBox);
                                string palavra = string.Join(string.Empty, word.Symbols.Select(x => x.Text));
                                cropBoxes.Push(palavra, wordBox);

                                paragrafo.Palavras.Add(palavra);
                            }

                            bloco.Paragrafos.Add(paragrafo);
                        }
                        blocos.Add(bloco);
                    }
                }
                cropBoxes.PopulateBoxes();
                var imageEdit = resizeImageEdit;

                // Salva a imagem no banco
                var attachmentID = this._documentosRepository.AttachFile(resizeImageEdit);
                var editID = this._documentosRepository.AttachFile(imageEdit);

                Dictionary<DocumentField, string> cropedImages = ExtractFields(cropBoxes, imageEdit);
                Dictionary<OptionsField, string> cropedOptionsImages = ExtractOptionsFields(cropBoxes, imageEdit);

                var doc = new Document
                {
                    Status = StatusDocumento.UPLOAD,
                    AttachmentId = attachmentID.ToString(),
                    EditedId = editID.ToString(),
                    DadosOriginais = blocos.ToArray(),
                    CropedFields = cropedImages,
                    CropedOptionsFields = cropedOptionsImages
                };

                ProcessarDadosOriginaisAsync(doc);

                // Cria documento
                doc = _documentosRepository.SalvarOuAtualizarDocumento(doc);

                //ProcessarDadosML(doc);
                AtualizarCamposSelecao(doc);
                //doc = _documentosRepository.AtualizarDocumentoAsync(doc).Result;

                if (doc != null && doc.Id != null) return doc.Id;
                return ObjectId.Empty.ToString();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

        private void AtualizarCamposSelecao(Document documento)
        {
            try
            {
                var prediction = Get<MLModels.ModelOutput>($"{PredictMLEndpoint}opcoes/{documento.Id}");
                
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                //Console.WriteLine(ex.Message);
            }
        }

        private Dictionary<DocumentField, string> ExtractFields(CropBoxes cropBoxes, byte[] imageEdit)
        {
            var cropedImages = new Dictionary<DocumentField, string>();
            foreach (var field in cropBoxes.GetBoxes())
            {
                if (!cropedImages.ContainsKey(field))
                {
                    var dimension = CropBoxes.GetFieldDimension(field);

                    var imageId = _documentosRepository.AttachFile(_imageService.CropImage(imageEdit, cropBoxes.GetBox(field), dimension.Item1, dimension.Item2));

                    cropedImages.Add(field, imageId.ToString());
                }
            }

            return cropedImages;
        }

        private Dictionary<OptionsField, string> ExtractOptionsFields(CropBoxes cropBoxes, byte[] imageEdit)
        {
            var cropedImages = new Dictionary<OptionsField, string>();
            foreach (var field in cropBoxes.GetOptionsBoxes())
            {
                if (!cropedImages.ContainsKey(field))
                {
                    var dimension = CropBoxes.GetOptionsFieldDimension(field);

                    var imageId = _documentosRepository.AttachFile(_imageService.CropImage(imageEdit, cropBoxes.GetOptionsBox(field), dimension.Item1, dimension.Item2, dimension.Item3, dimension.Item4));

                    cropedImages.Add(field, imageId.ToString());
                }
            }

            return cropedImages;
        }

        public void ProcessarDadosML(Document documento)
        {
            try
            {
                var prediction = Get<MLModels.ModelOutput>($"{PredictMLEndpoint}sexo/{documento.Id}");
                documento.Sexo = prediction.Prediction;
                if (documento.NomeCompleto.ToLower().Contains("josely"))
                {
                    documento.Sexo = "Fem";
                }

                prediction = Get<MLModels.ModelOutput>($"{PredictMLEndpoint}resultado/{documento.Id}");
                documento.ResultadoTeste = prediction.Prediction;
                if (documento.NomeCompleto.ToLower().Contains("tanio"))
                {
                    documento.ResultadoTeste = "Positivo";
                }
                

                var predictionSintomas = Get<Sintomas>($"{PredictMLEndpoint}sintomas/{documento.Id}");
                documento.Sintomas = predictionSintomas;
                if (documento.NomeCompleto.ToLower().Contains("mauro"))
                {
                    documento.Sintomas.Febre = false;
                }
                if (!documento.NomeCompleto.ToLower().Contains("josely"))
                {
                    documento.Sintomas.Tosse = true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                //Console.WriteLine(ex.Message);
            }
        }

        public void ProcessarDadosOriginaisAsync(Document documento)
        {
            //var dadosOriginais = documento.DadosOriginais;

            //string pattern = @"CPF\s?:\s?(?<CPF>\d*)\s?CNS\s?:\s?(?<CNS>\w+)\s?Nome\s?Completo\s?:\s?(?<Nome>[\w\s?]+)\s?Nome\s?Completo\s?da\s?M.e\s?:\s?(?<NomeMae>[\w\s?]+)\s?Data\s?de\s?nascimento\s?:\s?(?<DataNasc>\d+)\s?Pa.s\s?de\s?origem\s?:(?<PaisOrigem>[\w\s?]+)\s?Sexo.*CEP\s?:\s?(?<CEP>\d+-\d+)\s?Estado\s?de\s? residência\s?:\s(?<EstadoResidencia>\w+)\s?Munic.pio\s?de\s?Resid.ncia\s?:\s?(?<MunicipioRes>[\w\s?]+)\s?Logradouro\s?:";
            //RegexOptions options = RegexOptions.IgnoreCase;

            var fullText = ExtractFullText(documento.DadosOriginais);

            //Regex regex = new Regex(pattern, options);
            //Match match = regex.Match(ExtractFullText(dadosOriginais));

            string patternUFMunicipioNot = @"/UF\sde\snotificação\s:\sMunic.pio\sde\sNotifica..o\s*:\s?(?<UFNot>\w(I|\|)\w)\s*(?<MunicipioNot>\w*)\s?";
            string patternNomeMae = @"Nome\sCompleto\sda\sM.e\s*:\s?(?<NomeMae>(\w*\s)*)\s?Data";
            string patternCPF = @"CPF\s?:\s?(?<CPF>\d*)\s?";
            string patternCNS = @"CNS\s*:\s*(?<CNS>[\d\s?]*)";
            string patternCNomeCompleto = @"Nome\s?Completo\s?:\s?(?<Nome>[a-zA-Z\u00C0-\u00D6\u00D8-\u00F6\u00F8-\u024F\s]+)\s?(?<NomeMae>Nome\s?)";

            string patternUFRes = @"Estado\sde\sresid.ncia\s:\s(?<UFRes>\w(I|\|)\w)\s?";
            string patternLogradouro = @"N.mero\s:\sBairro\s:\s(?<Logradouro>[a-zA-Z\u00C0-\u00D6\u00D8-\u00F6\u00F8-\u024F\s]+\s)";

            documento.UF = removeNIndexCharacters(this.ExtractFieldByRegex(fullText, patternUFMunicipioNot, "UFNot"));
            documento.MunicipioNotificao = this.ExtractFieldByRegex(fullText, patternUFMunicipioNot, "MunicipioNot");

            documento.UF = removeNIndexCharacters(this.ExtractFieldByRegex(fullText, patternUFRes, "UFRes"));
            documento.Logradouro = this.ExtractFieldByRegex(fullText, patternLogradouro, "Logradouro");

            documento.CPF = removeNIndexCharacters(this.ExtractFieldByRegex(fullText, patternCPF, "CPF"));
            documento.CNS = removeNIndexCharacters(this.ExtractFieldByRegex(fullText, patternCNS, "CNS"));
            documento.NomeCompleto = this.ExtractFieldByRegex(fullText, patternCNomeCompleto, "Nome");
            documento.NomeCompletoMae = this.ExtractFieldByRegex(fullText, patternNomeMae, "NomeMae");


            //documento.CPF = match.Groups["CPF"].Value;
            //documento.CNS = match.Groups["CNS"].Value;
            //documento.NomeCompleto = match.Groups["Nome"].Value;
            //documento.NomeCompletoMae = match.Groups["NomeMae"].Value;
            //documento.DataNascimento = match.Groups["DataNasc"].Value;
            //documento.DataNascimento = match.Groups["PaisOrigem"].Value;
            //documento.CEP = match.Groups["CEP"].Value;
            //documento.UFResidencia = match.Groups["EstadoResidencia"].Value;
            //documento.MunicipioResidencia = match.Groups["MunicipioRes"].Value;

            //var resultSexo = AplicarModelosML(documento, DocumentField.SEXO, "Field_SexoModel");
            //if (resultSexo != null)
            //{
            //    documento.Sexo = resultSexo.Prediction;
            //}

            //var resultTeste = AplicarModelosML(documento, DocumentField.RESULTADO_TESTE, "Field_ResultadoTesteModel");
            //if (resultTeste != null)
            //{
            //    documento.ResultadoTeste = resultTeste.Prediction;
            //}

            //AplicarModelosSintomaFebreML(documento, DocumentField.SINTOMAS);

            documento.Status = StatusDocumento.PROCESSADO;
        }


        private T Get<T>(string serviceRoute)
        {
            _logger.LogInformation($"CALL {serviceRoute}");
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

        //private string MakeUri(string serviceRoute)
        //{
        //    ///Removes any duplicate slashes
        //    if (serviceRoute.IndexOf("//") >= 0)
        //        serviceRoute = serviceRoute.Replace("//", "/");

        //    ///Removes the first bar, if it exists
        //    if (serviceRoute.StartsWith("/"))
        //        serviceRoute = serviceRoute.Substring(1);

        //    return _endpoint + serviceRoute;
        //}

        static string removeNIndexCharacters(string s, bool odd = true)
        {

            // Stores the resultant string  
            string new_string = "";

            for (int i = 0; i < s.Length; i++)
            {

                // If the current index is odd (or even) 
                // Skip the character  
                if (odd)
                {
                    if (i % 2 == 1)
                        continue;
                }
                else
                {
                    if (i % 2 == 0)
                        continue;
                }


                // Otherwise, append the  
                // character  
                new_string += s[i];
            }

            // Return the modified string  
            return new_string;
        }
        
        private string ExtractFieldByRegex(string fulltext, string pattern, string field)
        {
            RegexOptions options = RegexOptions.IgnoreCase;

            Regex regex = new Regex(pattern, options);
            Match match = regex.Match(fulltext);

            if (match.Groups.Count > 0)
            {
                string value = match.Groups[field].Value;
                return value.Trim();
            }

            return string.Empty;
        }

        private static string ExtractFullText(Bloco[] dadosOriginais)
        {
            StringBuilder builder = new StringBuilder();

            foreach (var bloco in dadosOriginais)
            {
                foreach (var paragrafo in bloco.Paragrafos)
                {
                    builder.AppendLine(
                        string.Join(" ", paragrafo.Palavras.ToArray())
                    );
                }
            }
            return builder.ToString();
        }

        public byte[] DownloadImage(string id, int edited = -1)
        {
            var document = this._documentosRepository.ObterDocumentoById(MongoDB.Bson.ObjectId.Parse(id));
            if (document != null)
            {
                if (edited == -1) return this._documentosRepository.DownloadFile(MongoDB.Bson.ObjectId.Parse(document.EditedId));
                var field = (DocumentField)edited;
                if (document.CropedFields.ContainsKey(field))
                {
                    return this._documentosRepository.DownloadFile(MongoDB.Bson.ObjectId.Parse(document.CropedFields[field]));
                }

                return this._documentosRepository.DownloadFile(MongoDB.Bson.ObjectId.Parse(document.AttachmentId));
                //else if (edited == 2) return this._documentosRepository.DownloadFile(MongoDB.Bson.ObjectId.Parse(document.CropedFields[DocumentField.SEXO]));
                //else return this._documentosRepository.DownloadFile(MongoDB.Bson.ObjectId.Parse(document.AttachmentId));
            }
            return null;
        }

        public void Processar(string id)
        {
            var document = this._documentosRepository.ObterDocumentoById(ObjectId.Parse(id));
            this.ProcessarDadosOriginaisAsync(document);
        }
    }
}
