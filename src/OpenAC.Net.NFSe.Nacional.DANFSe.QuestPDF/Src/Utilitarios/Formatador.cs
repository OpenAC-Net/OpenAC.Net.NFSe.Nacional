using System;
using System.Globalization;
using System.Linq;

namespace OpenAC.Net.NFSe.Nacional.DANFSe.QuestPDF.Src.Utilitarios;

/// <summary>
/// Fornece métodos para formatação dos dados apresentados
/// no PDF da NFS-e Nacional.
/// </summary>
internal static partial class Formatador
{
    /// <summary>
    /// Cultura brasileira utilizada nas formatações.
    /// </summary>
    public static readonly CultureInfo Cultura =
        CultureInfo.GetCultureInfo("pt-BR");

    private static readonly CultureInfo CulturaBrasileira = Cultura;

    /// <summary>
    /// Formata um valor monetário opcional utilizando a cultura brasileira.
    /// </summary>    
    public static string Moeda(decimal? valor)
    {
        return valor.HasValue
            ? valor.Value.ToString("C2", CulturaBrasileira)
            : "-";
    }

    /// <summary>
    /// Formata um percentual opcional utilizando a cultura brasileira.
    /// </summary>
    public static string Percentual(decimal? valor)
    {
        return valor.HasValue
            ? $"{valor.Value.ToString("N2", CulturaBrasileira)}%"
            : "-";
    }

    /// <summary>
    /// Formata uma data opcional no padrão utilizado no documento.
    /// </summary>
    public static string Data(DateTime? valor)
    {
        return valor.HasValue
            ? valor.Value.ToString("dd/MM/yyyy", CulturaBrasileira)
            : "-";
    }

    /// <summary>
    /// Formata uma data e hora opcional no padrão utilizado no documento.
    /// </summary>
    public static string DataHora(DateTimeOffset? valor)
    {
        return valor.HasValue
            ? valor.Value.ToString("dd/MM/yyyy HH:mm:ss", CulturaBrasileira)
            : "-";
    }

    /// <summary>
    /// Retorna o texto informado ou hífen quando o conteúdo
    /// estiver vazio.
    /// </summary>
    public static string Texto(string? valor)
    {
        return string.IsNullOrWhiteSpace(valor)
            ? "-"
            : valor!.Trim();
    }

    /// <summary>
    /// Retorna o primeiro texto não vazio entre os valores informados.
    /// </summary>
    public static string Texto(params string?[] valores)
    {
        foreach (var valor in valores)
        {
            if (!string.IsNullOrWhiteSpace(valor))
            {
                return valor!.Trim();
            }
        }
        return "-";
    }

    /// <summary>
    /// Concatena os textos informados, ignorando valores vazios.
    /// </summary>
    public static string Concatenar(params string?[] valores)
    {
        var textos = valores
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(x => x!.Trim())
            .ToArray();
        return textos.Length == 0
            ? "-"
            : string.Join(", ", textos);
    }

    /// <summary>
    /// Retorna somente os caracteres numéricos do valor informado.
    /// </summary>
    public static string SomenteNumeros(string? valor)
    {
        if (string.IsNullOrWhiteSpace(valor))
        {
            return string.Empty;
        }
        return new string(valor.Where(char.IsDigit).ToArray());
    }

    /// <summary>
    /// Formata um número decimal sem casas desnecessárias utilizando a cultura brasileira.
    /// </summary>
    public static string Decimal(decimal? valor)
    {
        return valor.HasValue
            ? valor.Value.ToString("0.##", CulturaBrasileira)
            : "-";
    }

    /// <summary>
    /// Formata um número inteiro opcional utilizando a cultura brasileira.
    /// </summary>
    public static string Inteiro(int? valor)
    {
        return valor.HasValue
            ? valor.Value.ToString(CulturaBrasileira)
            : "-";
    }

    /// <summary>
    /// Formata um CPF, CNPJ ou NIF sem alterar o conteúdo
    /// original quando não for possível identificar o formato.
    /// </summary>
    public static string Documento(string? valor)
    {
        var documento = SomenteNumeros(valor);
        if (documento.Length == 11)
        {
            return Convert.ToUInt64(documento)
                .ToString(@"000\.000\.000\-00");
        }
        if (documento.Length == 14)
        {
            return Convert.ToUInt64(documento)
                .ToString(@"00\.000\.000\/0000\-00");
        }
        return Texto(valor);
    }

    /// <summary>
    /// Formata um CEP brasileiro.
    /// </summary>
    public static string Cep(string? valor)
    {
        var cep = SomenteNumeros(valor);
        if (cep.Length != 8)
        {
            return Texto(valor);
        }
        return Convert.ToUInt32(cep)
            .ToString(@"00\.000\-000");
    }

    /// <summary>
    /// Formata um telefone quando for possível identificar
    /// uma quantidade válida de dígitos.
    /// </summary>
    public static string Telefone(string? valor)
    {
        var telefone = SomenteNumeros(valor);
        if (telefone.Length == 10)
        {
            return Convert.ToUInt64(telefone)
                .ToString(@"(00) 0000\-0000");
        }
        if (telefone.Length == 11)
        {
            return Convert.ToUInt64(telefone)
                .ToString(@"(00) 00000\-0000");
        }
        return Texto(valor);
    }

    /// <summary>
    /// Formata uma competência opcional no padrão MM/yyyy.
    /// </summary>
    public static string Competencia(DateTime? valor)
    {
        return valor.HasValue
            ? valor.Value.ToString("MM/yyyy", CulturaBrasileira)
            : "-";
    }
}