using recopa_types;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace image_cloud_processor.Utils
{
    public class CropBoxes
    {
        private Dictionary<DocumentField, Tuple<PointF, PointF, PointF, PointF>> _boxes = new Dictionary<DocumentField, Tuple<PointF, PointF, PointF, PointF>>();

        private Dictionary<OptionsField, Tuple<PointF, PointF, PointF, PointF>> option_boxes = new Dictionary<OptionsField, Tuple<PointF, PointF, PointF, PointF>>();

        private Dictionary<string, Tuple<PointF, PointF, PointF, PointF>> all_boxes = new Dictionary<string, Tuple<PointF, PointF, PointF, PointF>>();

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

        public Dictionary<OptionsField, Tuple<PointF, PointF, PointF, PointF>>.KeyCollection GetOptionsBoxes()
        {
            return this.option_boxes.Keys;
        }

        public Tuple<PointF, PointF, PointF, PointF> GetBox(DocumentField field)
        {
            return this._boxes[field];
        }

        public Tuple<PointF, PointF, PointF, PointF> GetOptionsBox(OptionsField field)
        {
            return this.option_boxes[field];
        }

        private void PushSelected(DocumentField field, Tuple<PointF, PointF, PointF, PointF> box)
        {
            if (field == DocumentField.NULL) return;

            if (!this._boxes.ContainsKey(field))
            {
                this._boxes.Add(field, box);
            }
        }
        private void PushOption(OptionsField field, Tuple<PointF, PointF, PointF, PointF> box)
        {
            if (field == OptionsField.NULL) return;

            if (!this.option_boxes.ContainsKey(field))
            {
                this.option_boxes.Add(field, box);
            }
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
            //Push(GetFieldByText(word), box);
            PushAll(word, box);
        }
        private void PushAll(string word, Tuple<PointF, PointF, PointF, PointF> box)
        {
            if (!this.all_boxes.ContainsKey(word))
            {
                this.all_boxes.Add(word, box);
            }
        }

        public CropBoxes PopulateBoxes()
        {
            foreach (var item in this.all_boxes.Keys)
            {
                PushSelected(GetFieldByText(item), this.all_boxes[item]);
            }

            foreach (var item in this._boxes.Keys)
            {
                switch (item)
                {
                    case DocumentField.TEM_CPF:
                        FindContainedOption(OptionsField.TEM_CPF_SIM, this._boxes[item], GetFieldDimension(DocumentField.TEM_CPF));
                        FindContainedOption(OptionsField.TEM_CPF_NAO, this._boxes[item], GetFieldDimension(DocumentField.TEM_CPF));
                        break;

                    case DocumentField.ESTRANGEIRO:
                        FindContainedOption(OptionsField.ESTRANGEIRO_SIM, this._boxes[item], GetFieldDimension(DocumentField.ESTRANGEIRO));
                        FindContainedOption(OptionsField.ESTRANGEIRO_NAO, this._boxes[item], GetFieldDimension(DocumentField.ESTRANGEIRO));
                        break;

                    case DocumentField.SEXO:
                        FindContainedOption(OptionsField.SEXO_MASC, this._boxes[item], GetFieldDimension(DocumentField.SEXO));
                        FindContainedOption(OptionsField.SEXO_FEM, this._boxes[item], GetFieldDimension(DocumentField.SEXO));
                        break;
                    default: break;
                }
            }
            return this;
        }

        private void FindContainedOption(OptionsField option, Tuple<PointF, PointF, PointF, PointF> box, Tuple<float, float> dimension)
        {
            foreach (var item in this.all_boxes.Keys)
            {
                if (item.ToLower().Contains(GetWordFromOption(option)) &&
                    IsInsidePolygon(
                    this.all_boxes[item],
                    box, dimension))
                {
                    PushOption(option,
                    this.all_boxes[item]);
                }
            }
        }

        private string GetWordFromOption(OptionsField options)
        {
            switch (options)
            {
                case OptionsField.TEM_CPF_SIM: return "sim";
                case OptionsField.TEM_CPF_NAO: return "não";
                case OptionsField.ESTRANGEIRO_SIM: return "sim";
                case OptionsField.ESTRANGEIRO_NAO: return "não";
                case OptionsField.SEXO_MASC: return "masculino";
                case OptionsField.SEXO_FEM: return "feminino";
                default: return string.Empty;
            }

        }

        private bool IsInsidePolygon(Tuple<PointF, PointF, PointF, PointF> tuple, Tuple<PointF, PointF, PointF, PointF> box, Tuple<float, float> dimension)
        {
            RectangleF container = CreatRectFromTuple(box, dimension);

            RectangleF field = CreatRectFromTuple(tuple);

            return container.Contains(field);
        }
        private static RectangleF CreatRectFromTuple(Tuple<PointF, PointF, PointF, PointF> box, Tuple<float, float> dimension = null)
        {
            var localDimension = dimension;
            if (dimension == null)
                localDimension = new Tuple<float, float>(1, 1);

            return new RectangleF(box.Item1.X, box.Item1.Y,
                (box.Item2.X - box.Item1.X) * localDimension.Item1,
                (box.Item3.Y - box.Item1.Y) * localDimension.Item2);
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

        public static Tuple<float, float, float, float> GetOptionsFieldDimension(OptionsField field)
        {
            switch (field)
            {
                //case DocumentField.NULL:
                //    break;
                case OptionsField.TEM_CPF_SIM:
                    return new Tuple<float, float, float, float>(1.3f, 1.4f, -1.5f, 0f);
                case OptionsField.TEM_CPF_NAO:
                    return new Tuple<float, float, float, float>(1.3f, 1.4f, -1.3f, 0f);
                case OptionsField.ESTRANGEIRO_SIM:
                    return new Tuple<float, float, float, float>(1.3f, 1.4f, -1.5f, 0f);
                case OptionsField.ESTRANGEIRO_NAO:
                    return new Tuple<float, float, float, float>(1.3f, 1.4f, -1.3f, 0f);
                //case DocumentField.PROFISSIONAL_SAUDE:
                //    break;
                //case DocumentField.PROFISSIONAL_SEGURANCA:
                //    break;
                case OptionsField.SEXO_MASC:
                    return new Tuple<float, float, float, float>(0.5f, 1.4f, -0.5f, 0f);
                case OptionsField.SEXO_FEM:
                    return new Tuple<float, float, float, float>(0.5f, 1.4f, -0.5f, 0f);
                    //case DocumentField.RACA:
                    //    return new Tuple<float, float>(9.4f, 4f);
                    //case DocumentField.SINTOMAS:
                    //    return new Tuple<float, float>(5.4f, 4f);
                    //case DocumentField.CONDICOES:
                    //    return new Tuple<float, float>(12.0f, 5f);
                    //case DocumentField.ESTADO_TESTE:
                    //    return new Tuple<float, float>(1.4f, 6f);
                    //case DocumentField.TIPO_TESTE:
                    //    return new Tuple<float, float>(10f, 7f);
                    //case DocumentField.RESULTADO_TESTE:
                    //    return new Tuple<float, float>(2f, 5f);
                    //case DocumentField.CLASSIFICACAO_FINAL:
                    //    return new Tuple<float, float>(6f, 6f);
                    //case DocumentField.EVOLUCAO_CASO:
                    //    return new Tuple<float, float>(5f, 6f);
                    //default:
                    //break;
            }
            return new Tuple<float, float, float, float>(1f, 1f, 1f, 1f);
        }
    }
}
