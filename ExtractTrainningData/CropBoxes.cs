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
        SINTOMAS_FEBRE,
        SINTOMAS_DOR_GARGANTA,
        SINTOMAS_TOSSE,
        SINTOMAS_DISPNEIA,
        SINTOMAS_OUTROS,
        CONDICOES_DOENCAS_RESPIRATORIAS,
        CONDICOES_DOENCAS_RENAIS,
        CONDICOES_DOENCAS_CROMOSSOMICA,
        CONDICOES_DIABETES,
        CONDICOES_IMUNOSSUPRESSAO,
        CONDICOES_DOENCAS_CARDIACAS,
        CONDICOES_GESTANTE,
        //ESTADO_TESTE,
        //TIPO_TESTE,
        RESULTADO_TESTE_POSITIVO,
        RESULTADO_TESTE_NEGATIVO,
        CLASSIFICACAO_FINAL_DESCARTADO,
        CLASSIFICACAO_FINAL_CONFIRMADO_EPIDEMIOLOGICO,
        CLASSIFICACAO_FINAL_CONFIRMADO_LABORATORIAL,
        CLASSIFICACAO_FINAL_CONFIRMADO_IMAGEM,
        CLASSIFICACAO_FINAL_CONFIRMADO_CRITERIO,
        CLASSIFICACAO_FINAL_SINDROME_GRIPAL,
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

        private DocumentField GetFieldByText(string word, string texton1, string texton2)
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
                if (lowrCaseWord.ToLower().Contains("teste"))
                {
                    if (
                        (!string.IsNullOrEmpty(texton1) && texton1.ToLower().Contains("estado"))
                        ||
                        (!string.IsNullOrEmpty(texton2) && texton2.ToLower().Contains("estado"))
                        )
                    {
                        return DocumentField.ESTADO_TESTE;
                    }

                    if (
                        (!string.IsNullOrEmpty(texton1) && texton1.ToLower().Contains("tipo"))
                        ||
                        (!string.IsNullOrEmpty(texton2) && texton2.ToLower().Contains("tipo"))
                        )
                    {
                        return DocumentField.TIPO_TESTE;
                    }

                    //    if (lowrCaseWord.Contains("Estado")) return DocumentField.ESTADO_TESTE;
                    //if (lowrCaseWord.Contains("Tipo")) return DocumentField.TIPO_TESTE;
                }

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
            var texton1 = string.Empty;
            var texton2 = string.Empty;
            foreach (var item in this.all_boxes.Keys)
            {
                PushSelected(GetFieldByText(item, texton1, texton2), this.all_boxes[item]);
                texton2 = texton1;
                texton1 = item;
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
                    case DocumentField.SINTOMAS:
                        FindContainedOption(OptionsField.SINTOMAS_DISPNEIA, this._boxes[item], GetFieldDimension(item));
                        FindContainedOption(OptionsField.SINTOMAS_DOR_GARGANTA, this._boxes[item], GetFieldDimension(item));
                        FindContainedOption(OptionsField.SINTOMAS_FEBRE, this._boxes[item], GetFieldDimension(item));
                        FindContainedOption(OptionsField.SINTOMAS_TOSSE, this._boxes[item], GetFieldDimension(item));
                        FindContainedOption(OptionsField.SINTOMAS_OUTROS, this._boxes[item], GetFieldDimension(item));
                        break;

                    case DocumentField.CONDICOES:
                        FindContainedOption(OptionsField.CONDICOES_DIABETES, this._boxes[item], GetFieldDimension(item));
                        FindContainedOption(OptionsField.CONDICOES_DOENCAS_CARDIACAS, this._boxes[item], GetFieldDimension(item));
                        FindContainedOption(OptionsField.CONDICOES_DOENCAS_RENAIS, this._boxes[item], GetFieldDimension(item));
                        FindContainedOption(OptionsField.CONDICOES_DOENCAS_RESPIRATORIAS, this._boxes[item], GetFieldDimension(item));
                        FindContainedOption(OptionsField.CONDICOES_DOENCAS_CROMOSSOMICA, this._boxes[item], GetFieldDimension(item));
                        FindContainedOption(OptionsField.CONDICOES_GESTANTE, this._boxes[item], GetFieldDimension(item));
                        FindContainedOption(OptionsField.CONDICOES_IMUNOSSUPRESSAO, this._boxes[item], GetFieldDimension(item));
                        break;

                    case DocumentField.CLASSIFICACAO_FINAL:
                        FindContainedOption(OptionsField.CLASSIFICACAO_FINAL_CONFIRMADO_CRITERIO, this._boxes[item], GetFieldDimension(item));
                        FindContainedOption(OptionsField.CLASSIFICACAO_FINAL_CONFIRMADO_EPIDEMIOLOGICO, this._boxes[item], GetFieldDimension(item));
                        FindContainedOption(OptionsField.CLASSIFICACAO_FINAL_CONFIRMADO_IMAGEM, this._boxes[item], GetFieldDimension(item));
                        FindContainedOption(OptionsField.CLASSIFICACAO_FINAL_CONFIRMADO_LABORATORIAL, this._boxes[item], GetFieldDimension(item));
                        FindContainedOption(OptionsField.CLASSIFICACAO_FINAL_DESCARTADO, this._boxes[item], GetFieldDimension(item));
                        FindContainedOption(OptionsField.CLASSIFICACAO_FINAL_SINDROME_GRIPAL, this._boxes[item], GetFieldDimension(item));
                        break;
                    default: break;
                }
            }
            return this;
        }

        private void FindContainedOption(OptionsField option, Tuple<PointF, PointF, PointF, PointF> box, Tuple<float, float, float, float> dimension)
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
                case OptionsField.SINTOMAS_DISPNEIA: return "dispneia";
                case OptionsField.SINTOMAS_DOR_GARGANTA: return "garganta";
                case OptionsField.SINTOMAS_FEBRE: return "febre";
                case OptionsField.SINTOMAS_TOSSE: return "tosse";
                case OptionsField.SINTOMAS_OUTROS: return "outros";
                case OptionsField.CONDICOES_DOENCAS_RESPIRATORIAS: return "respiratórias";
                case OptionsField.CONDICOES_DOENCAS_RENAIS: return "renais";
                case OptionsField.CONDICOES_DOENCAS_CROMOSSOMICA: return "cromossômicas";
                case OptionsField.CONDICOES_DIABETES: return "diabetes";
                case OptionsField.CONDICOES_IMUNOSSUPRESSAO: return "imunossupressão";
                case OptionsField.CONDICOES_DOENCAS_CARDIACAS: return "cardíacas";
                case OptionsField.CONDICOES_GESTANTE: return "gestante";
                case OptionsField.CLASSIFICACAO_FINAL_CONFIRMADO_CRITERIO: return "critério";
                case OptionsField.CLASSIFICACAO_FINAL_CONFIRMADO_EPIDEMIOLOGICO: return "epidemiológico";
                case OptionsField.CLASSIFICACAO_FINAL_CONFIRMADO_IMAGEM: return "imagem";
                case OptionsField.CLASSIFICACAO_FINAL_CONFIRMADO_LABORATORIAL: return "laboratorial";
                case OptionsField.CLASSIFICACAO_FINAL_DESCARTADO: return "descartado";
                case OptionsField.CLASSIFICACAO_FINAL_SINDROME_GRIPAL: return "gripal";
                default: return string.Empty;
            }

        }

        private bool IsInsidePolygon(Tuple<PointF, PointF, PointF, PointF> tuple, Tuple<PointF, PointF, PointF, PointF> box, Tuple<float, float, float, float> dimension)
        {
            RectangleF container = CreatRectFromTuple(box, dimension);

            RectangleF field = CreatRectFromTuple(tuple);

            return container.Contains(field);
        }

        private static RectangleF CreatRectFromTuple(Tuple<PointF, PointF, PointF, PointF> box, Tuple<float, float, float, float> dimension = null)
        {
            var localDimension = dimension;
            if (dimension == null)
                localDimension = new Tuple<float, float, float, float>(1, 1, 0, 0);

            float hResize = localDimension.Item1, vResize = localDimension.Item2, xTranslate = localDimension.Item3, yTranslate = localDimension.Item4;

            var width = (box.Item2.X - box.Item1.X);
            var heigth = (box.Item3.Y - box.Item1.Y);

            RectangleF cropRect = new RectangleF(box.Item1.X + (width * xTranslate), box.Item1.Y + (heigth * yTranslate),
                width * hResize,
                heigth * vResize);

            //return new RectangleF(box.Item1.X, box.Item1.Y,
            //    (box.Item2.X - box.Item1.X) * localDimension.Item1,
            //    (box.Item3.Y - box.Item1.Y) * localDimension.Item2);
            return cropRect;
        }

        public static Tuple<float, float, float, float> GetFieldDimension(DocumentField field)
        {
            switch (field)
            {
                //case DocumentField.NULL:
                //    break;
                case DocumentField.TEM_CPF:
                    return new Tuple<float, float, float, float>(5f, 4f, 0f, 0f);
                case DocumentField.ESTRANGEIRO:
                    return new Tuple<float, float, float, float>(2f, 4f, 0f, 0f);
                case DocumentField.PROFISSIONAL_SAUDE:
                    return new Tuple<float, float, float, float>(5f, 4f, -3f, 0f);
                case DocumentField.PROFISSIONAL_SEGURANCA:
                    return new Tuple<float, float, float, float>(4f, 4f, -2f, 0f);
                case DocumentField.SEXO:
                    return new Tuple<float, float, float, float>(8f, 4f, 0f, 0f);
                case DocumentField.RACA:
                    return new Tuple<float, float, float, float>(9.4f, 4f, 0f, 0f);
                case DocumentField.SINTOMAS:
                    return new Tuple<float, float, float, float>(5.4f, 4f, 0f, 0f);
                case DocumentField.CONDICOES:
                    return new Tuple<float, float, float, float>(12.0f, 5f, 0f, 0f);
                case DocumentField.ESTADO_TESTE:
                    return new Tuple<float, float, float, float>(1.4f, 6f, 0f, 0f);
                case DocumentField.TIPO_TESTE:
                    return new Tuple<float, float, float, float>(7f, 8f, -1.9f, 0f);
                case DocumentField.RESULTADO_TESTE:
                    return new Tuple<float, float, float, float>(2f, 5f, 0f, 0f);
                case DocumentField.CLASSIFICACAO_FINAL:
                    return new Tuple<float, float, float, float>(6f, 6f, 0f, 0f);
                case DocumentField.EVOLUCAO_CASO:
                    return new Tuple<float, float, float, float>(5f, 6f, 0f, 0f);
                default:
                    break;
            }
            return new Tuple<float, float, float, float>(1f, 1f, 0f, 0f);
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
                    return new Tuple<float, float, float, float>(.8f, 1f, -1.0f, 0f);
                case OptionsField.SINTOMAS_DISPNEIA:
                    return new Tuple<float, float, float, float>(.5f, 1f, -0.6f, 0f);
                case OptionsField.SINTOMAS_DOR_GARGANTA:
                    return new Tuple<float, float, float, float>(.5f, 1f, -1.3f, 0f);
                case OptionsField.SINTOMAS_FEBRE:
                    return new Tuple<float, float, float, float>(0.8f, 1f, -0.9f, 0f);
                case OptionsField.SINTOMAS_TOSSE:
                    return new Tuple<float, float, float, float>(0.9f, 1f, -1.0f, 0f);
                case OptionsField.SINTOMAS_OUTROS:
                    return new Tuple<float, float, float, float>(0.8f, 1f, -0.8f, 0f);
                case OptionsField.CONDICOES_DIABETES:
                    return new Tuple<float, float, float, float>(.5f, 1f, -0.6f, 0f);
                case OptionsField.CONDICOES_DOENCAS_CARDIACAS:
                    return new Tuple<float, float, float, float>(.5f, 1f, -1.6f, 0f);
                case OptionsField.CONDICOES_DOENCAS_CROMOSSOMICA:
                    return new Tuple<float, float, float, float>(.3f, 1f, -1.7f, 0f);
                case OptionsField.CONDICOES_DOENCAS_RENAIS:
                    return new Tuple<float, float, float, float>(.8f, 1f, -2.5f, 0f);
                case OptionsField.CONDICOES_DOENCAS_RESPIRATORIAS:
                    return new Tuple<float, float, float, float>(.5f, 1f, -1.2f, 0f);
                case OptionsField.CONDICOES_GESTANTE:
                    return new Tuple<float, float, float, float>(.5f, 1f, -0.6f, 0f);
                case OptionsField.CONDICOES_IMUNOSSUPRESSAO:
                    return new Tuple<float, float, float, float>(.3f, 1f, -0.3f, 0f);
                //case OptionsField.ESTADO_TESTE:
                //    return new Tuple<float, float>(1.4f, 6f);
                //case OptionsField.TIPO_TESTE:
                //    return new Tuple<float, float>(10f, 7f);
                case OptionsField.RESULTADO_TESTE_POSITIVO:
                case OptionsField.RESULTADO_TESTE_NEGATIVO:
                    return new Tuple<float, float, float, float>(.6f, 1f, -0.6f, 0f);
                case OptionsField.CLASSIFICACAO_FINAL_CONFIRMADO_CRITERIO:
                    return new Tuple<float, float, float, float>(.6f, 1.1f, -3f, 0f);
                case OptionsField.CLASSIFICACAO_FINAL_CONFIRMADO_EPIDEMIOLOGICO:
                    return new Tuple<float, float, float, float>(.3f, 1.1f, -1.6f, 0f);
                case OptionsField.CLASSIFICACAO_FINAL_CONFIRMADO_IMAGEM:
                    return new Tuple<float, float, float, float>(.7f, 1.1f, -3.2f, 0f);
                case OptionsField.CLASSIFICACAO_FINAL_CONFIRMADO_LABORATORIAL:
                    return new Tuple<float, float, float, float>(.4f, 1.1f, -1.4f, 0f);
                case OptionsField.CLASSIFICACAO_FINAL_DESCARTADO:
                    return new Tuple<float, float, float, float>(.5f, 1.1f, -0.5f, 0f);
                case OptionsField.CLASSIFICACAO_FINAL_SINDROME_GRIPAL:
                    return new Tuple<float, float, float, float>(.7f, 1.1f, -2.5f, 0f);
                case OptionsField.EVOLUCAO_CASO_CURA:
                    return new Tuple<float, float, float, float>(1.0f, 1f, -1.0f, 0f);
                case OptionsField.EVOLUCAO_CASO_IGNORADO:
                    return new Tuple<float, float, float, float>(.7f, 1f, -0.6f, 0f);
                case OptionsField.EVOLUCAO_CASO_INTERNADO:
                case OptionsField.EVOLUCAO_CASO_OBITO:
                    return new Tuple<float, float, float, float>(0.8f, 1f, -0.8f, 0f);
                case OptionsField.EVOLUCAO_CASO_TRATAMENTO:
                    return new Tuple<float, float, float, float>(0.4f, 1f, -2.0f, 0f);
                case OptionsField.EVOLUCAO_CASO_UTI:
                    return new Tuple<float, float, float, float>(1.2f, 1f, -4f, 0f);
                    //default:
                    //break;
            }
            return new Tuple<float, float, float, float>(1f, 1f, 1f, 1f);
        }
    }
}
