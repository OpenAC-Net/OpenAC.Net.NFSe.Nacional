using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using OpenAC.Net.NFSe.Nacional.Common.Model;
using OpenAC.Net.NFSe.Nacional.Common.Types;
using OpenAC.Net.NFSe.Nacional.DANFSe.PDFSharp;
using OpenAC.Net.NFSe.Nacional.DANFSe.PDFSharp.Common;
using OpenAC.Net.NFSe.Nacional.DANFSe.PDFSharp.Configuracao;

namespace OpenAC.Net.NFSe.Nacional.Test;

public class TestDANFSePDF
{
    [Test]
    public async Task GerarPDF_ComDadosCompletos_GeraPdfValido()
    {
        var nota = CriarNotaExemplo();
        var config = new DANFSeNacionalConfig
        {
            Homologacao = true,
            ExibirCanhoto = true,
            ExibirQRCode = true
        };

        using var ms = new MemoryStream();
        OpenDANFSeNacional.GerarPDF(nota, ms, config);

        var pdfBytes = ms.ToArray();

        await Assert.That(pdfBytes).IsNotNull();
        await Assert.That(pdfBytes.Length).IsGreaterThan(1000);

        // O cabeçalho de um arquivo PDF válido inicia com %PDF
        var header = Encoding.ASCII.GetString(pdfBytes, 0, 4);
        await Assert.That(header).IsEqualTo("%PDF");
    }

    [Test]
    public async Task GerarPDF_ComReformaTributariaIBSCBS_GeraPdfValido()
    {
        var nota = CriarNotaExemplo();
        nota.Informacoes.IBSCBS = new RTCIBSCBS
        {
            CodigoLocalidadeIncidencia = "3550308",
            DescricaoLocalidadeIncidencia = "São Paulo",
            Valores = new RTCValoresIBSCBS
            {
                ValorBaseCalculo = 1500m,
                ValorCalcReeRepRes = 0m,
                Estado = new RTCValoresIBSCBSUf
                {
                    PercentualIBSUf = 0.10m,
                    PercentualAliquotaEfetivaUf = 0.10m
                },
                Municipio = new RTCValoresIBSCBSMunicipio
                {
                    PercentualIBSMunicipal = 0.05m,
                    PercentualAliquotaEfetivaMunicipal = 0.05m
                },
                Federal = new RTCValoresIBSCBSFederal
                {
                    PercentualCBS = 0.90m,
                    PercentualAliquotaEfetivaCBS = 0.90m
                }
            },
            Totais = new RTCTotalCIBS
            {
                TotalIBS = new RTCTotalIBS
                {
                    ValorTotalIBS = 2.25m
                },
                TotalCBS = new RTCTotalCBS
                {
                    ValorCBS = 13.50m
                },
                ValorTotalNF = 1500m
            }
        };

        nota.Informacoes.Dps.Informacoes.IBSCBS = new RTCInfoIBSCBS
        {
            FinalidadeNFSe = RTCFinNFSe.Regular,
            Valores = new RTCInfoValoresIBSCBS
            {
                Tributos = new RTCInfoTributosIBSCBS
                {
                    GrupoIBSCBS = new RTCInfoTributosSitClas
                    {
                        CodigoSituacaoTributaria = "000",
                        CodigoClassificacaoTributaria = "000001"
                    }
                }
            }
        };

        var config = new DANFSeNacionalConfig
        {
            Homologacao = false,
            ExibirCanhoto = true,
            ExibirQRCode = true
        };

        var bytes = OpenDANFSeNacional.GerarPDF(nota, config);
        await Assert.That(bytes).IsNotNull();
        await Assert.That(bytes.Length).IsGreaterThan(1000);

        var header = Encoding.ASCII.GetString(bytes, 0, 4);
        await Assert.That(header).IsEqualTo("%PDF");
    }

    [Test]
    public async Task GerarPDF_MultiplasNotas_GeraPdfComMultiplasPaginas()
    {
        var nota1 = CriarNotaExemplo();
        var nota2 = CriarNotaExemplo();
        nota2.Informacoes.NumeroNFSe = 12346;

        var config = new DANFSeNacionalConfig();
        var danfse = new OpenDANFSeNacional(config);
        var bytes = danfse.GerarPDF([nota1, nota2]);

        await Assert.That(bytes).IsNotNull();
        await Assert.That(bytes.Length).IsGreaterThan(1000);

        var header = Encoding.ASCII.GetString(bytes, 0, 4);
        await Assert.That(header).IsEqualTo("%PDF");
    }

    [Test]
    public async Task GerarPDF_ComSenhaUsuario_CriptografaPdfComSucesso()
    {
        var nota = CriarNotaExemplo();
        var config = new DANFSeNacionalConfig
        {
            Seguranca = new DANFSeSegurancaConfig
            {
                SenhaUsuario = "123456",
                SenhaProprietario = "admin987",
                PermitirImpressao = true,
                PermitirModificacao = false,
                PermitirCopiarConteudo = false
            }
        };

        var bytes = OpenDANFSeNacional.GerarPDF(nota, config);

        await Assert.That(bytes).IsNotNull();
        await Assert.That(bytes.Length).IsGreaterThan(1000);

        // Validar abertura com a senha correta
        using var ms = new MemoryStream(bytes);
        using var docProtegido = PdfSharp.Pdf.IO.PdfReader.Open(ms, "123456", PdfSharp.Pdf.IO.PdfDocumentOpenMode.Import);

        await Assert.That(docProtegido).IsNotNull();
        await Assert.That(docProtegido.PageCount).IsEqualTo(1);
        await Assert.That(docProtegido.SecuritySettings.PermitModifyDocument).IsFalse();
    }

    [Test]
    public async Task FontResolver_CarregaFontesEmbutidasCorretamente()
    {
        var resolver = DANFSeFontResolver.Instance;
        
        var regular = resolver.GetFont("LiberationSans-Regular");
        var bold = resolver.GetFont("LiberationSans-Bold");
        var italic = resolver.GetFont("LiberationSans-Italic");
        var boldItalic = resolver.GetFont("LiberationSans-BoldItalic");

        await Assert.That(regular).IsNotNull();
        await Assert.That(bold).IsNotNull();
        await Assert.That(italic).IsNotNull();
        await Assert.That(boldItalic).IsNotNull();

        await Assert.That(regular!.Length).IsGreaterThan(100000);
        await Assert.That(bold!.Length).IsGreaterThan(100000);
    }

    [Test]
    public async Task GerarArquivosExemplosPDF()
    {
        var outputDir = Path.Combine(AppContext.BaseDirectory, "../../../../../Exemplos_PDF");
        if (!Directory.Exists(outputDir))
            Directory.CreateDirectory(outputDir);

        // 1. Exemplo Padrão (Produção com Canhoto e QR Code)
        var notaPadrao = CriarNotaExemplo();
        var configPadrao = new DANFSeNacionalConfig
        {
            Homologacao = false,
            ExibirCanhoto = true,
            ExibirQRCode = true
        };
        var pathPadrao = Path.Combine(outputDir, "Exemplo_DANFSe_Nacional_Padrao.pdf");
        OpenDANFSeNacional.GerarPDF(notaPadrao, pathPadrao, configPadrao);

        // 2. Exemplo com Reforma Tributária (IBS / CBS - RTC)
        var notaRTC = CriarNotaExemplo();
        notaRTC.Informacoes.IBSCBS = new RTCIBSCBS
        {
            CodigoLocalidadeIncidencia = "3550308",
            DescricaoLocalidadeIncidencia = "São Paulo",
            Valores = new RTCValoresIBSCBS
            {
                ValorBaseCalculo = 1500m,
                ValorCalcReeRepRes = 0m,
                Estado = new RTCValoresIBSCBSUf
                {
                    PercentualIBSUf = 0.10m,
                    PercentualAliquotaEfetivaUf = 0.10m
                },
                Municipio = new RTCValoresIBSCBSMunicipio
                {
                    PercentualIBSMunicipal = 0.05m,
                    PercentualAliquotaEfetivaMunicipal = 0.05m
                },
                Federal = new RTCValoresIBSCBSFederal
                {
                    PercentualCBS = 0.90m,
                    PercentualAliquotaEfetivaCBS = 0.90m
                }
            },
            Totais = new RTCTotalCIBS
            {
                TotalIBS = new RTCTotalIBS { ValorTotalIBS = 2.25m },
                TotalCBS = new RTCTotalCBS { ValorCBS = 13.50m },
                ValorTotalNF = 1500m
            }
        };
        notaRTC.Informacoes.Dps.Informacoes.IBSCBS = new RTCInfoIBSCBS
        {
            FinalidadeNFSe = RTCFinNFSe.Regular,
            Valores = new RTCInfoValoresIBSCBS
            {
                Tributos = new RTCInfoTributosIBSCBS
                {
                    GrupoIBSCBS = new RTCInfoTributosSitClas
                    {
                        CodigoSituacaoTributaria = "000",
                        CodigoClassificacaoTributaria = "000001"
                    }
                }
            }
        };
        var pathRTC = Path.Combine(outputDir, "Exemplo_DANFSe_ReformaTributaria_IBS_CBS.pdf");
        OpenDANFSeNacional.GerarPDF(notaRTC, pathRTC, configPadrao);

        // 3. Exemplo Homologação
        var notaHomologacao = CriarNotaExemplo();
        var configHomologacao = new DANFSeNacionalConfig
        {
            Homologacao = true,
            ExibirCanhoto = true,
            ExibirQRCode = true
        };
        var pathHomologacao = Path.Combine(outputDir, "Exemplo_DANFSe_Homologacao.pdf");
        OpenDANFSeNacional.GerarPDF(notaHomologacao, pathHomologacao, configHomologacao);

        // 4. Exemplo Cancelada
        var notaCancelada = CriarNotaExemplo();
        var configCancelada = new DANFSeNacionalConfig
        {
            Homologacao = false,
            Cancelada = true,
            ExibirCanhoto = false,
            ExibirQRCode = true
        };
        var pathCancelada = Path.Combine(outputDir, "Exemplo_DANFSe_Cancelada.pdf");
        OpenDANFSeNacional.GerarPDF(notaCancelada, pathCancelada, configCancelada);

        // 5. Exemplo Protegido com Senha
        var notaProtegida = CriarNotaExemplo();
        var configProtegida = new DANFSeNacionalConfig
        {
            Homologacao = false,
            ExibirCanhoto = true,
            ExibirQRCode = true,
            Seguranca = new DANFSeSegurancaConfig
            {
                SenhaUsuario = "123456",
                SenhaProprietario = "masterAdmin@2026",
                PermitirImpressao = true,
                PermitirModificacao = false,
                PermitirCopiarConteudo = false
            }
        };
        var pathProtegida = Path.Combine(outputDir, "Exemplo_DANFSe_ProtegidaComSenha.pdf");
        OpenDANFSeNacional.GerarPDF(notaProtegida, pathProtegida, configProtegida);

        await Assert.That(File.Exists(pathPadrao)).IsTrue();
        await Assert.That(File.Exists(pathRTC)).IsTrue();
        await Assert.That(File.Exists(pathHomologacao)).IsTrue();
        await Assert.That(File.Exists(pathCancelada)).IsTrue();
        await Assert.That(File.Exists(pathProtegida)).IsTrue();
    }

    private static NotaFiscalServico CriarNotaExemplo()
    {
        var nota = new NotaFiscalServico
        {
            Informacoes = new InfNFSe
            {
                Id = "NFS35503082200000000000000000000000000000000000000001",
                NumeroNFSe = 12345,
                DhProcessamento = DateTimeOffset.Now,
                LocalEmissao = "São Paulo",
                LocalPrestacao = "São Paulo",
                LocalIncidencia = "São Paulo",
                AmbienteGerador = AmbienteGerador.Nacional,
                SituacaoNFSe = StatusNFSe.Gerada,
                DescricaoTributoNacional = "Análise e desenvolvimento de sistemas",
                Emitente = new EmitenteNFSe
                {
                    CNPJ = "12345678000195",
                    RazaoSocial = "EMPRESA DE SERVICOS LTDA",
                    InscricaoMunicipal = "123456",
                    Telefone = "11999998888",
                    Email = "contato@empresa.com.br",
                    Endereco = new EnderecoEmitente
                    {
                        Logradouro = "Avenida Paulista",
                        Numero = "1000",
                        Complemento = "Conjunto 101",
                        Bairro = "Bela Vista",
                        CodMunicipio = "3550308",
                        UF = "SP",
                        CEP = "01310100"
                    }
                },
                Dps = new Dps
                {
                    Informacoes = new InfDps
                    {
                        NumeroDps = "100",
                        Serie = "1",
                        DhEmissao = DateTimeOffset.Now,
                        Competencia = DateTime.Now,
                        Prestador = new PrestadorDps
                        {
                            Regime = new RegimeTributario
                            {
                                OptanteSimplesNacional = OptanteSimplesNacional.OptanteMEEPP
                            }
                        },
                        Tomador = new InfoPessoaNFSe
                        {
                            Nome = "TOMADOR DE EXEMPLO LTDA",
                            CNPJ = "98765432000109",
                            InscricaoMunicipal = "654321",
                            Telefone = "1133334444",
                            Email = "financeiro@tomador.com.br",
                            Endereco = new EnderecoNFSe
                            {
                                Logradouro = "Rua Augusta",
                                Numero = "500",
                                Bairro = "Consolação",
                                Municipio = new MunicipioNacional
                                {
                                    CodMunicipio = "3550308",
                                    CEP = "01305000"
                                }
                            }
                        },
                        Servico = new ServicoNFSe
                        {
                            Informacoes = new InformacoesServico
                            {
                                CodTributacaoNacional = "010101",
                                CodTributacaoMunicipio = "101",
                                CodNBS = "1.0101.10.00",
                                Descricao = "Serviços prestados de desenvolvimento e manutenção de software customizado."
                            },
                            InformacoesComplementares = new InformacoesComplementares
                            {
                                Informacoes = "Documento emitido por ME ou EPP optante pelo Simples Nacional."
                            }
                        },
                        Valores = new ValoresDps
                        {
                            ValoresServico = new ValoresServico
                            {
                                Valor = 1500.00m
                            },
                            ValoresDesconto = new ValoresDesconto
                            {
                                ValorIncodicional = 0m,
                                ValorCondicional = 0m
                            },
                            Tributos = new TributosNFSe
                            {
                                Municipal = new TributoMunicipal
                                {
                                    ISSQN = TributoISSQN.OperacaoTributavel,
                                    Aliquota = 5.00m
                                },
                                Federal = new TributoFederal
                                {
                                    ValorIRRF = 22.50m,
                                    ValorCP = 0m,
                                    ValorCSLL = 15.00m,
                                    PisCofins = new PisCofins
                                    {
                                        ValorPis = 9.75m,
                                        ValorCofins = 45.00m
                                    }
                                }
                            }
                        }
                    }
                },
                Valores = new ValoresNFSe
                {
                    ValorBc = 1500.00m,
                    Aliquota = 5.00m,
                    ValorISSQN = 75.00m,
                    TotalRetido = 92.25m,
                    ValorLiquido = 1407.75m
                }
            }
        };

        return nota;
    }
}
