using OpenAC.Net.NFSe.Nacional.Common.Types;
using OpenAC.Net.NFSe.Nacional.DANFSe.QuestPDF.Src.Utilitarios;
using OpenAC.Net.NFSe.Nacional.DANFSe.QuestPDF.Tests.Src.Comum;

namespace OpenAC.Net.NFSe.Nacional.DANFSe.QuestPDF.Tests.Src;

public class FormatadorModeloTests
{
    [Theory]
    [InlineData(AmbienteGerador.Prefeitura, "Prefeitura Municipal")]
    [InlineData(AmbienteGerador.Nacional, "Sefin Nacional NFS-e")]
    public void Deve_Formatar_Ambiente(AmbienteGerador ambiente, string esperado)
    {
        Assert.Equal(esperado, Formatador.FormatarAmbiente(ambiente));
    }

    [Theory]
    [InlineData(TipoEmissao.ModeloNacional, "Nacional")]
    [InlineData(TipoEmissao.ModeloMunicipio, "município")]
    public void Deve_Formatar_Tipo_Emissao(TipoEmissao tipoEmissao, string esperado)
    {
        Assert.Equal(esperado, Formatador.FormatarTipoEmissao(tipoEmissao));
    }

    [Fact]
    public void Deve_Obter_Documento_De_Pessoa()
    {
        var documento = Formatador.ObterDocumento(ModeloDados.Prestador);
        Assert.NotEqual("-", documento);
        Assert.NotNull(documento);
    }

    [Fact]
    public void Deve_Obter_Documento_De_Destinatario()
    {
        var documento = Formatador.ObterDocumento(ModeloDados.Destinatario);
        Assert.NotNull(documento);
    }

    [Fact]
    public void Deve_Format_Endereco_De_Pessoa()
    {
        var endereco = Formatador.FormatarEndereco(ModeloDados.EnderecoTomador);
        Assert.NotEqual("-", endereco);
    }

    [Fact]
    public void Deve_Format_Endereco_De_Emitente()
    {
        var endereco = Formatador.FormatarEndereco(ModeloDados.EnderecoEmitente);
        Assert.NotEqual("-", endereco);
    }

    [Theory]
    [InlineData(EmitenteDps.Prestador, "Prestador")]
    [InlineData(EmitenteDps.Tomador, "Tomador")]
    [InlineData(EmitenteDps.Intermediário, "Intermediário")]
    public void Deve_Format_Emitente(EmitenteDps emitente, string esperado)
    {
        Assert.Equal(esperado, Formatador.FormatarEmitente(emitente));
    }

    [Fact]
    public void Deve_Format_Status_Nfse()
    {
        Assert.Equal("NFS-e Gerada", Formatador.FormatarStatus(StatusNFSe.Gerada));
    }

    [Fact]
    public void Deve_Format_Finalidade()
    {
        Assert.Equal("NFS-e regular", Formatador.FormatarFinalidade(RTCFinNFSe.Regular));
        Assert.Equal("-", Formatador.FormatarFinalidade(null));
    }

    [Theory]
    [InlineData(RegimeApuracao.TributosFederaisMunicipalSN, "Regime de apuração dos tributos federais e municipal pelo Simples Nacional.")]
    [InlineData(RegimeApuracao.TributosFederaisSNISSQNPorForaSN, "Regime de apuração dos tributos federais pelo Simples Nacional e ISSQN por fora do Simples Nacional conforme legislação municipal.")]
    [InlineData(RegimeApuracao.TributosFederaisMunicipalForaSN, "Regime de apuração dos tributos federais e municipal por fora do Simples Nacional conforme legislações federal e municipal.")]
    public void Deve_Formatar_Regime_Apuracao(RegimeApuracao regime, string esperado)
    {
        Assert.Equal(esperado, Formatador.FormatarRegimeApuracao(regime));
    }

    [Fact]
    public void Deve_Retornar_Hifen_Quando_Regime_Apuracao_For_Nulo()
    {
        Assert.Equal("-", Formatador.FormatarRegimeApuracao(null));
    }

    [Theory]
    [InlineData(OptanteSimplesNacional.NaoOptante, "Não optante")]
    [InlineData(OptanteSimplesNacional.OptanteMEI, "Optante MEI")]
    [InlineData(OptanteSimplesNacional.OptanteMEEPP, "Optante Micro empresa/Empresa pequeno porte")]
    public void Deve_Formatar_Simples_Nacional(OptanteSimplesNacional opcao, string esperado)
    {
        Assert.Equal(esperado, Formatador.FormatarSimplesNacional(opcao));
    }

    [Theory]
    [InlineData("123456", "12.34.56")]
    [InlineData("123", "-")]
    [InlineData(null, "-")]
    public void Deve_Formatar_Codigo_Tributacao_Nacional(string? codigo, string esperado)
    {
        Assert.Equal(esperado, Formatador.FormatarCodigoTributacaoNacional(codigo));
    }

    [Theory]
    [InlineData("123456789", "1.2345.67.89")]
    [InlineData("123", "-")]
    [InlineData(null, "-")]
    public void Deve_Formatar_Codigo_Nbs(string? codigo, string esperado)
    {
        Assert.Equal(esperado, Formatador.FormatarCodigoNbs(codigo));
    }

    [Theory]
    [InlineData("3550308", "SP")]
    [InlineData("12", "AC")]
    [InlineData("1", "-")]
    [InlineData(null, "-")]
    public void Deve_Formatar_Uf_Por_Codigo_Municipio(string? codigo, string esperado)
    {
        Assert.Equal(esperado, Formatador.FormatarUfPorCodigoMunipio(codigo));
    }

    [Theory]
    [InlineData(TributoISSQN.OperacaoTributavel, "Operação tributável")]
    [InlineData(TributoISSQN.Imunidade, "Imunidade")]
    [InlineData(TributoISSQN.ExportacaoServico, "Exportação de serviço")]
    [InlineData(TributoISSQN.NaoIncidencia, "Não incidencia")]
    public void Deve_Formatar_Tributo_Issqn(TributoISSQN tributo, string esperado)
    {
        Assert.Equal(esperado, Formatador.FormatarTributoISSQN(tributo));
    }

    [Theory]
    [InlineData(RegimeEspecial.Nenhum, "Nenhum")]
    [InlineData(RegimeEspecial.Cooperativa, "Cooperativa")]
    [InlineData(RegimeEspecial.Estimativa, "Estimativa")]
    [InlineData(RegimeEspecial.MicroempresaMunicipal, "Microempresa Municipal")]
    [InlineData(RegimeEspecial.NotarioRegistrador, "Notário ou Registrador")]
    [InlineData(RegimeEspecial.ProfissionalAutonomo, "Profissional Autônomo")]
    [InlineData(RegimeEspecial.SociedadeProfissionais, "Sociedade de Profissionais")]
    public void Deve_Formatar_Regime_Especial_Issqn(RegimeEspecial regime, string esperado)
    {
        Assert.Equal(esperado, Formatador.FormatarRegimeEspecialISSQN(regime));
    }

    [Fact]
    public void Deve_Retornar_Hifen_Quando_Regime_Especial_Issqn_For_Nulo()
    {
        Assert.Equal("-", Formatador.FormatarRegimeEspecialISSQN(null));
    }

    [Theory]
    [InlineData(TipoImunidade.Imunidade, "Não informada")]
    [InlineData(TipoImunidade.CF88Art150VIa, "Órgãos Públicos (CF88, Art. 150, VI, a)")]
    [InlineData(TipoImunidade.CF88Art150VIb, "Templos Religiosos (CF88, Art. 150, VI, b)")]
    [InlineData(TipoImunidade.CF88Art150VIc, "Partidos, Sindicatos e Entidades sem Fins Lucrativos (CF88, Art. 150, VI, c)")]
    [InlineData(TipoImunidade.CF88Art150VId, "Livros, Jornais e Periódicos (CF88, Art. 150, VI, d)")]
    [InlineData(TipoImunidade.CF88Art150VIe, "Fonogramas e Mídias Nacionais (CF88, Art. 150, VI, e)")]
    public void Deve_Formatar_Tipo_Imunidade_Issqn(TipoImunidade tipo, string esperado)
    {
        Assert.Equal(esperado, Formatador.FormatarTipoImunidadeISSQN(tipo));
    }

    [Fact]
    public void Deve_Retornar_Hifen_Quando_Tipo_Imunidade_For_Nulo()
    {
        Assert.Equal("-", Formatador.FormatarTipoImunidadeISSQN(null));
    }

    [Theory]
    [InlineData(TipoSuspensao.PorDecisaoJudicial, "Exigibilidade Suspensa por Decisão Judicial.")]
    [InlineData(TipoSuspensao.PorProcessoAdministrativo, "Exigibilidade Suspensa por Processo Administrativo.")]
    public void Deve_Formatar_Suspensao_Exigibilidade_Issqn(TipoSuspensao tipo, string esperado)
    {
        Assert.Equal(esperado, Formatador.FormatarSuspensaoExibilidadeISSQN(tipo));
    }

    [Fact]
    public void Deve_Retornar_Hifen_Quando_Suspensao_Exigibilidade_For_Nula()
    {
        Assert.Equal("-", Formatador.FormatarSuspensaoExibilidadeISSQN(null));
    }

    [Theory]
    [InlineData(TipoBeneficioMunicipal.Isencao, "Isenção")]
    [InlineData(TipoBeneficioMunicipal.ReducaoBCPerc, "Redução da BC em 'ppBM' %")]
    [InlineData(TipoBeneficioMunicipal.ReducaoBCValor, "Redução da BC em R$ 'vInfoBM'")]
    [InlineData(TipoBeneficioMunicipal.AliquotaDiferenciada, "Alíquota Diferenciada de 'aliqDifBM' %")]
    public void Deve_Formatar_Tipo_Beneficio_Municipal(TipoBeneficioMunicipal tipo, string esperado)
    {
        Assert.Equal(esperado, Formatador.FormatarTipoBeneficioMunicipal(tipo));
    }

    [Fact]
    public void Deve_Retornar_Hifen_Quando_Tipo_Beneficio_Municipal_For_Nulo()
    {
        Assert.Equal("-", Formatador.FormatarTipoBeneficioMunicipal(null));
    }

    [Theory]
    [InlineData(TipoRetencaoISSQN.NaoRetido, "Não retido")]
    [InlineData(TipoRetencaoISSQN.RetidoTomador, "Retido pelo tomador")]
    [InlineData(TipoRetencaoISSQN.RetidoIntermediario, "Retido pelo intermediário")]
    public void Deve_Formatar_Tipo_Retencao_Issqn(TipoRetencaoISSQN tipo, string esperado)
    {
        Assert.Equal(esperado, Formatador.FormatarTipoRetencaoISSQN(tipo));
    }

    [Fact]
    public void Deve_Retornar_Hifen_Quando_Tipo_Retencao_Issqn_For_Nulo()
    {
        Assert.Equal("-", Formatador.FormatarTipoRetencaoISSQN(null));
    }

    [Theory]
    [InlineData(TipoRetencaoPisCofinsCsll.PisCofinsCsllNaoRetidos, "PIS/COFINS/CSLL Não Retidos")]
    [InlineData(TipoRetencaoPisCofinsCsll.PisCofinsRetidos, "PIS/COFINS Retidos")]
    [InlineData(TipoRetencaoPisCofinsCsll.PisCofinsNaoRetidos, "PIS/COFINS Não Retidos")]
    [InlineData(TipoRetencaoPisCofinsCsll.PisCofinsCsllRetidos, "PIS/COFINS/CSLL Retidos")]
    [InlineData(TipoRetencaoPisCofinsCsll.PisCofinsRetidosCsllNaoRetido, "PIS/COFINS Retidos, CSLL Não Retido")]
    [InlineData(TipoRetencaoPisCofinsCsll.PisRetidoCofinsCsllNaoRetidos, "PIS Retido, COFINS/CSLL Não Retidos")]
    [InlineData(TipoRetencaoPisCofinsCsll.CofinsRetidoPisCSllNaoRetidos, "COFINS Retido, PIS/CSLL Não Retidos")]
    [InlineData(TipoRetencaoPisCofinsCsll.PisNaoRetidoCofinsCsllRetidos, "PIS Não Retido, COFINS/CSLL Retidos")]
    [InlineData(TipoRetencaoPisCofinsCsll.PisCofinsNaoRetidosCsllRetido, "PIS/COFINS Não Retidos, CSLL Retido")]
    [InlineData(TipoRetencaoPisCofinsCsll.CofinsNaoRetidoPisCSllRetidos, "COFINS Não Retido, PIS/CSLL Retidos")]
    public void Deve_Formatar_Tipo_Retencao_Pis_Cofins_Csll(TipoRetencaoPisCofinsCsll tipo, string esperado)
    {
        Assert.Equal(esperado, Formatador.FormatarTipoRetencaoPisCofinsCsll(tipo));
    }
}
