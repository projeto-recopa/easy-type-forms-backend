﻿using Google.Cloud.Vision.V1;
using image_cloud_processor.Models;
using image_cloud_processor.Repository;
using image_cloud_processor.Utils;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;

namespace image_cloud_processor.Service
{
    public class UploadService
    {
        private readonly ILogger<UploadService> _logger;
        private readonly IDocumentosRepository<Document> _documentosRepository;
        private readonly ImageService _imageService;
        private readonly PredictionMLService _predictionMLService;
        private static readonly HttpClient client = new HttpClient();

        public UploadService(ILogger<UploadService> logger,
            ImageService imageService,
            PredictionMLService predictionMLService,
            IDocumentosRepository<Document> documentosRepository)
        {
            _logger = logger;
            _predictionMLService = predictionMLService;
            _documentosRepository = documentosRepository;
            _imageService = imageService;
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
            var client = ImageAnnotatorClient.Create();


            var image = Image.FromBytes(streamedFileContent);
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

            var imageEdit = streamedFileContent;
            //imageEdit = _imageService.CreateBoundingBox(imageEdit, boxesWords, System.Drawing.Color.Red);
            //imageEdit = _imageService.CreateBoundingBox(imageEdit, boxesParagraph, System.Drawing.Color.Purple);
            //imageEdit = _imageService.CreateBoundingBox(imageEdit, boxesBlock, System.Drawing.Color.DarkOliveGreen);

            // Salva a imagem no banco
            var attachmentID = this._documentosRepository.AttachFile(streamedFileContent);
            var editID = this._documentosRepository.AttachFile(imageEdit);

            //var cropedSexo = sexoBox != null ? _documentosRepository.AttachFile(_imageService.CropImage(imageEdit, sexoBox, 8, 4)) : MongoDB.Bson.ObjectId.Empty;
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


            var doc = new Document
            {
                Status = StatusDocumento.UPLOAD,
                AttachmentId = attachmentID.ToString(),
                EditedId = editID.ToString(),
                DadosOriginais = blocos.ToArray(),
                CropedFields = cropedImages
            };

            ProcessarDadosOriginais(doc);

            // Cria documento
            doc = _documentosRepository.SalvarOuAtualizarDocumento(doc);

            if (doc != null && doc.Id != null) return doc.Id;
            return ObjectId.Empty.ToString();
        }

        public void ProcessarDadosOriginais(Document documento)
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

            var resultSexo = AplicarModelosML(documento, DocumentField.SEXO, "Field_SexoModel");
            if (resultSexo != null)
            {
                documento.Sexo = resultSexo.Prediction;
            }

            var resultTeste = AplicarModelosML(documento, DocumentField.RESULTADO_TESTE, "Field_ResultadoTesteModel");
            if (resultTeste != null)
            {
                documento.ResultadoTeste = resultTeste.Prediction;
            }

            AplicarModelosSintomaFebreML(documento, DocumentField.SINTOMAS);

            documento.Status = StatusDocumento.PROCESSADO;
        }

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

                var result = this._predictionMLService.Predict(new MLModels.ModelInput
                {
                    ImageSource = path
                }, model);
                return result;
                //if (result != null)
                //{
                //    documento.Sexo = result.Prediction;
                //}
            }
            return null;
        }
        private void AplicarModelosSintomaFebreML(Document documento, DocumentField field)
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

                var result = this._predictionMLService.Predict(new MLModels.ModelInput
                {
                    ImageSource = path
                }, "Field_SintomaFebreModel");
                documento.Sintomas = new Sintomas();
                if (result != null)
                {
                    documento.Sintomas.Febre = (result.Prediction == "SIM");
                }
            }
        }

        private string ExtractFieldByRegex(string fulltext, string pattern, string field)
        {
            RegexOptions options = RegexOptions.IgnoreCase;

            Regex regex = new Regex(pattern, options);
            Match match = regex.Match(fulltext);

            if (match.Groups.Count > 0)
                return match.Groups[field].Value;

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

        public byte[] DownloadImage(string id, int edited = 0)
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
            this.ProcessarDadosOriginais(document);
        }
    }
}
