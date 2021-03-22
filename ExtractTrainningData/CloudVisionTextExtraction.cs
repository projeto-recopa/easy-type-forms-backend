using Google.Cloud.Vision.V1;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

namespace ExtractTrainningData
{
    public class CloudVisionTextExtraction
    {
        public static CropBoxes GetTextExtraction(string item, Bitmap btm)
        {
            var client = ImageAnnotatorClient.Create();
            var image = Google.Cloud.Vision.V1.Image.FromBytes(DrawUtils.ImageToByteArray(btm));
            var response = client.DetectDocumentText(image);

            var cropBoxes = new CropBoxes();
            foreach (var page in response.Pages)
            {
                foreach (var block in page.Blocks)
                {
                    foreach (var paragraph in block.Paragraphs)
                    {
                        var texton1 = string.Empty;
                        var texton2 = string.Empty;
                        foreach (var word in paragraph.Words)
                        {
                            var wordBox = BoundigBoxToPoints(word.BoundingBox);
                            string palavra = string.Join(string.Empty, word.Symbols.Select(x => x.Text));
                            var field = VerificarCampos(palavra, texton1, texton2);
                            cropBoxes.Push(field, wordBox);
                            texton2 = texton1;
                            texton1 = palavra;
                        }
                    }
                }
            }
            return cropBoxes.PopulateBoxes();
        }

        private static string VerificarCampos(string word, string texton1, string texton2)
        {
            if (!string.IsNullOrEmpty(word))
            {
                var lowrCaseWord = word; //.ToLower();
                //if (lowrCaseWord.Contains("Tem")) return DocumentField.TEM_CPF;
                //if (lowrCaseWord.Contains("Estrangeiro")) return DocumentField.ESTRANGEIRO;
                ////if (lowrCaseWord.Contains("profissional")) {
                if (lowrCaseWord.ToLower().Contains("teste"))
                {
                    if (
                        (!string.IsNullOrEmpty(texton1) && texton1.ToLower().Contains("saúde"))
                        ||
                        (!string.IsNullOrEmpty(texton2) && texton2.ToLower().Contains("saúde"))
                        )
                    {
                        return "Profissional saúde";
                    }
                    //if (lowrCaseWord.Contains("saúde")) return DocumentField.PROFISSIONAL_SAUDE;
                    if (
                        (!string.IsNullOrEmpty(texton1) && texton1.ToLower().Contains("segurança"))
                        ||
                        (!string.IsNullOrEmpty(texton2) && texton2.ToLower().Contains("segurança"))
                        )
                    {
                        return "Profissional segurança";
                    }
                    //if (lowrCaseWord.Contains("segurança")) return DocumentField.PROFISSIONAL_SEGURANCA;
                }
                ////}
                //if (lowrCaseWord.Contains("Sexo")) return DocumentField.SEXO;
                //if (lowrCaseWord.Contains("Raça") || lowrCaseWord.Contains("Raca")) return DocumentField.RACA;
                //if (lowrCaseWord.Contains("Sintomas")) return DocumentField.SINTOMAS;
                //if (lowrCaseWord.Contains("Condições")) return DocumentField.CONDICOES;
                if (lowrCaseWord.ToLower().Contains("teste"))
                {
                    if (
                        (!string.IsNullOrEmpty(texton1) && texton1.ToLower().Contains("estado"))
                        ||
                        (!string.IsNullOrEmpty(texton2) && texton2.ToLower().Contains("estado"))
                        )
                    {
                        return "Estado Teste";
                    }

                    if (
                        (!string.IsNullOrEmpty(texton1) && texton1.ToLower().Contains("tipo"))
                        ||
                        (!string.IsNullOrEmpty(texton2) && texton2.ToLower().Contains("tipo"))
                        )
                    {
                        return "Tipo Teste";
                    }

                    //    if (lowrCaseWord.Contains("Estado")) return DocumentField.ESTADO_TESTE;
                    //if (lowrCaseWord.Contains("Tipo")) return DocumentField.TIPO_TESTE;
                }

                //if (lowrCaseWord.Contains("Resultado")) return DocumentField.RESULTADO_TESTE;
                //if (lowrCaseWord.Contains("Classificação")) return DocumentField.CLASSIFICACAO_FINAL;
                //if (lowrCaseWord.Contains("Evolução")) return DocumentField.EVOLUCAO_CASO;
            }

            return word;

        }

        private static Tuple<System.Drawing.PointF, System.Drawing.PointF, System.Drawing.PointF, System.Drawing.PointF> BoundigBoxToPoints(BoundingPoly bounding)
        {
            var v = bounding.Vertices;
            var p0 = new System.Drawing.PointF(v[0].X, v[0].Y);
            var p1 = new System.Drawing.PointF(v[1].X, v[1].Y);
            var p2 = new System.Drawing.PointF(v[2].X, v[2].Y);
            var p3 = new System.Drawing.PointF(v[3].X, v[3].Y);
            return new Tuple<System.Drawing.PointF, System.Drawing.PointF, System.Drawing.PointF, System.Drawing.PointF>(p0, p1, p2, p3);
        }


    }
}
