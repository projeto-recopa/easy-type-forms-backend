using System;
using System.Collections.Generic;
using System.Text;

namespace recopa_types
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
    class Enumerations
    {
    }
}
