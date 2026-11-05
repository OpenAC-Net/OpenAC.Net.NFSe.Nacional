// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe.Nacional
// Author           : OpenAC .Net
// ***********************************************************************
// <summary>
// Montagem da NFS-e (layout nacional) exigida pelo webservice da SIL Tecnologia.
// </summary>
// ***********************************************************************

using System;
using System.Linq;
using OpenAC.Net.Core.Extensions;
using OpenAC.Net.DFe.Core.Common;
using OpenAC.Net.DFe.Core.Extensions;
using OpenAC.Net.NFSe.Nacional.Common.Model;
using OpenAC.Net.NFSe.Nacional.Common.Types;

namespace OpenAC.Net.NFSe.Nacional.Webservice.SilTecnologia;

/// <summary>
/// Monta a <see cref="NotaFiscalServico"/> enviada ao webservice da SIL Tecnologia a partir de uma DPS.
/// </summary>
/// <remarks>
/// A SIL não recebe a DPS: o contribuinte envia a NFS-e inteira (<c>infNFSe</c> com a DPS dentro),
/// preenchendo os campos que no Emissor Nacional seriam gerados pelo fisco. Os valores fixos seguem
/// a documentação da SIL (https://interface.siltecnologia.com.br/notafiscalnacional/):
/// <c>Id</c> = "NFS" + 50 zeros, <c>nNFSe</c> = 0, <c>ambGer</c> = 1, <c>tpEmis</c> = 2, <c>cStat</c> = 100
/// e <c>verAplic</c> = <see cref="VersaoAplicacao"/>.
/// </remarks>
public static class SilTecnologiaNFSe
{
    #region Constants

    /// <summary>Versão do aplicativo exigida pela SIL em <c>infNFSe/verAplic</c> e <c>infDPS/verAplic</c>.</summary>
    public const string VersaoAplicacao = "SilTecnologia_v1.00";

    #endregion Constants

    #region Methods

    /// <summary>
    /// Monta a NFS-e a partir da DPS.
    /// </summary>
    /// <param name="dps">DPS de origem. Não é alterada; a NFS-e recebe uma cópia.</param>
    /// <param name="nomeMunicipio">Retorna o nome do município a partir do código IBGE (7 dígitos).</param>
    /// <param name="dataProcessamento">Data/hora informada em <c>dhProc</c>.</param>
    /// <param name="codMunicipioIncidencia">
    /// Código IBGE do município de incidência do ISSQN (<c>cLocIncid</c>). Quando vazio, usa o município do emitente.
    /// </param>
    /// <returns>NFS-e pronta para assinatura.</returns>
    /// <remarks>
    /// <list type="bullet">
    /// <item><c>emit</c> usa o nome e o endereço informados em <c>prest</c> da DPS, que são retirados da
    /// cópia da DPS embutida (os modelos da SIL não trazem <c>xNome</c>/<c>end</c> no prestador).</item>
    /// <item><c>pAliqAplic</c> usa a alíquota informada em <c>tribMun/pAliq</c> da DPS. Quando o ISSQN não é
    /// retido, a alíquota é retirada da cópia da DPS embutida (os modelos da SIL não trazem <c>pAliq</c> nesse caso).</item>
    /// <item><c>xTribNac</c> recebe a descrição do serviço (<c>cServ/xDescServ</c>), limitada a 600 caracteres.</item>
    /// <item><c>cLocIncid</c> recebe o município informado em <paramref name="codMunicipioIncidencia"/>. A regra
    /// de incidência (LC 116/2003, art. 3º) fica com quem emite.</item>
    /// <item>Valores calculados conforme a seção 10 da documentação da SIL.</item>
    /// </list>
    /// Para outro preenchimento, ajuste a NFS-e retornada e envie com
    /// <see cref="SilTecnologiaWebService.EnviarAsync(NotaFiscalServico, System.Threading.CancellationToken)"/>.
    /// </remarks>
    public static NotaFiscalServico Montar(Dps dps, Func<string, string> nomeMunicipio, DateTimeOffset dataProcessamento,
        string? codMunicipioIncidencia = null)
    {
        // A DPS vai sem assinatura própria: a SIL assina só infNFSe.
        var copia = Dps.Load(dps.GetXml());
        copia.Signature = null!;
        var inf = copia.Informacoes;
        var prestador = inf.Prestador;

        var municipioPrestador = prestador.Endereco?.Municipio as MunicipioNacional;
        if (string.IsNullOrEmpty(prestador.Nome) || prestador.Endereco == null || municipioPrestador == null)
            throw new InvalidOperationException(
                "Informe o nome (Prestador.Nome) e o endereço nacional (Prestador.Endereco com MunicipioNacional) " +
                "do prestador na DPS. A SIL Tecnologia exige esses dados no grupo emit da NFS-e.");

        var emitente = new EmitenteNFSe
        {
            CNPJ = prestador.CNPJ,
            CPF = prestador.CPF,
            InscricaoMunicipal = prestador.InscricaoMunicipal,
            RazaoSocial = prestador.Nome!,
            Endereco = new EnderecoEmitente
            {
                Logradouro = prestador.Endereco.Logradouro,
                Numero = prestador.Endereco.Numero,
                Complemento = prestador.Endereco.Complemento,
                Bairro = prestador.Endereco.Bairro,
                CodMunicipio = municipioPrestador.CodMunicipio,
                UF = SiglaUF(municipioPrestador.CodMunicipio),
                CEP = municipioPrestador.CEP
            },
            Telefone = prestador.Telefone,
            Email = prestador.Email
        };

        prestador.Nome = null;
        prestador.Endereco = null;
        inf.VersaoAplicacao = VersaoAplicacao;

        var codLocalPrestacao = inf.Servico.Localidade.CodMunicipioPrestacao;
        if (string.IsNullOrEmpty(codLocalPrestacao))
            throw new InvalidOperationException(
                "Prestação no exterior (cPaisPrestacao) não é montada automaticamente para a SIL Tecnologia. " +
                "Monte a NotaFiscalServico e envie com SilTecnologiaWebService.EnviarAsync(NotaFiscalServico).");

        var codIncidencia = string.IsNullOrEmpty(codMunicipioIncidencia) ? inf.LocalidadeEmitente : codMunicipioIncidencia!;
        var descricao = inf.Servico.Informacoes.Descricao;

        var valores = CalcularValores(inf.Valores);
        var municipal = inf.Valores.Tributos.Municipal;
        if (municipal.TipoRetencaoISSQN is null or TipoRetencaoISSQN.NaoRetido)
            municipal.Aliquota = null;

        return new NotaFiscalServico
        {
            Versao = copia.Versao.GetDFeValue(),
            Informacoes = new InfNFSe
            {
                Id = "NFS" + new string('0', 50),
                LocalEmissao = nomeMunicipio(inf.LocalidadeEmitente),
                LocalPrestacao = nomeMunicipio(codLocalPrestacao!),
                NumeroNFSe = 0,
                CodLocalIncidencia = codIncidencia,
                LocalIncidencia = nomeMunicipio(codIncidencia),
                DescricaoTributoNacional = descricao.Length > 600 ? descricao.Substring(0, 600) : descricao,
                VersaoAplicacao = VersaoAplicacao,
                AmbienteGerador = AmbienteGerador.Prefeitura,
                TipoEmissao = TipoEmissao.ModeloMunicipio,
                ProcessoEmissao = ProcessoEmissao.AplicativoContribuinte,
                SituacaoNFSe = StatusNFSe.Gerada,
                DhProcessamento = dataProcessamento,
                NumeroDFSe = int.TryParse(inf.NumeroDps, out var numeroDps) ? numeroDps : 0,
                Emitente = emitente,
                Valores = valores,
                Dps = copia
            }
        };
    }

    /// <summary>
    /// Calcula o grupo <c>infNFSe/valores</c> conforme a seção 10 da documentação da SIL.
    /// </summary>
    /// <param name="valores">Valores da DPS.</param>
    /// <returns>Valores da NFS-e.</returns>
    /// <remarks>
    /// <list type="bullet">
    /// <item><c>vBC</c> = vServ - vDR - vDescIncond.</item>
    /// <item><c>pAliqAplic</c> = pAliq da DPS (0 quando não informada).</item>
    /// <item><c>vISSQN</c> = vBC x pAliqAplic / 100, somente quando <c>tribISSQN</c> = 1.</item>
    /// <item><c>vTotalRet</c> = vRetCP + vRetIRRF + vRetCSLL + vISSQN (quando retido).</item>
    /// <item><c>vLiq</c> = vServ - vDescCond - vDescIncond - vTotalRet.</item>
    /// </list>
    /// </remarks>
    public static ValoresNFSe CalcularValores(ValoresDps valores)
    {
        var valorServico = valores.ValoresServico.Valor;
        var descontoIncondicionado = valores.ValoresDesconto?.ValorIncodicional ?? 0;
        var descontoCondicionado = valores.ValoresDesconto?.ValorCondicional ?? 0;
        var deducao = ValorDeducao(valores.ValoresDeducaoReducao, valorServico);

        var municipal = valores.Tributos.Municipal;
        var baseCalculo = Arredondar(valorServico - deducao - descontoIncondicionado);
        var aliquota = municipal.Aliquota ?? 0;
        var valorIss = municipal.ISSQN == TributoISSQN.OperacaoTributavel ? Arredondar(baseCalculo * aliquota / 100) : 0;
        var issRetido = municipal.TipoRetencaoISSQN is TipoRetencaoISSQN.RetidoTomador or TipoRetencaoISSQN.RetidoIntermediario;

        var federal = valores.Tributos.Federal;
        var totalRetido = Arredondar((federal?.ValorCP ?? 0) + (federal?.ValorIRRF ?? 0) + (federal?.ValorCSLL ?? 0) +
                                     (issRetido ? valorIss : 0));

        return new ValoresNFSe
        {
            ValorBc = baseCalculo,
            Aliquota = aliquota,
            ValorISSQN = valorIss,
            TotalRetido = totalRetido,
            ValorLiquido = Arredondar(valorServico - descontoCondicionado - descontoIncondicionado - totalRetido)
        };
    }

    private static decimal ValorDeducao(ValoresDeducaoReducao? deducao, decimal valorServico)
    {
        if (deducao == null) return 0;
        if (deducao.Valor.HasValue) return deducao.Valor.Value;
        if (deducao.Porcentagem.HasValue) return Arredondar(valorServico * deducao.Porcentagem.Value / 100);

        return deducao.Documentos?.Sum(x => x.ValorReducaoDeducao) ?? 0;
    }

    private static string SiglaUF(string codMunicipio)
    {
        if (codMunicipio.Length < 2 || !int.TryParse(codMunicipio.Substring(0, 2), out var codUf) ||
            !Enum.IsDefined(typeof(DFeCodUF), codUf))
            throw new InvalidOperationException($"Código de município inválido para o emitente: {codMunicipio}.");

        return ((DFeCodUF)codUf).ToSiglaUF().ToString();
    }

    private static decimal Arredondar(decimal valor) => Math.Round(valor, 2, MidpointRounding.AwayFromZero);

    #endregion Methods
}
