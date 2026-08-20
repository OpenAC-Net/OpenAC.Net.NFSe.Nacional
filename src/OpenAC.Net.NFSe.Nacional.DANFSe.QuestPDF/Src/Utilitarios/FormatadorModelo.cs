using System;
using System.Collections.Generic;
using OpenAC.Net.NFSe.Nacional.Common.Model;
using OpenAC.Net.NFSe.Nacional.Common.Types;

namespace OpenAC.Net.NFSe.Nacional.DANFSe.QuestPDF.Src.Utilitarios;

/// <summary>
/// Fornece métodos auxiliares para extração e formatação
/// de dados dos modelos da NFS-e Nacional.
/// </summary>
internal static partial class Formatador
{
    /// <summary>
    /// Retorna a descrição textual correspondente ao ambiente da NFS-e.
    /// </summary>
    /// <param name="ambiente">Ambiente de geração da NFS-e.</param>
    /// <returns>Descrição do ambiente.</returns>
    public static string FormatarAmbiente(AmbienteGerador ambiente)
    {
        return ambiente switch
        {
            AmbienteGerador.Prefeitura => "Prefeitura Municipal",
            AmbienteGerador.Nacional => "Sefin Nacional NFS-e",
            _ => throw new NotImplementedException("Ambiente não implementado!"),
        };
    }

    /// <summary>
    /// Retorna a descrição textual correspondente ao tipo de emissão da NFS-e.
    /// </summary>
    /// <param name="tipoEmissao">Tipo de emissão da NFS-e.</param>
    /// <returns>Descrição do tipo de emissão.</returns>
    public static string FormatarTipoEmissao(TipoEmissao tipoEmissao)
    {
        return tipoEmissao switch
        {
            TipoEmissao.ModeloNacional => "Nacional",
            TipoEmissao.ModeloMunicipio => "município",
            _ => throw new NotImplementedException("Tipo de emissão não implementada!"),
        };
    }

    /// <summary>
    /// Retorna o documento de identificação de uma pessoa
    /// (CNPJ, CPF ou NIF), quando informado.
    /// </summary>
    public static string ObterDocumento(InfoPessoaNFSe? pessoa)
    {
        if (pessoa is null)
        {
            return "-";
        }
        if (!string.IsNullOrWhiteSpace(pessoa.CNPJ))
        {
            return Documento(pessoa.CNPJ);
        }
        if (!string.IsNullOrWhiteSpace(pessoa.CPF))
        {
            return Documento(pessoa.CPF);
        }
        if (!string.IsNullOrWhiteSpace(pessoa.Nif))
        {
            return Documento(pessoa.Nif);
        }
        return "-";
    }

    /// <summary>
    /// Retorna o documento de identificação de uma pessoa
    /// (CNPJ, CPF ou NIF), quando informado.
    /// </summary>
    public static string ObterDocumento(RTCInfoDest? destinatario)
    {
        if (!string.IsNullOrWhiteSpace(destinatario?.CNPJ))
        {
            return destinatario!.CNPJ!;
        }
        if (!string.IsNullOrWhiteSpace(destinatario?.CPF))
        {
            return destinatario!.CPF!;
        }
        if (!string.IsNullOrWhiteSpace(destinatario?.NIF))
        {
            return destinatario!.NIF!;
        }
        return "-";
    }

    /// <summary>
    /// Formata o endereço de uma pessoa, unindo logradouro,
    /// número e complemento quando disponíveis.
    /// </summary>
    public static string FormatarEndereco(EnderecoNFSe? endereco)
    {
        var partes = new[]
        {
            endereco?.Logradouro,
            endereco?.Numero,
            endereco?.Complemento,
            endereco?.Bairro,
        };
        return Concatenar(partes);
    }

    /// <summary>
    /// Formata o endereço de uma pessoa, unindo logradouro,
    /// número e complemento quando disponíveis.
    /// </summary>
    public static string FormatarEndereco(EnderecoEmitente? endereco)
    {
        var partes = new[]
        {
            endereco?.Logradouro,
            endereco?.Numero,
            endereco?.Complemento,
            endereco?.Bairro,
        };
        return Concatenar(partes);
    }

    /// <summary>
    /// Retorna a descrição textual correspondente ao emitente da DPS.
    /// </summary>
    public static string FormatarEmitente(EmitenteDps emitente)
    {
        return emitente switch
        {
            EmitenteDps.Prestador => "Prestador",
            EmitenteDps.Tomador => "Tomador",
            EmitenteDps.Intermediário => "Intermediário",
            _ => throw new NotImplementedException("Emitente não implementado!")
        };
    }

    /// <summary>
    /// Retorna a descrição textual correspondente ao status da NFS-e.
    /// </summary>
    public static string FormatarStatus(StatusNFSe status)
    {
        return status switch
        {
            StatusNFSe.Gerada or (StatusNFSe)100 => "NFS-e Gerada",
            StatusNFSe.SubstituicaoGerada or (StatusNFSe)101 => "Substituída",
            StatusNFSe.DecisaoJudicial or (StatusNFSe)102 => "Decisão judícial",
            StatusNFSe.Avulsa or (StatusNFSe)103 => "Avulsa",
            _ => "Status não implementado!"
        };
    }

    /// <summary>
    /// Retorna a descrição textual correspondente à finalidade da NFS-e.
    /// </summary>
    public static string FormatarFinalidade(RTCFinNFSe? finalidade)
    {
        return finalidade switch
        {
            null => "-",
            RTCFinNFSe.Regular => "NFS-e regular",
            _ => throw new NotImplementedException("Finalidade não implementada!")
        };
    }

    /// <summary>
    /// Retorna a descrição textual correspondente ao regime de apuração tributária.
    /// </summary>
    /// <param name="regimeApuracao">Regime de apuração do prestador.</param>
    /// <returns>Descrição do regime de apuração ou "-" quando nulo.</returns>
    public static string FormatarRegimeApuracao(RegimeApuracao? regimeApuracao)
    {
        return regimeApuracao switch
        {
            null => "-",
            RegimeApuracao.TributosFederaisMunicipalSN => "Regime de apuração dos tributos federais e municipal pelo Simples Nacional.",
            RegimeApuracao.TributosFederaisSNISSQNPorForaSN => "Regime de apuração dos tributos federais pelo Simples Nacional e ISSQN por fora do Simples Nacional conforme legislação municipal.",
            RegimeApuracao.TributosFederaisMunicipalForaSN => "Regime de apuração dos tributos federais e municipal por fora do Simples Nacional conforme legislações federal e municipal.",
            _ => throw new NotImplementedException("Regime de apuração não implementado!"),
        };
    }

    /// <summary>
    /// Retorna a descrição textual correspondente ao enquadramento no Simples Nacional.
    /// </summary>
    public static string FormatarSimplesNacional(OptanteSimplesNacional simplesNacional)
    {
        return simplesNacional switch
        {
            OptanteSimplesNacional.NaoOptante => "Não optante",
            OptanteSimplesNacional.OptanteMEI => "Optante MEI",
            OptanteSimplesNacional.OptanteMEEPP => "Optante Micro empresa/Empresa pequeno porte",
            _ => throw new NotImplementedException("Simples nacional não implementado!"),
        };
    }

    /// <summary>
    /// Formata um código de tributação nacional quando for possível identificar
    /// uma quantidade válida de dígitos.
    /// </summary>
    public static string FormatarCodigoTributacaoNacional(string? valor)
    {
        var codigoTributacaoNacional = SomenteNumeros(valor);
        if (codigoTributacaoNacional?.Length == 6)
        {
            return Convert.ToUInt32(codigoTributacaoNacional)
                .ToString(@"00\.00\.00");
        }
        return "-";
    }

    /// <summary>
    /// Formata um código nbs quando for possível identificar
    /// uma quantidade válida de dígitos.
    /// </summary>
    public static string FormatarCodigoNbs(string? valor)
    {
        var codigoNbs = SomenteNumeros(valor);
        if (codigoNbs?.Length == 9)
        {
            return Convert.ToUInt64(codigoNbs)
                .ToString(@"0\.0000\.00\.00");
        }
        return "-";
    }

    /// <summary>
    /// Retorna a sigla da UF correspondente ao código IBGE do município informado.
    /// </summary>
    /// <param name="codigoMunicipio">Código IBGE do município.</param>
    /// <returns>Sigla da UF ou "-" quando não for possível identificar.</returns>
    public static string FormatarUfPorCodigoMunipio(string? codigoMunicipio)
    {
        if (string.IsNullOrWhiteSpace(codigoMunicipio) || codigoMunicipio!.Length < 2)
        {
            return "-";
        }
        var municipios = new Dictionary<string, string>
        {
            { "12", "AC" },
            { "27", "AL" },
            { "16", "AP" },
            { "13", "AM" },
            { "29", "BA" },
            { "23", "CE" },
            { "53", "DF" },
            { "32", "ES" },
            { "52", "GO" },
            { "21", "MA" },
            { "51", "MT" },
            { "50", "MS" },
            { "31", "MG"},
            { "15", "PA" },
            { "25", "PB" },
            { "41", "PR" },
            { "26", "PE" },
            { "22", "PI" },
            { "24", "RN" },
            { "43", "RS" },
            { "33", "RJ" },
            { "11", "RO" },
            { "14", "RR" },
            { "42", "SC"},
            { "35", "SP"},
            { "28", "SE" },
            { "17", "TO" }
        };
        var digito = codigoMunicipio.Substring(0, 2);
        return municipios[digito];
    }

    /// <summary>
    /// Retorna a descrição textual correspondente à tributação do ISSQN.
    /// </summary>
    public static string FormatarTributoISSQN(TributoISSQN tributoISSQN)
    {
        return tributoISSQN switch
        {
            TributoISSQN.OperacaoTributavel => "Operação tributável",
            TributoISSQN.Imunidade => "Imunidade",
            TributoISSQN.ExportacaoServico => "Exportação de serviço",
            TributoISSQN.NaoIncidencia => "Não incidencia",
            _ => throw new NotImplementedException("Tributação ISSQN não implementada!"),
        };
    }

    /// <summary>
    /// Retorna a descrição textual correspondente ao regime especial do ISSQN.
    /// </summary>
    public static string FormatarRegimeEspecialISSQN(RegimeEspecial? regimeEspecial)
    {
        return regimeEspecial switch
        {
            null => "-",
            RegimeEspecial.Nenhum => "Nenhum",
            RegimeEspecial.Cooperativa => "Cooperativa",
            RegimeEspecial.Estimativa => "Estimativa",
            RegimeEspecial.MicroempresaMunicipal => "Microempresa Municipal",
            RegimeEspecial.NotarioRegistrador => "Notário ou Registrador",
            RegimeEspecial.ProfissionalAutonomo => "Profissional Autônomo",
            RegimeEspecial.SociedadeProfissionais => "Sociedade de Profissionais",
            _ => throw new NotImplementedException("Regime especial do ISSQN não implementado"),
        };
    }

    /// <summary>
    /// Retorna a descrição textual correspondente ao tipo de imunidade do ISSQN.
    /// </summary>
    public static string FormatarTipoImunidadeISSQN(TipoImunidade? tipoImunidade)
    {
        return tipoImunidade switch
        {
            null => "-",
            TipoImunidade.Imunidade => "Não informada",
            TipoImunidade.CF88Art150VIa => "Órgãos Públicos (CF88, Art. 150, VI, a)",
            TipoImunidade.CF88Art150VIb => "Templos Religiosos (CF88, Art. 150, VI, b)",
            TipoImunidade.CF88Art150VIc => "Partidos, Sindicatos e Entidades sem Fins Lucrativos (CF88, Art. 150, VI, c)",
            TipoImunidade.CF88Art150VId => "Livros, Jornais e Periódicos (CF88, Art. 150, VI, d)",
            TipoImunidade.CF88Art150VIe => "Fonogramas e Mídias Nacionais (CF88, Art. 150, VI, e)",
            _ => throw new NotImplementedException("Tipo de imunidade do ISSQN não implementado!"),
        };
    }

    /// <summary>
    /// Retorna a descrição textual correspondente ao tipo de suspensão da exigibilidade do ISSQN.
    /// </summary>
    public static string FormatarSuspensaoExibilidadeISSQN(TipoSuspensao? tipoSuspensao)
    {
        return tipoSuspensao switch
        {
            null => "-",
            TipoSuspensao.PorDecisaoJudicial => "Exigibilidade Suspensa por Decisão Judicial.",
            TipoSuspensao.PorProcessoAdministrativo => "Exigibilidade Suspensa por Processo Administrativo.",
            _ => throw new NotImplementedException("Tipo de suspensão ISSQN não implementado!"),
        };
    }

    /// <summary>
    /// Retorna o número formatado do benefício municipal, quando informado.
    /// </summary>
    public static string FormatarTipoBeneficioMunicipal(TipoBeneficioMunicipal? tipoBeneficioMunicipal)
    {
        return tipoBeneficioMunicipal switch
        {
            null => "-",
            TipoBeneficioMunicipal.Isencao => "Isenção",
            TipoBeneficioMunicipal.ReducaoBCPerc => "Redução da BC em 'ppBM' %",
            TipoBeneficioMunicipal.ReducaoBCValor => "Redução da BC em R$ 'vInfoBM'",
            TipoBeneficioMunicipal.AliquotaDiferenciada => "Alíquota Diferenciada de 'aliqDifBM' %",
            _ => throw new NotImplementedException("Tipo de beneficio municipal não implementado!"),
        };
    }

    /// <summary>
    /// Retorna o número formatado do benefício municipal, quando informado.
    /// </summary>
    public static string FormatarBenefinicioMunicial(BeneficioMunicipal? beneficioMunicipal)
    {
        return Texto(beneficioMunicipal?.NumeroBeneficio);
    }

    /// <summary>
    /// Retorna a descrição textual correspondente ao tipo de retenção do ISSQN.
    /// </summary>
    public static string FormatarTipoRetencaoISSQN(TipoRetencaoISSQN? tipoRetencao)
    {
        return tipoRetencao switch
        {
            null => "-",
            TipoRetencaoISSQN.NaoRetido => "Não retido",
            TipoRetencaoISSQN.RetidoTomador => "Retido pelo tomador",
            TipoRetencaoISSQN.RetidoIntermediario => "Retido pelo intermediário",
            _ => throw new NotImplementedException("Tipo de retenção não implementada!"),
        };
    }

    /// <summary>
    /// Retorna a descrição textual correspondente ao tipo de retenção de PIS, COFINS e CSLL.
    /// </summary>
    public static string FormatarTipoRetencaoPisCofinsCsll(TipoRetencaoPisCofinsCsll? tipoRetencaoPisCofinsCsll)
    {
        return tipoRetencaoPisCofinsCsll switch
        {
            null => "-",
            TipoRetencaoPisCofinsCsll.PisCofinsCsllNaoRetidos => "PIS/COFINS/CSLL Não Retidos",
            TipoRetencaoPisCofinsCsll.PisCofinsRetidos => "PIS/COFINS Retidos",
            TipoRetencaoPisCofinsCsll.PisCofinsNaoRetidos => "PIS/COFINS Não Retidos",
            TipoRetencaoPisCofinsCsll.PisCofinsCsllRetidos => "PIS/COFINS/CSLL Retidos",
            TipoRetencaoPisCofinsCsll.PisCofinsRetidosCsllNaoRetido => "PIS/COFINS Retidos, CSLL Não Retido",
            TipoRetencaoPisCofinsCsll.PisRetidoCofinsCsllNaoRetidos => "PIS Retido, COFINS/CSLL Não Retidos",
            TipoRetencaoPisCofinsCsll.CofinsRetidoPisCSllNaoRetidos => "COFINS Retido, PIS/CSLL Não Retidos",
            TipoRetencaoPisCofinsCsll.PisNaoRetidoCofinsCsllRetidos => "PIS Não Retido, COFINS/CSLL Retidos",
            TipoRetencaoPisCofinsCsll.PisCofinsNaoRetidosCsllRetido => "PIS/COFINS Não Retidos, CSLL Retido",
            TipoRetencaoPisCofinsCsll.CofinsNaoRetidoPisCSllRetidos => "COFINS Não Retido, PIS/CSLL Retidos",
            _ => throw new NotImplementedException("Tipo de retenção PIS/COFINS/CSLL não implementado"),
        };
    }
}