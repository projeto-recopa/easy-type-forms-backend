using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace ExtractTrainningData
{
    public enum DocumentField
    {
        NULL = 0,
        TEM_CPF,
        ESTRANGEIRO,
        PROFISSIONAL_SAUDE,
        PROFISSIONAL_SEGURANCA,
        SEXO,
        RACA,
        //RACA_BRANCA,
        //RACA_PRETA,
        //RACA_AMARELA,
        //RACA_PARDA,
        //RACA_INDIGENA,
        SINTOMAS,
        CONDICOES,
        ESTADO_TESTE,
        TIPO_TESTE,
        RESULTADO_TESTE,
        CLASSIFICACAO_FINAL,
        EVOLUCAO_CASO,
    }
    public enum OptionsField
    {
        NULL = 0,
        TEM_CPF_SIM,
        TEM_CPF_NAO,
        ESTRANGEIRO_SIM,
        ESTRANGEIRO_NAO,
        PROFISSIONAL_SAUDE_SIM,
        PROFISSIONAL_SAUDE_NAO,
        PROFISSIONAL_SEGURANCA_SIM,
        PROFISSIONAL_SEGURANCA_NAO,
        SEXO_MASC,
        SEXO_FEM,
        //RACA,
        RACA_BRANCA,
        RACA_PRETA,
        RACA_AMARELA,
        RACA_PARDA,
        RACA_INDIGENA,
        //SINTOMAS,
        //CONDICOES,
        //ESTADO_TESTE,
        //TIPO_TESTE,
        RESULTADO_TESTE_POSITIVO,
        RESULTADO_TESTE_NEGATIVO,
        //CLASSIFICACAO_FINAL,
        EVOLUCAO_CASO_CANCELADO,
        EVOLUCAO_CASO_IGNORADO,
        EVOLUCAO_CASO_TRATAMENTO,
        EVOLUCAO_CASO_INTERNADO,
        EVOLUCAO_CASO_UTI,
        EVOLUCAO_CASO_OBITO,
        EVOLUCAO_CASO_CURA,
    }
    public class CropBoxes
    {
        private Dictionary<DocumentField, Tuple<PointF, PointF, PointF, PointF>> _boxes = new Dictionary<DocumentField, Tuple<PointF, PointF, PointF, PointF>>();

        private Dictionary<OptionsField, Tuple<PointF, PointF, PointF, PointF>> option_boxes = new Dictionary<OptionsField, Tuple<PointF, PointF, PointF, PointF>>();

        private Dictionary<string, Tuple<PointF, PointF, PointF, PointF>> all_boxes = new Dictionary<string, Tuple<PointF, PointF, PointF, PointF>>();

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
        private void PushAll(string word, Tuple<PointF, PointF, PointF, PointF> box)
        {
            if (!this.all_boxes.ContainsKey(word))
            {
                this.all_boxes.Add(word, box);
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

        private DocumentField GetFieldByText(string word)
        {
            if (!string.IsNullOrEmpty(word))
            {
                var lowrCaseWord = word; //.ToLower();
                if (lowrCaseWord.Contains("Tem")) return DocumentField.TEM_CPF;
                if (lowrCaseWord.Contains("Estrangeiro")) return DocumentField.ESTRANGEIRO;
                //if (lowrCaseWord.Contains("profissional")) {
                if (lowrCaseWord.Contains("saúde")) return DocumentField.PROFISSIONAL_SAUDE;
                if (lowrCaseWord.Contains("segurança")) return DocumentField.PROFISSIONAL_SEGURANCA;
                //}
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
            PushAll(word, box);
            //PushSelected(GetFieldByText(word), box);
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
                        FindContainedOption(OptionsField.TEM_CPF_SIM, this._boxes[item], GetFieldDimension(item));
                        FindContainedOption(OptionsField.TEM_CPF_NAO, this._boxes[item], GetFieldDimension(item));
                        break;

                    case DocumentField.ESTRANGEIRO:
                        FindContainedOption(OptionsField.ESTRANGEIRO_SIM, this._boxes[item], GetFieldDimension(item));
                        FindContainedOption(OptionsField.ESTRANGEIRO_NAO, this._boxes[item], GetFieldDimension(item));
                        break;

                    case DocumentField.PROFISSIONAL_SAUDE:
                        FindContainedOption(OptionsField.PROFISSIONAL_SAUDE_SIM, this._boxes[item], GetFieldDimension(item));
                        FindContainedOption(OptionsField.PROFISSIONAL_SAUDE_NAO, this._boxes[item], GetFieldDimension(item));
                        break;

                    case DocumentField.PROFISSIONAL_SEGURANCA:
                        FindContainedOption(OptionsField.PROFISSIONAL_SEGURANCA_SIM, this._boxes[item], GetFieldDimension(item));
                        FindContainedOption(OptionsField.PROFISSIONAL_SEGURANCA_NAO, this._boxes[item], GetFieldDimension(item));
                        break;

                    case DocumentField.SEXO:
                        FindContainedOption(OptionsField.SEXO_MASC, this._boxes[item], GetFieldDimension(item));
                        FindContainedOption(OptionsField.SEXO_FEM, this._boxes[item], GetFieldDimension(item));
                        break;

                    case DocumentField.RACA:
                        FindContainedOption(OptionsField.RACA_AMARELA, this._boxes[item], GetFieldDimension(item));
                        FindContainedOption(OptionsField.RACA_BRANCA, this._boxes[item], GetFieldDimension(item));
                        FindContainedOption(OptionsField.RACA_INDIGENA, this._boxes[item], GetFieldDimension(item));
                        FindContainedOption(OptionsField.RACA_PARDA, this._boxes[item], GetFieldDimension(item));
                        FindContainedOption(OptionsField.RACA_PRETA, this._boxes[item], GetFieldDimension(item));
                        break;

                    case DocumentField.RESULTADO_TESTE:
                        FindContainedOption(OptionsField.RESULTADO_TESTE_POSITIVO, this._boxes[item], GetFieldDimension(item));
                        FindContainedOption(OptionsField.RESULTADO_TESTE_NEGATIVO, this._boxes[item], GetFieldDimension(item));
                        break;

                    case DocumentField.EVOLUCAO_CASO:
                        FindContainedOption(OptionsField.EVOLUCAO_CASO_CANCELADO, this._boxes[item], GetFieldDimension(item));
                        FindContainedOption(OptionsField.EVOLUCAO_CASO_CURA, this._boxes[item], GetFieldDimension(item));
                        FindContainedOption(OptionsField.EVOLUCAO_CASO_IGNORADO, this._boxes[item], GetFieldDimension(item));
                        FindContainedOption(OptionsField.EVOLUCAO_CASO_INTERNADO, this._boxes[item], GetFieldDimension(item));
                        FindContainedOption(OptionsField.EVOLUCAO_CASO_OBITO, this._boxes[item], GetFieldDimension(item));
                        FindContainedOption(OptionsField.EVOLUCAO_CASO_TRATAMENTO, this._boxes[item], GetFieldDimension(item));
                        FindContainedOption(OptionsField.EVOLUCAO_CASO_UTI, this._boxes[item], GetFieldDimension(item));
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
                case OptionsField.PROFISSIONAL_SAUDE_SIM: return "sim";
                case OptionsField.PROFISSIONAL_SAUDE_NAO: return "não";
                case OptionsField.PROFISSIONAL_SEGURANCA_SIM: return "sim";
                case OptionsField.PROFISSIONAL_SEGURANCA_NAO: return "não";
                case OptionsField.RESULTADO_TESTE_POSITIVO: return "positivo";
                case OptionsField.RESULTADO_TESTE_NEGATIVO: return "negativo";
                case OptionsField.RACA_AMARELA: return "amarela";
                case OptionsField.RACA_BRANCA: return "branca";
                case OptionsField.RACA_INDIGENA: return "indígena";
                case OptionsField.RACA_PARDA: return "parda";
                case OptionsField.RACA_PRETA: return "preta";
                case OptionsField.EVOLUCAO_CASO_CANCELADO: return "cancelado";
                case OptionsField.EVOLUCAO_CASO_CURA: return "cura";
                case OptionsField.EVOLUCAO_CASO_IGNORADO: return "ignorado";
                case OptionsField.EVOLUCAO_CASO_INTERNADO: return "internado"; //TODO: Revisar duplicidade
                case OptionsField.EVOLUCAO_CASO_OBITO: return "óbito";
                case OptionsField.EVOLUCAO_CASO_TRATAMENTO: return "domiciliar";
                case OptionsField.EVOLUCAO_CASO_UTI: return "uti";
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
                    return new Tuple<float, float>(2f, 4f);
                case DocumentField.PROFISSIONAL_SEGURANCA:
                    return new Tuple<float, float>(2f, 4f);
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
                    return new Tuple<float, float, float, float>(1.1f, 1.4f, -1.5f, 0f);
                case OptionsField.ESTRANGEIRO_NAO:
                    return new Tuple<float, float, float, float>(1.1f, 1.4f, -1.3f, 0f);
                case OptionsField.PROFISSIONAL_SAUDE_SIM:
                    return new Tuple<float, float, float, float>(1.3f, 1.4f, -1.5f, 0f);
                case OptionsField.PROFISSIONAL_SAUDE_NAO:
                    return new Tuple<float, float, float, float>(1.3f, 1.4f, -1.3f, 0f);
                case OptionsField.PROFISSIONAL_SEGURANCA_SIM:
                    return new Tuple<float, float, float, float>(1.3f, 1.4f, -1.5f, 0f);
                case OptionsField.PROFISSIONAL_SEGURANCA_NAO:
                    return new Tuple<float, float, float, float>(1.3f, 1.4f, -1.3f, 0f);
                case OptionsField.SEXO_MASC:
                    return new Tuple<float, float, float, float>(0.5f, 1.4f, -0.5f, 0f);
                case OptionsField.SEXO_FEM:
                    return new Tuple<float, float, float, float>(0.5f, 1.4f, -0.5f, 0f);
                case OptionsField.RACA_AMARELA:
                    return new Tuple<float, float, float, float>(.6f, 1f, -0.6f, 0f);
                case OptionsField.RACA_BRANCA:
                    return new Tuple<float, float, float, float>(.7f, 1f, -0.7f, 0f);
                case OptionsField.RACA_INDIGENA:
                    return new Tuple<float, float, float, float>(.6f, 1f, -0.6f, 0f);
                case OptionsField.RACA_PARDA:
                //return new Tuple<float, float, float, float>(.6f, 1f, -0.6f, 0f);
                case OptionsField.RACA_PRETA:
                    return new Tuple<float, float, float, float>(.8f, 1f, -0.8f, 0f);
                //case OptionsField.SINTOMAS:
                //    return new Tuple<float, float>(5.4f, 4f);
                //case OptionsField.CONDICOES:
                //    return new Tuple<float, float>(12.0f, 5f);
                //case OptionsField.ESTADO_TESTE:
                //    return new Tuple<float, float>(1.4f, 6f);
                //case OptionsField.TIPO_TESTE:
                //    return new Tuple<float, float>(10f, 7f);
                case OptionsField.RESULTADO_TESTE_POSITIVO:
                case OptionsField.RESULTADO_TESTE_NEGATIVO:
                    return new Tuple<float, float, float, float>(.6f, 1f, -0.6f, 0f);
                //case OptionsField.CLASSIFICACAO_FINAL:
                //    return new Tuple<float, float>(6f, 6f);
                case OptionsField.EVOLUCAO_CASO_CANCELADO:
                    return new Tuple<float, float, float, float>(.5f, 1f, -0.5f, 0f);
                case OptionsField.EVOLUCAO_CASO_CURA:
                    return new Tuple<float, float, float, float>(.9f, 1f, -0.8f, 0f);
                case OptionsField.EVOLUCAO_CASO_IGNORADO:
                    return new Tuple<float, float, float, float>(.4f, 1f, -0.4f, 0f);
                case OptionsField.EVOLUCAO_CASO_INTERNADO:
                case OptionsField.EVOLUCAO_CASO_OBITO:
                    return new Tuple<float, float, float, float>(0.6f, 1f, -0.5f, 0f);
                case OptionsField.EVOLUCAO_CASO_TRATAMENTO:
                    return new Tuple<float, float, float, float>(0.4f, 1f, -0.9f, 0f);
                case OptionsField.EVOLUCAO_CASO_UTI:
                    return new Tuple<float, float, float, float>(1.2f, 1f, -4f, 0f);
                    //default:
                    //break;
            }
            return new Tuple<float, float, float, float>(1f, 1f, 1f, 1f);
        }
    }
}
