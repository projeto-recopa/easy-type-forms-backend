using recopa_types;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace image_cloud_processor.Utils
{
    //public enum DocumentField
    //{
    //    NULL = 0,
    //    TEM_CPF,
    //    ESTRANGEIRO,
    //    PROFISSIONAL_SAUDE,
    //    PROFISSIONAL_SEGURANCA,
    //    SEXO,
    //    RACA,
    //    //RACA_BRANCA,
    //    //RACA_PRETA,
    //    //RACA_AMARELA,
    //    //RACA_PARDA,
    //    //RACA_INDIGENA,
    //    SINTOMAS,
    //    CONDICOES,
    //    ESTADO_TESTE,
    //    TIPO_TESTE,
    //    RESULTADO_TESTE,
    //    CLASSIFICACAO_FINAL,
    //    EVOLUCAO_CASO,
    //}
    public class CropBoxes
    {
        private Dictionary<DocumentField, Tuple<PointF, PointF, PointF, PointF>> _boxes = new Dictionary<DocumentField, Tuple<PointF, PointF, PointF, PointF>>();

        private void Push(DocumentField field, Tuple<PointF, PointF, PointF, PointF> box)
        {
            if (field == DocumentField.NULL) return;

            if (!this._boxes.ContainsKey(field))
            {
                this._boxes.Add(field, box);
            }
        }

        public Dictionary<DocumentField, Tuple<PointF, PointF, PointF, PointF>>.KeyCollection GetBoxes()
        {
            return this._boxes.Keys;
        }

        public Tuple<PointF, PointF, PointF, PointF> GetBox(DocumentField field)
        {
            return this._boxes[field];
        }

        private DocumentField GetFieldByText(string word)
        {
            if (!string.IsNullOrEmpty(word))
            {
                var lowrCaseWord = word; //.ToLower();
                if (lowrCaseWord.Contains("Tem")) return DocumentField.TEM_CPF;
                if (lowrCaseWord.Contains("Estrangeiro")) return DocumentField.ESTRANGEIRO;
                //if (lowrCaseWord.Contains("profissional") && lowrCaseWord.Contains("saúde")) return DocumentField.PROFISSIONAL_SAUDE;
                //if (lowrCaseWord.Contains("profissional") && lowrCaseWord.Contains("segurança")) return DocumentField.PROFISSIONAL_SEGURANCA;
                if (lowrCaseWord.Contains("Sexo")) return DocumentField.SEXO;
                if (lowrCaseWord.Contains("Raça") || lowrCaseWord.Contains("Raca")) return DocumentField.RACA;
                if (lowrCaseWord.Contains("Sintomas")) return DocumentField.SINTOMAS;
                if (lowrCaseWord.Contains("Condições")) return DocumentField.CONDICOES;
                if (lowrCaseWord.Contains("Estado")) return DocumentField.ESTADO_TESTE;
                if (lowrCaseWord.Contains("Tipo")) return DocumentField.TIPO_TESTE;
                if (lowrCaseWord.Contains("Resultado")) return DocumentField.RESULTADO_TESTE;
                if (lowrCaseWord.Contains("Classificação")) return DocumentField.CLASSIFICACAO_FINAL;
                if (lowrCaseWord.Contains("Evolução")) return DocumentField.EVOLUCAO_CASO;
            }

            return DocumentField.NULL;
        }

        public void Push(string word, Tuple<PointF, PointF, PointF, PointF> box)
        {
            Push(GetFieldByText(word), box);
        }

        public static Tuple<float, float> GetFieldDimension(DocumentField field)
        {
            switch (field)
            {
                //case DocumentField.NULL:
                //    break;
                case DocumentField.TEM_CPF:
                    return new Tuple<float, float>(5f, 4f);
                case DocumentField.ESTRANGEIRO:
                    return new Tuple<float, float>(2f, 4f);
                case DocumentField.PROFISSIONAL_SAUDE:
                    break;
                case DocumentField.PROFISSIONAL_SEGURANCA:
                    break;
                case DocumentField.SEXO:
                    return new Tuple<float, float>(8f, 4f);
                case DocumentField.RACA:
                    return new Tuple<float, float>(9.4f, 4f);
                case DocumentField.SINTOMAS:
                    return new Tuple<float, float>(5.4f, 4f);
                case DocumentField.CONDICOES:
                    return new Tuple<float, float>(12.0f, 5f);
                case DocumentField.ESTADO_TESTE:
                    return new Tuple<float, float>(1.4f, 6f);
                case DocumentField.TIPO_TESTE:
                    return new Tuple<float, float>(10f, 7f);
                case DocumentField.RESULTADO_TESTE:
                    return new Tuple<float, float>(2f, 5f);
                case DocumentField.CLASSIFICACAO_FINAL:
                    return new Tuple<float, float>(6f, 6f);
                case DocumentField.EVOLUCAO_CASO:
                    return new Tuple<float, float>(5f, 6f);
                default:
                    break;
            }
            return new Tuple<float, float>(1f, 1f);
        }
    }
}
