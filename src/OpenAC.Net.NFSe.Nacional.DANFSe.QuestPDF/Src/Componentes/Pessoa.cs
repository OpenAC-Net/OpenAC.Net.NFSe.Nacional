using OpenAC.Net.NFSe.Nacional.Common.Model;
using OpenAC.Net.NFSe.Nacional.DANFSe.QuestPDF.Src.Comum;
using OpenAC.Net.NFSe.Nacional.DANFSe.QuestPDF.Src.Utilitarios;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace OpenAC.Net.NFSe.Nacional.DANFSe.QuestPDF.Src.Componentes;

/// <summary>
/// Componente responsável pela renderização dos dados de uma pessoa
/// (tomador, intermediário ou outra) no DANFSe.
/// </summary>
internal sealed class Pessoa : IComponent
{
    #region Fields    

    private readonly string _titulo;

    private readonly string _informacaoNaoExistente;

    private readonly InfoPessoaNFSe? _pessoa;

    #endregion

    #region Constructors

    /// <summary>
    /// Inicializa uma nova instância do componente de dados de pessoa.
    /// </summary>
    /// <param name="titulo">Título da seção.</param>
    /// <param name="informacaoNaoExistente">Texto exibido quando não houver dados da pessoa.</param>
    /// <param name="pessoa">Dados da pessoa a serem apresentados.</param>
    public Pessoa(
        string titulo,
        string informacaoNaoExistente,
        InfoPessoaNFSe? pessoa)
    {
        _titulo = titulo;
        _informacaoNaoExistente = informacaoNaoExistente;
        _pessoa = pessoa;
    }

    #endregion Constructors

    #region Methods

    /// <summary>
    /// Renderiza a primeira linha com título, documento, inscrição municipal e telefone.
    /// </summary>
    /// <param name="coluna">Descritor da coluna do QuestPDF.</param>
    private void CriarPrimeiraLinha(ColumnDescriptor coluna)
    {
        coluna
            .Item()
            .Row(linha =>
            {
                SessaoFabrica.Criar(linha.RelativeItem(), _titulo);
                linha
                    .RelativeItem()
                    .Element(c => CampoFabrica.Criar(c, DANFSeConstantes.Mensagens.CnpjCpfNif, Formatador.ObterDocumento(_pessoa)));
                linha
                    .RelativeItem()
                    .Element(c => CampoFabrica.Criar(c, DANFSeConstantes.Mensagens.InscricaoMunicipal, Formatador.Texto(_pessoa?.InscricaoMunicipal)));
                linha
                    .RelativeItem()
                    .Element(c => CampoFabrica.Criar(c, DANFSeConstantes.Mensagens.Telefone, Formatador.Telefone(_pessoa?.Telefone)));
            });
    }

    /// <summary>
    /// Renderiza a segunda linha com nome/razão social, município/UF e código IBGE/CEP.
    /// </summary>
    /// <param name="coluna">Descritor da coluna do QuestPDF.</param>
    private void CriarSegundaLinhaLinha(ColumnDescriptor coluna)
    {
        var endereco = _pessoa?.Endereco;
        var municipio = (MunicipioNacional?)endereco?.Municipio;
        var municipioTexto = $"{Formatador.Texto(municipio?.CodMunicipio)} / {Formatador.FormatarUfPorCodigoMunipio(municipio?.CodMunicipio)}";
        var codigoIbgeCep = $"{Formatador.Texto(municipio?.CodMunicipio)} / {Formatador.Cep(municipio?.CEP)}";
        coluna
            .Item()
            .Row(linha =>
            {
                linha
                    .RelativeItem(2)
                    .Element(c => CampoFabrica.Criar(c, DANFSeConstantes.Mensagens.NomeOuNomeEmpresarial, Formatador.Texto(_pessoa?.Nome)));
                linha
                    .RelativeItem()
                    .Element(c => CampoFabrica.Criar(c, DANFSeConstantes.Mensagens.MunicipioSiglaUf, municipioTexto));
                linha
                    .RelativeItem()
                    .Element(c => CampoFabrica.Criar(c, DANFSeConstantes.Mensagens.CodigoIbgeCep, codigoIbgeCep));
            });
    }

    /// <summary>
    /// Renderiza a terceira linha com endereço e e-mail.
    /// </summary>
    /// <param name="coluna">Descritor da coluna do QuestPDF.</param>
    private void CriarTerceiraLinha(ColumnDescriptor coluna)
    {
        coluna
            .Item()
            .Row(linha =>
            {
                linha
                    .RelativeItem()
                    .Element(c => CampoFabrica.Criar(c, DANFSeConstantes.Mensagens.Endereco, Formatador.FormatarEndereco(_pessoa?.Endereco)));
                linha
                    .RelativeItem()
                    .Element(c => CampoFabrica.Criar(c, DANFSeConstantes.Mensagens.Email, Formatador.Texto(_pessoa?.Email)));
            });
    }

    /// <summary>
    /// Compõe a seção de dados da pessoa no container informado.
    /// </summary>
    /// <param name="container">Container do QuestPDF onde a seção será renderizada.</param>
    public void Compose(IContainer container)
    {
        var temPessoa = !string.IsNullOrWhiteSpace(_pessoa?.CNPJ) ||
            !string.IsNullOrWhiteSpace(_pessoa?.CPF) ||
            !string.IsNullOrWhiteSpace(_pessoa?.Nif) ||
            _pessoa?.CodigoNaoNif != null ||
            !string.IsNullOrWhiteSpace(_pessoa?.NumeroCAEPF) ||
            !string.IsNullOrWhiteSpace(_pessoa?.InscricaoMunicipal) ||
            !string.IsNullOrWhiteSpace(_pessoa?.Nome) ||
            _pessoa?.Endereco != null ||
            !string.IsNullOrWhiteSpace(_pessoa?.Telefone) ||
            !string.IsNullOrWhiteSpace(_pessoa?.Email);
        if (!temPessoa)
        {
            container
                .Border(DANFSeConstantes.Sessao.TamanhoBorda)
                .Column(coluna =>
                {
                    coluna
                        .Item()
                        .PaddingHorizontal(DANFSeConstantes.Sessao.TamanhoEspacoConteudoMedio)
                        .PaddingVertical(DANFSeConstantes.Sessao.TamanhoEspacoConteudoPequeno)
                        .Text(_informacaoNaoExistente)
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