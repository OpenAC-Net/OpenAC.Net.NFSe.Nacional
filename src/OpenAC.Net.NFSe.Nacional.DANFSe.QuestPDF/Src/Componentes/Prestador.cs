using OpenAC.Net.NFSe.Nacional.DANFSe.QuestPDF.Src.Comum;
using OpenAC.Net.NFSe.Nacional.DANFSe.QuestPDF.Src.Utilitarios;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace OpenAC.Net.NFSe.Nacional.DANFSe.QuestPDF.Src.Componentes;

/// <summary>
/// Componente responsável pela renderização dos dados do prestador de serviços no DANFSe.
/// </summary>
internal sealed class Prestador : IComponent
{
    #region Fields    

    private readonly Common.Model.NotaFiscalServico _nfs;

    private readonly Common.Model.PrestadorDps _prestador;

    #endregion

    #region Constructors

    /// <summary>
    /// Inicializa uma nova instância do componente de dados do prestador.
    /// </summary>
    /// <param name="nfs">Dados da NFS-e contendo o prestador.</param>
    public Prestador(Common.Model.NotaFiscalServico nfs)
    {
        _nfs = nfs;
        _prestador = _nfs.Informacoes.Dps.Informacoes.Prestador;
    }

    #endregion Constructors

    #region Methods

    /// <summary>
    /// Renderiza a primeira linha com título, documento, inscrição municipal e telefone do prestador.
    /// </summary>
    /// <param name="coluna">Descritor da coluna do QuestPDF.</param>
    private void CriarPrimeiraLinha(ColumnDescriptor coluna)
    {
        coluna
            .Item()
            .Row(linha =>
            {
                SessaoFabrica.Criar(linha.RelativeItem(), DANFSeConstantes.Mensagens.PrestadorFornecedor);
                linha
                    .RelativeItem()
                    .Element(c => CampoFabrica.Criar(c, DANFSeConstantes.Mensagens.CnpjCpfNif, Formatador.ObterDocumento(_prestador)));
                linha
                    .RelativeItem()
                    .Element(c => CampoFabrica.Criar(c, DANFSeConstantes.Mensagens.InscricaoMunicipal, Formatador.Texto(_prestador.InscricaoMunicipal)));
                linha
                    .RelativeItem()
                    .Element(c => CampoFabrica.Criar(c, DANFSeConstantes.Mensagens.Telefone, Formatador.Telefone(_prestador.Telefone)));
            });
    }

    /// <summary>
    /// Renderiza a segunda linha com nome/razão social, município/UF e código IBGE/CEP do emitente.
    /// </summary>
    /// <param name="coluna">Descritor da coluna do QuestPDF.</param>
    private void CriarSegundaLinha(ColumnDescriptor coluna)
    {
        var emitente = _nfs.Informacoes.Emitente;
        var endereco = emitente.Endereco;
        var municipioTexto = $"{Formatador.Texto(_nfs.Informacoes.LocalEmissao)} / {Formatador.Texto(endereco.UF)}";
        var codigoIbgeCep = $"{Formatador.Texto(endereco.CodMunicipio)} / {Formatador.Cep(endereco.CEP)}";
        coluna
            .Item()
            .Row(linha =>
            {
                linha
                    .RelativeItem(DANFSeConstantes.Sessao.TamanhoEspacoConteudoPequeno)
                    .Element(c => CampoFabrica.Criar(c, DANFSeConstantes.Mensagens.NomeOuNomeEmpresarial, Formatador.Texto(emitente.RazaoSocial)));
                linha
                    .RelativeItem()
                    .Element(c => CampoFabrica.Criar(c, DANFSeConstantes.Mensagens.MunicipioSiglaUf, municipioTexto));
                linha
                    .RelativeItem()
                    .Element(c => CampoFabrica.Criar(c, DANFSeConstantes.Mensagens.CodigoIbgeCep, codigoIbgeCep));
            });
    }

    /// <summary>
    /// Renderiza a terceira linha com endereço e e-mail do emitente.
    /// </summary>
    /// <param name="coluna">Descritor da coluna do QuestPDF.</param>
    private void CriarTerceiraLinha(ColumnDescriptor coluna)
    {
        var emitente = _nfs.Informacoes.Emitente;
        var endereco = emitente.Endereco;
        coluna
            .Item()
            .Row(linha =>
            {
                linha
                    .RelativeItem(DANFSeConstantes.Sessao.TamanhoEspacoConteudoPequeno)
                    .Element(c => CampoFabrica.Criar(c, DANFSeConstantes.Mensagens.Endereco, Formatador.FormatarEndereco(endereco)));
                linha
                    .RelativeItem()
                    .Element(c => CampoFabrica.Criar(c, DANFSeConstantes.Mensagens.Email, Formatador.Texto(emitente.Email)));
                linha
                    .RelativeItem()
                    .Element(c => CampoFabrica.Criar(c, "", ""));
            });
    }

    /// <summary>
    /// Renderiza a quarta linha com informações do Simples Nacional e regime de apuração.
    /// </summary>
    /// <param name="coluna">Descritor da coluna do QuestPDF.</param>
    private void CriarQuartaLinha(ColumnDescriptor coluna)
    {
        coluna
            .Item()
            .Row(linha =>
            {
                linha
                    .RelativeItem()
                    .Element(c => CampoFabrica.Criar(c, DANFSeConstantes.Mensagens.SimplesNacionalDataCompetencia, Formatador.FormatarSimplesNacional(_prestador.Regime.OptanteSimplesNacional)));
                linha
                    .RelativeItem(DANFSeConstantes.Sessao.TamanhoEspacoConteudoPequeno)
                    .Element(c => CampoFabrica.Criar(c, DANFSeConstantes.Mensagens.RegimeApuracaoTributaria, Formatador.FormatarRegimeApuracao(_prestador.Regime.RegimeApuracao)));
                linha
                    .RelativeItem()
                    .Element(c => CampoFabrica.Criar(c, "", ""));
            });
    }

    /// <summary>
    /// Compõe a seção de dados do prestador no container informado.
    /// </summary>
    /// <param name="container">Container do QuestPDF onde a seção será renderizada.</param>
    public void Compose(IContainer container)
    {
        container
            .Border(DANFSeConstantes.Sessao.TamanhoBorda)
            .Column(coluna =>
            {
                CriarPrimeiraLinha(coluna);
                CriarSegundaLinha(coluna);
                CriarTerceiraLinha(coluna);
                CriarQuartaLinha(coluna);
            });
    }

    #endregion
}