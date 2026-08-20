using OpenAC.Net.NFSe.Nacional.Common.Model;
using OpenAC.Net.NFSe.Nacional.DANFSe.QuestPDF.Src.Comum;
using OpenAC.Net.NFSe.Nacional.DANFSe.QuestPDF.Src.Utilitarios;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace OpenAC.Net.NFSe.Nacional.DANFSe.QuestPDF.Src.Componentes;

/// <summary>
/// Componente responsável pela renderização dos dados do destinatário da operação no DANFSe.
/// </summary>
internal sealed class Destinatario : IComponent
{
    #region Fields    

    private readonly RTCInfoDest? _destinatario;

    #endregion

    #region Constructors

    /// <summary>
    /// Inicializa uma nova instância do componente de dados do destinatário.
    /// </summary>
    /// <param name="destinatario">Dados do destinatário da operação.</param>
    public Destinatario(RTCInfoDest? destinatario)
    {
        _destinatario = destinatario;
    }

    #endregion Constructors

    #region Methods

    /// <summary>
    /// Renderiza a primeira linha com título, documento e telefone do destinatário.
    /// </summary>
    /// <param name="coluna">Descritor da coluna do QuestPDF.</param>
    private void CriarPrimeiraLinha(ColumnDescriptor coluna)
    {
        coluna
            .Item()
            .Row(linha =>
            {
                SessaoFabrica.Criar(linha.RelativeItem(), DANFSeConstantes.Mensagens.DestinatarioOperacao);
                linha
                    .RelativeItem()
                    .Element(c => CampoFabrica.Criar(c, DANFSeConstantes.Mensagens.CnpjCpfNif, Formatador.ObterDocumento(_destinatario)));
                linha
                    .RelativeItem()
                    .Element(c => CampoFabrica.Criar(c, "", ""));
                linha
                    .RelativeItem()
                    .Element(c => CampoFabrica.Criar(c, DANFSeConstantes.Mensagens.Telefone, Formatador.Telefone(_destinatario?.Telefone)));
            });
    }

    /// <summary>
    /// Renderiza a segunda linha com nome/razão social, município/UF e código IBGE/CEP do destinatário.
    /// </summary>
    /// <param name="coluna">Descritor da coluna do QuestPDF.</param>
    private void CriarSegundaLinhaLinha(ColumnDescriptor coluna)
    {
        var endereco = _destinatario?.Endereco;
        var municipio = (MunicipioNacional?)endereco?.Municipio;
        var municipioTexto = $"{Formatador.Texto(municipio?.CodMunicipio)} / {Formatador.FormatarUfPorCodigoMunipio(municipio?.CodMunicipio)}";
        coluna
            .Item()
            .Row(linha =>
            {
                linha
                    .RelativeItem(DANFSeConstantes.Sessao.TamanhoEspacoConteudoPequeno)
                    .Element(c => CampoFabrica.Criar(c, DANFSeConstantes.Mensagens.NomeOuNomeEmpresarial, Formatador.Texto(_destinatario?.Nome)));
                linha
                    .RelativeItem()
                    .Element(c => CampoFabrica.Criar(c, DANFSeConstantes.Mensagens.MunicipioSiglaUf, municipioTexto));
                linha
                    .RelativeItem()
                    .Element(c => CampoFabrica.Criar(c, DANFSeConstantes.Mensagens.CodigoIbgeCep, $"{Formatador.Texto(municipio?.CodMunicipio)} / {endereco?.Municipio}"));
            });
    }

    /// <summary>
    /// Renderiza a terceira linha com endereço e e-mail do destinatário.
    /// </summary>
    /// <param name="coluna">Descritor da coluna do QuestPDF.</param>
    private void CriarTerceiraLinha(ColumnDescriptor coluna)
    {
        coluna
            .Item()
            .Row(linha =>
            {
                linha
                    .RelativeItem(DANFSeConstantes.Sessao.TamanhoEspacoConteudoPequeno)
                    .Element(c => CampoFabrica.Criar(c, DANFSeConstantes.Mensagens.Endereco, Formatador.FormatarEndereco(_destinatario?.Endereco)));
                linha
                    .RelativeItem()
                    .Element(c => CampoFabrica.Criar(c, DANFSeConstantes.Mensagens.Email, Formatador.Texto(_destinatario?.Email)));
                linha
                    .RelativeItem()
                    .Element(c => CampoFabrica.Criar(c, "", ""));
            });
    }

    /// <summary>
    /// Compõe a seção de dados do destinatário no container informado.
    /// </summary>
    /// <param name="container">Container do QuestPDF onde a seção será renderizada.</param>
    public void Compose(IContainer container)
    {
        var temDestinatario = !string.IsNullOrWhiteSpace(_destinatario?.CNPJ) ||
            !string.IsNullOrWhiteSpace(_destinatario?.CPF) ||
            !string.IsNullOrWhiteSpace(_destinatario?.NIF) ||
            _destinatario?.CodigoNaoNIF != null ||
            !string.IsNullOrWhiteSpace(_destinatario?.Nome) ||
            _destinatario?.Endereco != null ||
            !string.IsNullOrWhiteSpace(_destinatario?.Telefone) ||
            !string.IsNullOrWhiteSpace(_destinatario?.Email);
        if (!temDestinatario)
        {
            container
                .Border(DANFSeConstantes.Sessao.TamanhoBorda)
                .Column(coluna =>
                {
                    coluna
                        .Item()
                        .PaddingHorizontal(DANFSeConstantes.Sessao.TamanhoEspacoConteudoMedio)
                        .PaddingVertical(DANFSeConstantes.Sessao.TamanhoEspacoConteudoPequeno)
                        .Text(DANFSeConstantes.Mensagens.DestinatarioOperacaoNaoIdentificado)
                        .AlignCenter()
                        .FontSize(DANFSeConstantes.Fonte.Campo.TamanhoValor);
                });
            return;
        }
        container
            .Border(DANFSeConstantes.Sessao.TamanhoBorda)
            .Column(coluna =>
            {
                CriarPrimeiraLinha(coluna);
                CriarSegundaLinhaLinha(coluna);
                CriarTerceiraLinha(coluna);
            });
    }

    #endregion
}