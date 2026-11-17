using System;
using System.IO;
using OpenAC.Net.DFe.Core.Storage;
using OpenAC.Net.NFSe.Nacional.Common;

namespace OpenAC.Net.NFSe.Nacional.Storage;

/// <summary>Arquivo da NFS-e com os metadados necessários à organização fiscal e ao isolamento no host web.</summary>
public sealed class NFSeDocument : IDFeDocument
{
    /// <summary>Cria um documento com as regras de arquivos e os metadados da operação.</summary>
    /// <param name="arquivos">Configuração de arquivos associada à empresa.</param>
    /// <param name="categoria">Categoria fiscal ou de comunicação.</param>
    /// <param name="conteudo">Conteúdo textual a persistir.</param>
    /// <param name="nomeArquivo">Nome do arquivo, incluindo a extensão, sem diretórios.</param>
    /// <param name="documentoContribuinte">CPF ou CNPJ usado para organizar diretórios.</param>
    /// <param name="data">Data de referência do documento.</param>
    /// <param name="arquivoExiste">Comportamento quando o destino já existe.</param>
    public NFSeDocument(NFSeArquivoConfig arquivos, string categoria, string conteudo, string nomeArquivo,
        string? documentoContribuinte, DateTime data, DFeArquivoExiste arquivoExiste = DFeArquivoExiste.AdicionarSufixo)
    {
        Arquivos = arquivos ?? throw new ArgumentNullException(nameof(arquivos));
        if (string.IsNullOrWhiteSpace(categoria) || categoria == "." || categoria == ".." ||
            categoria.IndexOfAny(['/', '\\', ':']) >= 0 || categoria.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
            throw new ArgumentException("Informe uma categoria válida, sem diretórios.", nameof(categoria));
        if (string.IsNullOrWhiteSpace(nomeArquivo) || nomeArquivo == "." || nomeArquivo == ".." ||
            nomeArquivo.IndexOfAny(['/', '\\', ':']) >= 0 || nomeArquivo.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
            throw new ArgumentException("Informe apenas o nome do arquivo, sem diretórios.", nameof(nomeArquivo));
        if (!Enum.IsDefined(typeof(DFeArquivoExiste), arquivoExiste))
            throw new ArgumentOutOfRangeException(nameof(arquivoExiste));
        Categoria = categoria;
        Conteudo = conteudo ?? throw new ArgumentNullException(nameof(conteudo));
        NomeArquivo = nomeArquivo;
        DocumentoContribuinte = documentoContribuinte;
        Data = data;
        ArquivoExiste = arquivoExiste;
    }

    /// <summary>Configuração de arquivos que identifica as regras e a empresa do documento.</summary>
    public NFSeArquivoConfig Arquivos { get; }

    /// <summary>Categoria fiscal ou de comunicação.</summary>
    public string Categoria { get; }

    /// <summary>Nome do arquivo, incluindo a extensão.</summary>
    public string NomeArquivo { get; }

    /// <summary>CPF ou CNPJ usado na organização dos diretórios.</summary>
    public string? DocumentoContribuinte { get; }

    /// <summary>Data de referência do documento.</summary>
    public DateTime Data { get; }

    /// <summary>Caminho completo resolvido pelas regras do componente quando consultado.</summary>
    public string Caminho => Path.GetFullPath(Path.Combine(
        Arquivos.GetDocumentPath(Categoria, DocumentoContribuinte, Data), NomeArquivo));

    /// <inheritdoc />
    public string Conteudo { get; }

    /// <inheritdoc />
    public DFeArquivoExiste ArquivoExiste { get; }
}
