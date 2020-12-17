using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace image_cloud_processor.Models
{
    public enum StatusDocumento
    {
        UPLOAD = 0, PROCESSADO, EDICAO, CONCLUIDO, TRANSMITIDO
    }
    public class Document
    {
        public ObjectId _id { get; set; }
        /// <summary>
        /// Usuário que fez a importação
        /// </summary>
        public string User { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public StatusDocumento Status { get; set; }

        public ObjectId attachmentId { get; set; }

        /// <summary>
        /// Extração original
        /// </summary>
        public string[] DadosOriginais { get; set; }
        /// <summary>
        /// Identificador do fomrmulário
        /// </summary>
        public string NumeroFormulario { get; set; }


        public string UF { get; set; }

        public string MunicipioNotificao { get; set; }

        // Dados de Identificação

        public bool PossuiCPF { get; set; }
        public string CPF { get; set; }
        public bool Estrangeiro { get; set; }
        public bool ProfisionalSaude { get; set; }
        public string CBO { get; set; }

        public string CNS { get; set; }
        public string NomeCompleto { get; set; }
        public string NomeCompletoMae { get; set; }

        public string DataNascimento { get; set; }
        public string PaisOrigem { get; set; }

        public string Sexo { get; set; }
        public string Raca { get; set; }

        public string Passaporte { get; set; }


        // Dados clinicos epidemiologicos
        public string CEP { get; set; }
        public string UFResidencia { get; set; }
        public string MunicipioResidencia { get; set; }
        public string Logradouro { get; set; }
        public string Numero { get; set; }
        public string Complemento { get; set; }
        public string Bairro { get; set; }
        public string Telefone { get; set; }

        // TODO: Pendente incluiro campos do formulário
    }
}
