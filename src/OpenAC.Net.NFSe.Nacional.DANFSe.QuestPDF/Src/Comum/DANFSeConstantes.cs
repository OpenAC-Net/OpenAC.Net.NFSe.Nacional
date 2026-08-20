namespace OpenAC.Net.NFSe.Nacional.DANFSe.QuestPDF.Src.Comum;

/// <summary>
/// Constantes utilizadas na montagem visual do DANFSe Padrão Nacional.
/// </summary>
internal static class DANFSeConstantes
{
    /// <summary>URL base para consulta pública da NFS-e Nacional.</summary>
    public const string UrlConsultaPublica = "https://www.nfse.gov.br/ConsultaPublica/?tpc=1&chave=";

    /// <summary>
    /// Constantes de dimensão e espaçamento das sessões do documento.
    /// </summary>
    public static class Sessao
    {
        public const float TamanhoLogo = 30;

        public const float TamanhoEspacoLogo = 1.2f;

        public const float TamanhoEspacoConteudo = 4;

        public const float TamanhoEspacoConteudoPequeno = 2;

        public const float TamanhoEspacoConteudoMedio = 3;

        public const float TamanhoEspacoTituloCabecalho = 2.5f;

        public const float TamanhoEspacoDescricaoCabecalho = 1.8f;

        public const float TamanhoQrCode = 45;

        public const float TamanhoBorda = 0.7f;

        public const float TamanhoBordaCabecalho = 0.8f;

        public const float EspacoAlturaDescricaoQrCode = 4;

        public const string CorFundoCabecalho = "#F2F2F2";
    }

    /// <summary>
    /// Constantes relacionadas às fontes utilizadas no documento.
    /// </summary>
    public static class Fonte
    {
        public const float TamanhoTitulo = 8;

        public const float TamanhoTituloPequeno = 6;

        public const float TamanhoTituloMedio = 9;

        public const float TamanhoTituloGrande = 18;

        public const float TamanhoTituloGigante = 38;

        /// <summary>
        /// Constantes de tamanho de fonte dos campos do documento.
        /// </summary>
        public static class Campo
        {
            public const float TamanhoTitulo = 7;

            public const float TamanhoValor = 7;
        }

        /// <summary>
        /// Constantes de tamanho de fonte do QR Code.
        /// </summary>
        public static class QrCode
        {
            public const float TamanhoValor = 6;
        }

        /// <summary>
        /// Cores utilizadas no documento.
        /// </summary>
        public static class Cores
        {
            public const string FundoTitulo = "#f0f0f0";
            public const string TextoHomologacao = "#ff222d";

            public const string TextoSemValidadeJuridica = "#CCffb2b4";

            public const string TextoCancelada = TextoSemValidadeJuridica;

            public const string TextoSubstituida = TextoSemValidadeJuridica;
        }

    }

    /// <summary>
    /// Textos e rótulos exibidos no DANFSe.
    /// </summary>
    public static class Mensagens
    {
        public const string CanhotoRecebimento = "CANHOTO DE RECEBIMENTO";

        public const string DataCientificacao = "Data da Cientificação";

        public const string IdentificacaoAssinaturaRecebedor = "Identificação e Assinatura do Recebedor";

        public const string NumeroNfseChave = "Nº NFS-e / Chave";

        public const string Nfse = "NFS-e";

        public const string NfseVersao = "DANFSe v2.0";

        public const string DocumentoAuxiliar = "Documento Auxiliar da NFS-e";

        public const string Municipio = "Município: {0}";

        public const string AmbienteGerador = "Ambiente gerador: {0}";

        public const string TipoEmissao = "Tipo de emissão: {0}";

        public const string NfseSemValidadeJuridica = "NFS-e SEM VALIDADE JURÍDICA";

        public const string NfseCancelada = "CANCELADA";

        public const string NfseSubstituida = "SUBSTITUÍDA";

        public const string ChaveAcesso = "CHAVE DE ACESSO DA NFS-e";

        public const string NumeroNfse = "NÚMERO DA NFS-e";

        public const string CompetenciaNfse = "COMPETÊNCIA DA NFS-e";

        public const string DataHoraEmissao = "DATA E HORA DA EMISSÃO";

        public const string NumeroDps = "NÚMERO DA DPS";

        public const string SerieDps = "SÉRIE DA DPS";

        public const string DataHoraEmissaoDps = "DATA E HORA DA EMISSÃO DA DPS";

        public const string EmitenteNfse = "EMITENTE DA NFS-e";

        public const string SituacaoNfse = "SITUAÇÃO DA NFS-e";

        public const string Finalidade = "FINALIDADE";

        public const string QrCodeNaoDisponivel = "QR Code\nnão disponível";

        public const string QrCodeDescricao = "A autenticidade desta NFS-e pode ser verificada pela leitura deste código QR ou pela consulta da chave de acesso no portal nacional da NFS-e";

        public const string PrestadorFornecedor = "Prestador / Fornecedor";

        public const string CnpjCpfNif = "CNPJ / CPF / NIF";

        public const string InscricaoMunicipal = "Indicador Municipal (Inscrição)";

        public const string Telefone = "Telefone";

        public const string NomeOuNomeEmpresarial = "Nome / Nome Empresarial";

        public const string MunicipioSiglaUf = "Município / Sigla UF";

        public const string CodigoIbgeCep = "Código IBGE / CEP";

        public const string Endereco = "Endereço";

        public const string Email = "E-mail";

        public const string SimplesNacionalDataCompetencia = "Simples Nacional na Data de Competência";

        public const string RegimeApuracaoTributaria = "Regime de Apuração Tributária pelo SN";

        public const string TomadorAdiquirente = "Tomador / Adquirente";

        public const string TomadorAdiquirenteOperacaoNaoIdentificado = "TOMADOR / ADQUIRENTE DA OPERAÇÃO NÃO IDENTIFICADO NA NFS-e";

        public const string DestinatarioOperacao = "Destinatário da Operação";

        public const string DestinatarioOperacaoNaoIdentificado = "DESTINATÁRIO DA OPERAÇÃO NÃO IDENTIFICADO NA NFS-e";

        public const string IntermediarioOperacao = "Intermediário da Operação";

        public const string IntermediarioOperacaoNaoIdentificado = "INTERMEDIÁRIO DA OPERAÇÃO NÃO IDENTIFICADO NA NFS-e";

        public const string ServicoPrestado = "Serviço Prestado";

        public const string CodigoTributacaoNacionalMunicipal = "Código de Trib. Nacional / Municipal";

        public const string CodigoNbs = "Código da NBS";

        public const string LocalPrestacaoSiglaUfPais = "Local da Prestação / Sigla UF / País";

        public const string DescricaoServico = "Descrição do Serviço";

        public const string TributacaoMunicipalIssqn = "Tributação Municipal (ISSQN)";

        public const string TipoTributacaoIssqn = "Tipo de Tributação do ISSQN";

        public const string MunicipioSiglaUfPaisIncidenciaIssqn = "Município / Sigla UF / País de Incidência do ISSQN";

        public const string RegimeEspecialTributosIssqn = "Regime Especial de Tributos do ISSQN";

        public const string TipoImunidadeIssqn = "Tipo de Imunidade do ISSQN";

        public const string SuspensaoExigibiliddeIssqn = "Suspensão da Exigibilidade do ISSQN";

        public const string NumeroProcessoSuspensao = "Número Processo Suspensão";

        public const string BeneficioMunicipal = "Beneficio Municipal";

        public const string CalculoBeneficioMunicipal = "Cálculo do BM";

        public const string TotalDeducoesReducoes = "Total de Deduções/Reduções";

        public const string DescontoIncondicionado = "Desconto Incondicionado";

        public const string BaseCalculoIssqn = "BC ISSQN";

        public const string AliquotaAplicada = "Alíquota Aplicada";

        public const string RetencaoIssqn = "Retenção do ISSON";

        public const string IssqnApurado = "ISSQN Apurado";

        public const string TributacaoFederalExcetoCbs = "Tributação Federal (Exceto CBS)";

        public const string Irrf = "IRRF";

        public const string ContribuicaoPrevidenciariaRetida = "Contribuição Previdenciária - Retida";

        public const string ContribuicoesSocialRetida = "Contribuições Sociais - Retidas";

        public const string PisDebitoApuracaoPropria = "PIS - Débito Apuração Própria";

        public const string CofinsDebitoApuracaoPropria = "COFINS - Débito Apuração Própria";

        public const string DescricaoContribuicaoSocialRetida = "Descrição Contrib. Sociais - Retidas";

        public const string TributacaoIbsCbs = "Tributação IBS / CBS";

        public const string CstCclassTrib = "CST / cClassTrib";

        public const string IndicadorOperacaoCodigoIbgeIncidenciaMunicipioIncidenciaSiglaUf = "Indicador de Operação / Código IBGE Incidência / Município Incidência / Sigla UF";

        public const string ExclusaoReducaoBaseCalculo = "Exclusões e Reduções da Base de Cálculo";

        public const string BaseCalculoAposExclusaoReducao = "Base de Cálculo após Exclusões e Reduções";

        public const string ReducaoAliquotaIbsCbs = "Redu. Alíquota IBS / CBS";

        public const string AliquotaIbsUfMunicipio = "Alíquota - IBS UF / Mun";

        public const string AliquotaEfetivaMunicipalIbs = "Aliq. Efetiva Municipal - IBS";

        public const string ValorApuradoMunicipalIbs = "Valor Apurado Municipal - IBS";

        public const string AliquotaEfetivaEstadualIbs = "Aliq. Efetiva Estadual - IBS";

        public const string ValorApuradoEstadualIbs = "Valor Apurado Estadual - IBS";

        public const string ValorTotalApuradoIbs = "Valor Total Apurado - IBS";

        public const string AliquotaCbs = "Alíquota - CBS";

        public const string AliquotaEfetivaCbs = "Alíquota Efetiva - CBS";

        public const string ValorTotalApuradoCbs = "Valor Total Apurado - CBS";

        public const string ValorTotalNfse = "Valor Total da NFS-e";

        public const string ValorOperacaoServico = "Valor da Operação / Serviço";

        public const string DescontoCondicionado = "Desconto Condicionado";

        public const string TotalRetencaoFederal = "Total das Retenções Federais";

        public const string ValorLiquidoNfse = "Valor Líquido da NFS-e";

        public const string TotalIbsCbs = "Total IBS / CBS";

        public const string ValorLiquidoSomaIbsCbs = "Valor Líquido da NFSe + IBS/CBS";

        public const string InformacoesComplementares = "Informações complementares";

        public const string InformacoesComplementaresTotais = "Totais aproximado dos Tributos cfe Lei nº 12.741/2012: Federais: {0}, Estaduais: {1}, Municipais: {2}";

        public const string MetaDadoTitulo = "NFS-e Padrão Nacional";
    }
}