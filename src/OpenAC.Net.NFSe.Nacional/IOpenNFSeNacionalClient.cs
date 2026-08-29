using System.Threading;
using System.Threading.Tasks;
using OpenAC.Net.NFSe.Nacional.Common.Model;

namespace OpenAC.Net.NFSe.Nacional;

/// <summary>Contrato assíncrono da integração com a NFS-e Nacional.</summary>
public interface IOpenNFSeNacionalClient
{
    /// <summary>Envia uma DPS para emissão síncrona da NFS-e.</summary>
    /// <param name="dps">Declaração de Prestação de Serviços que será enviada.</param>
    /// <param name="cancellationToken">Token para cancelar a operação.</param>
    /// <returns>Resposta da emissão contendo o resultado fiscal retornado pelo provedor.</returns>
    Task<NFSeResponse<RespostaEnvioDps>> EnviarAsync(Dps dps, CancellationToken cancellationToken = default);

    /// <summary>Envia um pedido de registro de evento da NFS-e.</summary>
    /// <param name="evento">Pedido de evento, como cancelamento ou substituição.</param>
    /// <param name="cancellationToken">Token para cancelar a operação.</param>
    /// <returns>Resposta fiscal do processamento do evento.</returns>
    Task<NFSeResponse<RespostaEnvioEvento>> EnviarEventoAsync(PedidoRegistroEvento evento, CancellationToken cancellationToken = default);

    /// <summary>Consulta documentos fiscais distribuídos a partir de um NSU.</summary>
    /// <param name="nsu">Número Sequencial Único inicial da consulta.</param>
    /// <param name="cancellationToken">Token para cancelar a operação.</param>
    /// <returns>Documentos fiscais encontrados e informações da distribuição.</returns>
    Task<NFSeResponse<RespostaConsultaDFe>> ConsultaNsuAsync(int nsu, CancellationToken cancellationToken = default);

    /// <summary>Consulta a distribuição de um documento pela chave de acesso.</summary>
    /// <param name="chave">Chave de acesso da NFS-e.</param>
    /// <param name="cancellationToken">Token para cancelar a operação.</param>
    /// <returns>Documento fiscal e informações retornadas pela distribuição.</returns>
    Task<NFSeResponse<RespostaConsultaDFe>> ConsultaChaveAsync(string chave, CancellationToken cancellationToken = default);

    /// <summary>Baixa o Documento Auxiliar da NFS-e.</summary>
    /// <param name="chave">Chave de acesso da NFS-e.</param>
    /// <param name="cancellationToken">Token para cancelar a operação.</param>
    /// <returns>Conteúdo binário do DANFSe.</returns>
    Task<byte[]> DownloadDANFSeAsync(string chave, CancellationToken cancellationToken = default);

    /// <summary>Consulta a chave da NFS-e gerada a partir do identificador da DPS.</summary>
    /// <param name="id">Identificador da DPS.</param>
    /// <param name="cancellationToken">Token para cancelar a operação.</param>
    /// <returns>Resposta contendo a chave encontrada.</returns>
    Task<NFSeResponse<RespostaConsultaChaveDps>> ConsultaChaveDpsAsync(string id, CancellationToken cancellationToken = default);

    /// <summary>Verifica se existe NFS-e emitida para uma DPS.</summary>
    /// <param name="id">Identificador da DPS.</param>
    /// <param name="cancellationToken">Token para cancelar a operação.</param>
    /// <returns><see langword="true"/> quando a NFS-e existir; caso contrário, <see langword="false"/>.</returns>
    Task<bool> ConsultaExisteDpsAsync(string id, CancellationToken cancellationToken = default);

    /// <summary>Envia um lote assíncrono de DPS.</summary>
    /// <param name="lote">Lote que será transmitido.</param>
    /// <param name="cancellationToken">Token para cancelar a operação.</param>
    /// <returns>Resposta de recepção e protocolo do lote.</returns>
    Task<NFSeResponse<RespostaRecepcaoLote>> EnviarLoteAsync(LoteDps lote, CancellationToken cancellationToken = default);

    /// <summary>Envia um lote síncrono de DPS.</summary>
    /// <param name="lote">Lote que será transmitido.</param>
    /// <param name="cancellationToken">Token para cancelar a operação.</param>
    /// <returns>Resposta do processamento síncrono do lote.</returns>
    Task<NFSeResponse<RespostaRecepcaoLoteSincrono>> EnviarLoteSincronoAsync(LoteDps lote, CancellationToken cancellationToken = default);

    /// <summary>Consulta o processamento de um lote anteriormente enviado.</summary>
    /// <param name="filtro">Critérios de identificação do lote.</param>
    /// <param name="cancellationToken">Token para cancelar a operação.</param>
    /// <returns>Situação e documentos resultantes do lote.</returns>
    Task<NFSeResponse<RespostaConsultaLote>> ConsultarLoteAsync(ConsultaLoteFiltro filtro, CancellationToken cancellationToken = default);

    /// <summary>Consulta NFS-e conforme os critérios suportados pelo provedor.</summary>
    /// <param name="filtro">Critérios da consulta de NFS-e.</param>
    /// <param name="cancellationToken">Token para cancelar a operação.</param>
    /// <returns>Notas fiscais encontradas pelo provedor.</returns>
    Task<NFSeResponse<RespostaConsultaNFSe>> ConsultarNFSeAsync(ConsultaNFSeFiltro filtro, CancellationToken cancellationToken = default);
}
