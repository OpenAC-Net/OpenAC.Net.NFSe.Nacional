using System.Xml.Linq;
using OpenAC.Net.DFe.Core.Common;
using OpenAC.Net.NFSe.Nacional.Common.Model;
using OpenAC.Net.NFSe.Nacional.Common.Types;
using OpenAC.Net.NFSe.Nacional.Webservice;

namespace OpenAC.Net.NFSe.Nacional.Test;

/// <summary>
/// Testes unitários para validar a geração (serialização) e leitura (desserialização) de XML
/// com as modificações geradas pelo DFe Source Generator (OpenAC.Net.DFe.Generator).
/// </summary>
public class TestXmlSerialization
{
    #region DPS Tests

    [Test]
    public async Task Dps_SerializacaoEDesserializacao_Completa()
    {
        var dps = new Dps
        {
            Versao = VersaoNFSe.Ve101,
            Informacoes = new InfDps
            {
                TipoAmbiente = DFeTipoAmbiente.Homologacao,
                DhEmissao = new DateTime(2026, 8, 16, 14, 30, 0),
                VersaoAplicacao = "OpenAC 1.0",
                Serie = "1",
                NumeroDps = "12345",
                Competencia = new DateTime(2026, 8, 1),
                TipoEmitente = EmitenteDps.Prestador,
                LocalidadeEmitente = "3550308",
                Prestador = new PrestadorDps
                {
                    CNPJ = "12ABC678000199",
                    InscricaoMunicipal = "123456",
                    Nome = "Prestador Teste Ltda",
                    Email = "prestador@teste.com",
                    Telefone = "11999999999",
                    Regime = new RegimeTributario
                    {
                        OptanteSimplesNacional = OptanteSimplesNacional.NaoOptante,
                        RegimeEspecial = RegimeEspecial.Nenhum
                    },
                    Endereco = new EnderecoNFSe
                    {
                        Logradouro = "Rua das Flores",
                        Numero = "100",
                        Complemento = "Sala 1",
                        Bairro = "Centro",
                        Municipio = new MunicipioNacional
                        {
                            CodMunicipio = "3550308",
                            CEP = "01001000"
                        }
                    }
                },
                Tomador = new InfoPessoaNFSe
                {
                    CNPJ = "98XYZ432000188",
                    Nome = "Tomador Teste SA",
                    Email = "tomador@teste.com",
                    Endereco = new EnderecoNFSe
                    {
                        Logradouro = "Avenida Paulista",
                        Numero = "1000",
                        Bairro = "Bela Vista",
                        Municipio = new MunicipioNacional
                        {
                            CodMunicipio = "3550308",
                            CEP = "01310100"
                        }
                    }
                },
                Intermediario = new InfoPessoaNFSe
                {
                    CNPJ = "11222333000144",
                    Nome = "Intermediario Servicos Ltda"
                },
                Servico = new ServicoNFSe
                {
                    Localidade = new LocalidadeNFSe
                    {
                        CodMunicipioPrestacao = "3550308"
                    },
                    Informacoes = new InformacoesServico
                    {
                        CodTributacaoNacional = "010101",
                        CodTributacaoMunicipio = "002",
                        Descricao = "Serviço de consultoria e desenvolvimento de software"
                    }
                },
                Valores = new ValoresDps
                {
                    ValoresServico = new ValoresServico
                    {
                        Valor = 2500.50m
                    },
                    ValoresDesconto = new ValoresDesconto
                    {
                        ValorCondicional = 50.00m,
                        ValorIncodicional = 100.00m
                    },
                    Tributos = new TributosNFSe
                    {
                        Municipal = new TributoMunicipal
                        {
                            ISSQN = TributoISSQN.OperacaoTributavel,
                            TipoRetencaoISSQN = TipoRetencaoISSQN.NaoRetido,
                            Aliquota = 5.00m
                        },
                        Federal = new TributoFederal
                        {
                            PisCofins = new PisCofins
                            {
                                Cst = TipoCST.AliquotaBasica,
                                AliquotaPis = 0.65m,
                                ValorPis = 15.60m,
                                AliquotaCofins = 3.00m,
                                ValorCofins = 72.00m
                            },
                            ValorCSLL = 25.00m,
                            ValorCP = 50.00m,
                            ValorIRRF = 37.50m
                        },
                        Total = new TotalTributos
                        {
                            PorcentagemTotal = new PorcentagemTotalTributos
                            {
                                TotalFederal = 5.15m,
                                TotalEstadual = 0m,
                                TotalMunicipal = 5.00m
                            }
                        }
                    }
                }
            }
        };

        dps.GerarId();

        // 1. Serializar
        var xml = dps.GetXml();
        await Assert.That(xml).IsNotEmpty();
        await Assert.That(xml).Contains("<DPS");
        await Assert.That(xml).Contains("versao=\"1.01\"");
        await Assert.That(xml).Contains("<infDPS");
        await Assert.That(xml).Contains("<tpAmb>2</tpAmb>");
        await Assert.That(xml).Contains("<serie>1</serie>");
        await Assert.That(xml).Contains("<nDPS>12345</nDPS>");
        await Assert.That(xml).Contains("<CNPJ>12ABC678000199</CNPJ>");
        await Assert.That(xml).Contains("<CNPJ>98XYZ432000188</CNPJ>");
        await Assert.That(xml).Contains("<vServ>2500.50</vServ>");
        await Assert.That(xml).Contains("<cTribNac>010101</cTribNac>");

        // 2. Desserializar via XElement / ReadFromXml
        var element = XElement.Parse(xml);
        var dpsLida = Dps.ReadFromXml(element);

        await Assert.That(dpsLida).IsNotNull();
        await Assert.That(dpsLida!.Versao).IsEqualTo(VersaoNFSe.Ve101);
        await Assert.That(dpsLida.Informacoes).IsNotNull();
        await Assert.That(dpsLida.Informacoes.Id).IsEqualTo(dps.Informacoes.Id);
        await Assert.That(dpsLida.Informacoes.TipoAmbiente).IsEqualTo(DFeTipoAmbiente.Homologacao);
        await Assert.That(dpsLida.Informacoes.Serie).IsEqualTo("1");
        await Assert.That(dpsLida.Informacoes.NumeroDps).IsEqualTo("12345");
        await Assert.That(dpsLida.Informacoes.Prestador.CNPJ).IsEqualTo("12ABC678000199");
        await Assert.That(dpsLida.Informacoes.Prestador.Nome).IsEqualTo("Prestador Teste Ltda");
        await Assert.That(dpsLida.Informacoes.Tomador!.CNPJ).IsEqualTo("98XYZ432000188");
        await Assert.That(dpsLida.Informacoes.Tomador.Nome).IsEqualTo("Tomador Teste SA");
        await Assert.That(dpsLida.Informacoes.Intermediario!.CNPJ).IsEqualTo("11222333000144");
        await Assert.That(dpsLida.Informacoes.Servico.Informacoes.CodTributacaoNacional).IsEqualTo("010101");
        await Assert.That(dpsLida.Informacoes.Valores.ValoresServico.Valor).IsEqualTo(2500.50m);
        await Assert.That(dpsLida.Informacoes.Valores.ValoresDesconto!.ValorCondicional).IsEqualTo(50.00m);
        await Assert.That(dpsLida.Informacoes.Valores.ValoresDesconto.ValorIncodicional).IsEqualTo(100.00m);
        await Assert.That(dpsLida.Informacoes.Valores.Tributos!.Municipal!.Aliquota).IsEqualTo(5.00m);
        await Assert.That(dpsLida.Informacoes.Valores.Tributos.Federal!.PisCofins!.ValorPis).IsEqualTo(15.60m);
    }

    [Test]
    public async Task Dps_SerializacaoEDesserializacao_ReformaTributaria_IBSCBS()
    {
        var dps = new Dps
        {
            Versao = VersaoNFSe.Ve101,
            Informacoes = new InfDps
            {
                TipoAmbiente = DFeTipoAmbiente.Homologacao,
                DhEmissao = DateTime.Now,
                LocalidadeEmitente = "3550308",
                Serie = "1",
                NumeroDps = "777",
                Competencia = DateTime.Today,
                TipoEmitente = EmitenteDps.Prestador,
                Prestador = new PrestadorDps
                {
                    CNPJ = "12ABC678000199",
                    Regime = new RegimeTributario
                    {
                        OptanteSimplesNacional = OptanteSimplesNacional.NaoOptante,
                        RegimeEspecial = RegimeEspecial.Nenhum
                    }
                },
                Tomador = new InfoPessoaNFSe
                {
                    CNPJ = "98XYZ432000188",
                    Nome = "Tomador IBS CBS Ltda"
                },
                Servico = new ServicoNFSe
                {
                    Localidade = new LocalidadeNFSe { CodMunicipioPrestacao = "3550308" },
                    Informacoes = new InformacoesServico
                    {
                        CodTributacaoNacional = "010101",
                        Descricao = "Servico com IBS e CBS"
                    }
                },
                Valores = new ValoresDps
                {
                    ValoresServico = new ValoresServico { Valor = 1000m }
                },
                IBSCBS = new RTCInfoIBSCBS
                {
                    FinalidadeNFSe = RTCFinNFSe.Regular,
                    IndicadorDestinatario = RTCIndDest.ProprioTomador,
                    CodigoIndicadorOperacao = "100001",
                    Valores = new RTCInfoValoresIBSCBS
                    {
                        Tributos = new RTCInfoTributosIBSCBS
                        {
                            GrupoIBSCBS = new RTCInfoTributosSitClas
                            {
                                CodigoSituacaoTributaria = "001",
                                CodigoClassificacaoTributaria = "000001"
                            }
                        }
                    }
                }
            }
        };

        dps.GerarId();

        var xml = dps.GetXml();
        await Assert.That(xml).Contains("<IBSCBS");
        await Assert.That(xml).Contains("<finNFSe>0</finNFSe>");
        await Assert.That(xml).Contains("<cIndOp>100001</cIndOp>");
        await Assert.That(xml).Contains("<CST>001</CST>");
        await Assert.That(xml).Contains("<cClassTrib>000001</cClassTrib>");

        var element = XElement.Parse(xml);
        var dpsLida = Dps.ReadFromXml(element);

        await Assert.That(dpsLida).IsNotNull();
        await Assert.That(dpsLida!.Informacoes.IBSCBS).IsNotNull();
        await Assert.That(dpsLida.Informacoes.IBSCBS!.FinalidadeNFSe).IsEqualTo(RTCFinNFSe.Regular);
        await Assert.That(dpsLida.Informacoes.IBSCBS.CodigoIndicadorOperacao).IsEqualTo("100001");
        await Assert.That(dpsLida.Informacoes.IBSCBS.Valores.Tributos.GrupoIBSCBS.CodigoSituacaoTributaria).IsEqualTo("001");
        await Assert.That(dpsLida.Informacoes.IBSCBS.Valores.Tributos.GrupoIBSCBS.CodigoClassificacaoTributaria).IsEqualTo("000001");
    }

    [Test]
    public async Task Dps_SerializacaoEDesserializacao_ComDeducaoReducao()
    {
        var dps = new Dps
        {
            Versao = VersaoNFSe.Ve101,
            Informacoes = new InfDps
            {
                TipoAmbiente = DFeTipoAmbiente.Homologacao,
                DhEmissao = DateTime.Now,
                LocalidadeEmitente = "3550308",
                Serie = "1",
                NumeroDps = "888",
                Competencia = DateTime.Today,
                TipoEmitente = EmitenteDps.Prestador,
                Prestador = new PrestadorDps { CNPJ = "12ABC678000199" },
                Tomador = new InfoPessoaNFSe { CNPJ = "98XYZ432000188" },
                Servico = new ServicoNFSe
                {
                    Localidade = new LocalidadeNFSe { CodMunicipioPrestacao = "3550308" },
                    Informacoes = new InformacoesServico { CodTributacaoNacional = "010101", Descricao = "Servico" }
                },
                Valores = new ValoresDps
                {
                    ValoresServico = new ValoresServico { Valor = 5000m },
                    ValoresDeducaoReducao = new ValoresDeducaoReducao
                    {
                        Documentos =
                        {
                            new DocumentoDeducaoReducao
                            {
                                TipoDeducaoReducao = TipoDeducaoReducao.Materiais,
                                DataEmissao = new DateTime(2026, 8, 10),
                                ValorDedutivelRedutivel = 1000m,
                                ValorReducaoDeducao = 500m,
                                ChaveNFe = "35260812345678000199550010000001231000000010"
                            },
                            new DocumentoDeducaoReducao
                            {
                                TipoDeducaoReducao = TipoDeducaoReducao.SubempreitadaMaoObra,
                                DataEmissao = new DateTime(2026, 8, 12),
                                ValorDedutivelRedutivel = 800m,
                                ValorReducaoDeducao = 400m,
                                ChaveNFSe = "3550308212ABC67800019900001000000000012345"
                            }
                        }
                    }
                }
            }
        };

        dps.GerarId();

        var xml = dps.GetXml();
        await Assert.That(xml).Contains("<vDedRed>");
        await Assert.That(xml).Contains("<docDedRed>");
        await Assert.That(xml).Contains("<chNFe>35260812345678000199550010000001231000000010</chNFe>");
        await Assert.That(xml).Contains("<chNFSe>3550308212ABC67800019900001000000000012345</chNFSe>");

        var element = XElement.Parse(xml);
        var dpsLida = Dps.ReadFromXml(element);

        await Assert.That(dpsLida).IsNotNull();
        var docs = dpsLida!.Informacoes.Valores.ValoresDeducaoReducao!.Documentos;
        await Assert.That(docs.Count).IsEqualTo(2);
        await Assert.That(docs[0].TipoDeducaoReducao).IsEqualTo(TipoDeducaoReducao.Materiais);
        await Assert.That(docs[0].ChaveNFe).IsEqualTo("35260812345678000199550010000001231000000010");
        await Assert.That(docs[0].ValorReducaoDeducao).IsEqualTo(500m);
        await Assert.That(docs[1].TipoDeducaoReducao).IsEqualTo(TipoDeducaoReducao.SubempreitadaMaoObra);
        await Assert.That(docs[1].ChaveNFSe).IsEqualTo("3550308212ABC67800019900001000000000012345");
    }

    [Test]
    public async Task Dps_SerializacaoEDesserializacao_ComBeneficioMunicipal()
    {
        var dps = new Dps
        {
            Versao = VersaoNFSe.Ve101,
            Informacoes = new InfDps
            {
                TipoAmbiente = DFeTipoAmbiente.Homologacao,
                DhEmissao = DateTime.Now,
                LocalidadeEmitente = "3550308",
                Serie = "1",
                NumeroDps = "999",
                Competencia = DateTime.Today,
                TipoEmitente = EmitenteDps.Prestador,
                Prestador = new PrestadorDps { CNPJ = "12ABC678000199" },
                Tomador = new InfoPessoaNFSe { CNPJ = "98XYZ432000188" },
                Servico = new ServicoNFSe
                {
                    Localidade = new LocalidadeNFSe { CodMunicipioPrestacao = "3550308" },
                    Informacoes = new InformacoesServico { CodTributacaoNacional = "010101", Descricao = "Servico com BM" }
                },
                Valores = new ValoresDps
                {
                    ValoresServico = new ValoresServico { Valor = 3000m },
                    Tributos = new TributosNFSe
                    {
                        Municipal = new TributoMunicipal
                        {
                            ISSQN = TributoISSQN.OperacaoTributavel,
                            TipoRetencaoISSQN = TipoRetencaoISSQN.NaoRetido,
                            Beneficio = new BeneficioMunicipal
                            {
                                NumeroBeneficio = "123456",
                                ValorReducao = 500m,
                                PorcentagemReducao = 16.67m
                            }
                        }
                    }
                }
            }
        };

        dps.GerarId();

        var xml = dps.GetXml();
        await Assert.That(xml).Contains("<BM>");
        await Assert.That(xml).Contains("<nBM>123456</nBM>");
        await Assert.That(xml).Contains("<vRedBCBM>500.00</vRedBCBM>");
        await Assert.That(xml).Contains("<pRedBCBM>16.67</pRedBCBM>");

        var element = XElement.Parse(xml);
        var dpsLida = Dps.ReadFromXml(element);

        await Assert.That(dpsLida).IsNotNull();
        var bm = dpsLida!.Informacoes.Valores.Tributos.Municipal!.Beneficio;
        await Assert.That(bm).IsNotNull();
        await Assert.That(bm!.NumeroBeneficio).IsEqualTo("123456");
        await Assert.That(bm.ValorReducao).IsEqualTo(500m);
        await Assert.That(bm.PorcentagemReducao).IsEqualTo(16.67m);
    }

    [Test]
    public async Task Dps_SerializacaoEDesserializacao_ComTomadorExterior()
    {
        var dps = new Dps
        {
            Versao = VersaoNFSe.Ve101,
            Informacoes = new InfDps
            {
                TipoAmbiente = DFeTipoAmbiente.Homologacao,
                DhEmissao = DateTime.Now,
                LocalidadeEmitente = "3550308",
                Serie = "1",
                NumeroDps = "1001",
                Competencia = DateTime.Today,
                TipoEmitente = EmitenteDps.Prestador,
                Prestador = new PrestadorDps { CNPJ = "12ABC678000199" },
                Tomador = new InfoPessoaNFSe
                {
                    Nif = "US123456789",
                    Nome = "Global Tech Corporation",
                    Email = "billing@globaltech.com",
                    Endereco = new EnderecoNFSe
                    {
                        Logradouro = "Silicon Valley Blvd",
                        Numero = "100",
                        Municipio = new MunicipioExterior
                        {
                            CodigoPais = "US",
                            EnderecoPostal = "95112",
                            Cidade = "San Jose",
                            EstadoProvincia = "California"
                        }
                    }
                },
                Servico = new ServicoNFSe
                {
                    Localidade = new LocalidadeNFSe { CodMunicipioPrestacao = "3550308" },
                    Informacoes = new InformacoesServico { CodTributacaoNacional = "010101", Descricao = "Exportação de Software" },
                    ServicoExterior = new ServicoExterior
                    {
                        Modo = ModoPrestacao.Transfronteirico,
                        CodMoeda = 220, // USD
                        ValorServico = 5000.00m
                    }
                },
                Valores = new ValoresDps
                {
                    ValoresServico = new ValoresServico { Valor = 27500.00m }
                }
            }
        };

        dps.GerarId();

        var xml = dps.GetXml();
        await Assert.That(xml).Contains("<NIF>US123456789</NIF>");
        await Assert.That(xml).Contains("<endExt>");
        await Assert.That(xml).Contains("<cPais>US</cPais>");
        await Assert.That(xml).Contains("<cEndPost>95112</cEndPost>");
        await Assert.That(xml).Contains("<xCidade>San Jose</xCidade>");
        await Assert.That(xml).Contains("<comExt>");
        await Assert.That(xml).Contains("<vServMoeda>5000.00</vServMoeda>");

        var element = XElement.Parse(xml);
        var dpsLida = Dps.ReadFromXml(element);

        await Assert.That(dpsLida).IsNotNull();
        await Assert.That(dpsLida!.Informacoes.Tomador!.Nif).IsEqualTo("US123456789");
        await Assert.That(dpsLida.Informacoes.Tomador.Nome).IsEqualTo("Global Tech Corporation");
        await Assert.That(dpsLida.Informacoes.Tomador.Endereco!.Municipio is MunicipioExterior).IsTrue();
        var munExt = (MunicipioExterior)dpsLida.Informacoes.Tomador.Endereco.Municipio!;
        await Assert.That(munExt.CodigoPais).IsEqualTo("US");
        await Assert.That(munExt.Cidade).IsEqualTo("San Jose");
        await Assert.That(munExt.EnderecoPostal).IsEqualTo("95112");
        await Assert.That(dpsLida.Informacoes.Servico.ServicoExterior).IsNotNull();
        await Assert.That(dpsLida.Informacoes.Servico.ServicoExterior!.ValorServico).IsEqualTo(5000.00m);
    }

    [Test]
    public async Task Dps_SerializacaoEDesserializacao_ComObra()
    {
        var dps = new Dps
        {
            Versao = VersaoNFSe.Ve101,
            Informacoes = new InfDps
            {
                TipoAmbiente = DFeTipoAmbiente.Homologacao,
                DhEmissao = DateTime.Now,
                LocalidadeEmitente = "3550308",
                Serie = "1",
                NumeroDps = "1002",
                Competencia = DateTime.Today,
                TipoEmitente = EmitenteDps.Prestador,
                Prestador = new PrestadorDps { CNPJ = "12ABC678000199" },
                Tomador = new InfoPessoaNFSe { CNPJ = "98XYZ432000188" },
                Servico = new ServicoNFSe
                {
                    Localidade = new LocalidadeNFSe { CodMunicipioPrestacao = "3550308" },
                    Informacoes = new InformacoesServico { CodTributacaoNacional = "070201", Descricao = "Execução de Obra" },
                    Obra = new ObraNFSe
                    {
                        CodObra = "OBRA-9988",
                        InscricaoImobiliaria = "INSC-IMO-001"
                    }
                },
                Valores = new ValoresDps
                {
                    ValoresServico = new ValoresServico { Valor = 15000.00m }
                }
            }
        };

        dps.GerarId();

        var xml = dps.GetXml();
        await Assert.That(xml).Contains("<obra>");
        await Assert.That(xml).Contains("<cObra>OBRA-9988</cObra>");
        await Assert.That(xml).Contains("<inscImobFisc>INSC-IMO-001</inscImobFisc>");

        var element = XElement.Parse(xml);
        var dpsLida = Dps.ReadFromXml(element);

        await Assert.That(dpsLida).IsNotNull();
        await Assert.That(dpsLida!.Informacoes.Servico.Obra).IsNotNull();
        await Assert.That(dpsLida.Informacoes.Servico.Obra!.CodObra).IsEqualTo("OBRA-9988");
        await Assert.That(dpsLida.Informacoes.Servico.Obra.InscricaoImobiliaria).IsEqualTo("INSC-IMO-001");
    }

    [Test]
    public async Task Dps_SerializacaoEDesserializacao_ComExploracaoRodoviaria()
    {
        var dps = new Dps
        {
            Versao = VersaoNFSe.Ve101,
            Informacoes = new InfDps
            {
                TipoAmbiente = DFeTipoAmbiente.Homologacao,
                DhEmissao = DateTime.Now,
                LocalidadeEmitente = "3550308",
                Serie = "1",
                NumeroDps = "1003",
                Competencia = DateTime.Today,
                TipoEmitente = EmitenteDps.Prestador,
                Prestador = new PrestadorDps { CNPJ = "12ABC678000199" },
                Tomador = new InfoPessoaNFSe { CNPJ = "98XYZ432000188" },
                Servico = new ServicoNFSe
                {
                    Localidade = new LocalidadeNFSe { CodMunicipioPrestacao = "3550308" },
                    Informacoes = new InformacoesServico { CodTributacaoNacional = "220101", Descricao = "Pedágio Rodoviário" },
                    ExploracaoRodoviaria = new ExploracaoRodoviaria
                    {
                        Categoria = CategoriaVeiculo.AutomovelCaminhoneteFurgao,
                        NumeroEixos = 4,
                        OrientacaoPesagem = 1,
                        Placa = "ABC1D23"
                    }
                },
                Valores = new ValoresDps
                {
                    ValoresServico = new ValoresServico { Valor = 25.50m }
                }
            }
        };

        dps.GerarId();

        var xml = dps.GetXml();
        await Assert.That(xml).Contains("<explRod>");
        await Assert.That(xml).Contains("<categVeic>01</categVeic>");
        await Assert.That(xml).Contains("<nEixos>4</nEixos>");
        await Assert.That(xml).Contains("<sentido>1</sentido>");
        await Assert.That(xml).Contains("<placa>ABC1D23</placa>");

        var element = XElement.Parse(xml);
        var dpsLida = Dps.ReadFromXml(element);

        await Assert.That(dpsLida).IsNotNull();
        await Assert.That(dpsLida!.Informacoes.Servico.ExploracaoRodoviaria).IsNotNull();
        await Assert.That(dpsLida.Informacoes.Servico.ExploracaoRodoviaria!.Categoria).IsEqualTo(CategoriaVeiculo.AutomovelCaminhoneteFurgao);
        await Assert.That(dpsLida.Informacoes.Servico.ExploracaoRodoviaria.NumeroEixos).IsEqualTo(4);
        await Assert.That(dpsLida.Informacoes.Servico.ExploracaoRodoviaria.Placa).IsEqualTo("ABC1D23");
    }

    #endregion DPS Tests

    #region PedidoRegistroEvento Tests

    [Test]
    public async Task PedidoRegistroEvento_Cancelamento_SerializacaoEDesserializacao()
    {
        var evento = new PedidoRegistroEvento
        {
            Versao = VersaoNFSe.Ve100,
            Informacoes = new InfPedReg
            {
                Id = "PEDREG3550308212ABC6780001990000100000000001234500001",
                TipoAmbiente = DFeTipoAmbiente.Homologacao,
                DhEvento = new DateTime(2026, 8, 16, 15, 0, 0),
                VersaoAplicacao = "OpenAC 1.0",
                ChNFSe = "3550308212ABC67800019900001000000000012345",
                CNPJAutor = "12ABC678000199",
                Evento = new EventoCancelamento
                {
                    CodMotivo = MotivoCancelamento.ErroEmissao,
                    Motivo = "Cancelamento solicitado por erro na emissão dos valores."
                }
            }
        };

        var xml = evento.GetXml();
        await Assert.That(xml).Contains("<pedRegEvento");
        await Assert.That(xml).Contains("<chNFSe>3550308212ABC67800019900001000000000012345</chNFSe>");
        await Assert.That(xml).Contains("<e101101>");
        await Assert.That(xml).Contains("<cMotivo>1</cMotivo>");
        await Assert.That(xml).Contains("<xMotivo>Cancelamento solicitado por erro na emissão dos valores.</xMotivo>");

        var element = XElement.Parse(xml);
        var eventoLido = PedidoRegistroEvento.ReadFromXml(element);

        await Assert.That(eventoLido).IsNotNull();
        await Assert.That(eventoLido!.Informacoes.ChNFSe).IsEqualTo("3550308212ABC67800019900001000000000012345");
        await Assert.That(eventoLido.Informacoes.CNPJAutor).IsEqualTo("12ABC678000199");
        await Assert.That(eventoLido.Informacoes.Evento is EventoCancelamento).IsTrue();
        var cancelamento = (EventoCancelamento)eventoLido.Informacoes.Evento;
        await Assert.That(cancelamento.CodMotivo).IsEqualTo(MotivoCancelamento.ErroEmissao);
        await Assert.That(cancelamento.Motivo).IsEqualTo("Cancelamento solicitado por erro na emissão dos valores.");
    }

    [Test]
    public async Task PedidoRegistroEvento_SolicitacaoCancelamento_SerializacaoEDesserializacao()
    {
        var evento = new PedidoRegistroEvento
        {
            Versao = VersaoNFSe.Ve100,
            Informacoes = new InfPedReg
            {
                TipoAmbiente = DFeTipoAmbiente.Homologacao,
                DhEvento = DateTime.Now,
                ChNFSe = "3550308212ABC67800019900001000000000012345",
                CNPJAutor = "98XYZ432000188",
                Evento = new EventoSolicitacaoCancelamento
                {
                    CodMotivo = JustificativaAnalise.ServicoNãoPrestado,
                    Motivo = "Serviço cancelado pelo cliente"
                }
            }
        };

        var xml = evento.GetXml();
        await Assert.That(xml).Contains("<e101103>");
        await Assert.That(xml).Contains("<cMotivo>2</cMotivo>");

        var element = XElement.Parse(xml);
        var eventoLido = PedidoRegistroEvento.ReadFromXml(element);

        await Assert.That(eventoLido).IsNotNull();
        await Assert.That(eventoLido!.Informacoes.Evento is EventoSolicitacaoCancelamento).IsTrue();
        var sol = (EventoSolicitacaoCancelamento)eventoLido.Informacoes.Evento;
        await Assert.That(sol.CodMotivo).IsEqualTo(JustificativaAnalise.ServicoNãoPrestado);
    }

    [Test]
    public async Task PedidoRegistroEvento_CancelamentoPorSubstituicao_SerializacaoEDesserializacao()
    {
        var evento = new PedidoRegistroEvento
        {
            Versao = VersaoNFSe.Ve100,
            Informacoes = new InfPedReg
            {
                TipoAmbiente = DFeTipoAmbiente.Homologacao,
                DhEvento = DateTime.Now,
                ChNFSe = "3550308212ABC67800019900001000000000012345",
                CNPJAutor = "12ABC678000199",
                Evento = new EventoCancelamentoPorSubstituicao
                {
                    CodMotivo = JustificativaSubstituicao.Outros,
                    Motivo = "Substituída pela NFS-e subsequente",
                    ChaveSubstituta = "3550308212ABC67800019900001000000000099999"
                }
            }
        };

        var xml = evento.GetXml();
        await Assert.That(xml).Contains("<e105102>");
        await Assert.That(xml).Contains("<chSubstituta>3550308212ABC67800019900001000000000099999</chSubstituta>");

        var element = XElement.Parse(xml);
        var eventoLido = PedidoRegistroEvento.ReadFromXml(element);

        await Assert.That(eventoLido).IsNotNull();
        await Assert.That(eventoLido!.Informacoes.Evento is EventoCancelamentoPorSubstituicao).IsTrue();
        var cancSubst = (EventoCancelamentoPorSubstituicao)eventoLido.Informacoes.Evento;
        await Assert.That(cancSubst.ChaveSubstituta).IsEqualTo("3550308212ABC67800019900001000000000099999");
    }

    [Test]
    public async Task PedidoRegistroEvento_Confirmacoes_SerializacaoEDesserializacao()
    {
        // 1. Confirmacao Prestador
        var evPrest = new PedidoRegistroEvento
        {
            Versao = VersaoNFSe.Ve100,
            Informacoes = new InfPedReg
            {
                TipoAmbiente = DFeTipoAmbiente.Homologacao,
                DhEvento = DateTime.Now,
                ChNFSe = "3550308212ABC67800019900001000000000012345",
                CNPJAutor = "12ABC678000199",
                Evento = new EventoConfirmacaoPrestador()
            }
        };
        var xmlPrest = evPrest.GetXml();
        await Assert.That(xmlPrest).Contains("<e202201");
        var lidoPrest = PedidoRegistroEvento.ReadFromXml(XElement.Parse(xmlPrest));
        await Assert.That(lidoPrest!.Informacoes.Evento is EventoConfirmacaoPrestador).IsTrue();

        // 2. Confirmacao Tomador
        var evToma = new PedidoRegistroEvento
        {
            Versao = VersaoNFSe.Ve100,
            Informacoes = new InfPedReg
            {
                TipoAmbiente = DFeTipoAmbiente.Homologacao,
                DhEvento = DateTime.Now,
                ChNFSe = "3550308212ABC67800019900001000000000012345",
                CNPJAutor = "98XYZ432000188",
                Evento = new EventoConfirmacaoTomador()
            }
        };
        var xmlToma = evToma.GetXml();
        await Assert.That(xmlToma).Contains("<e203202");
        var lidoToma = PedidoRegistroEvento.ReadFromXml(XElement.Parse(xmlToma));
        await Assert.That(lidoToma!.Informacoes.Evento is EventoConfirmacaoTomador).IsTrue();

        // 3. Confirmacao Tacita
        var evTacita = new PedidoRegistroEvento
        {
            Versao = VersaoNFSe.Ve100,
            Informacoes = new InfPedReg
            {
                TipoAmbiente = DFeTipoAmbiente.Homologacao,
                DhEvento = DateTime.Now,
                ChNFSe = "3550308212ABC67800019900001000000000012345",
                CNPJAutor = "00000000000191",
                Evento = new EventoConfirmacaoTacita()
            }
        };
        var xmlTacita = evTacita.GetXml();
        await Assert.That(xmlTacita).Contains("<e203204");
        var lidoTacita = PedidoRegistroEvento.ReadFromXml(XElement.Parse(xmlTacita));
        await Assert.That(lidoTacita!.Informacoes.Evento is EventoConfirmacaoTacita).IsTrue();
    }

    [Test]
    public async Task PedidoRegistroEvento_Rejeicoes_SerializacaoEDesserializacao()
    {
        var evRej = new PedidoRegistroEvento
        {
            Versao = VersaoNFSe.Ve100,
            Informacoes = new InfPedReg
            {
                TipoAmbiente = DFeTipoAmbiente.Homologacao,
                DhEvento = DateTime.Now,
                ChNFSe = "3550308212ABC67800019900001000000000012345",
                CNPJAutor = "98XYZ432000188",
                Evento = new EventoRejeicaoTomador
                {
                    CodMotivo = MotivoRejeicao.NaoOcorrencia,
                    Motivo = "Serviço não ocorrido de fato pelo tomador."
                }
            }
        };

        var xml = evRej.GetXml();
        await Assert.That(xml).Contains("<e203206>");
        await Assert.That(xml).Contains("<cMotivo>3</cMotivo>");
        await Assert.That(xml).Contains("<xMotivo>Serviço não ocorrido de fato pelo tomador.</xMotivo>");

        var element = XElement.Parse(xml);
        var lido = PedidoRegistroEvento.ReadFromXml(element);

        await Assert.That(lido).IsNotNull();
        await Assert.That(lido!.Informacoes.Evento is EventoRejeicaoTomador).IsTrue();
        var rej = (EventoRejeicaoTomador)lido.Informacoes.Evento;
        await Assert.That(rej.CodMotivo).IsEqualTo(MotivoRejeicao.NaoOcorrencia);
        await Assert.That(rej.Motivo).IsEqualTo("Serviço não ocorrido de fato pelo tomador.");
    }

    #endregion PedidoRegistroEvento Tests

    #region NotaFiscalServico Tests

    [Test]
    public async Task NotaFiscalServico_SerializacaoEDesserializacao_Completa()
    {
        var nfse = new NotaFiscalServico
        {
            Versao = "1.00",
            Informacoes = new InfNFSe
            {
                Id = "NFS3550308212ABC67800019900001000000000012345",
                NumeroNFSe = 12345,
                LocalEmissao = "São Paulo",
                CodLocalIncidencia = "3550308",
                LocalIncidencia = "São Paulo",
                DescricaoTributoNacional = "Serviços de TI",
                AmbienteGerador = AmbienteGerador.Prefeitura,
                SituacaoNFSe = StatusNFSe.Gerada,
                DhProcessamento = new DateTimeOffset(2026, 8, 16, 14, 0, 0, TimeSpan.FromHours(-3)),
                Emitente = new EmitenteNFSe
                {
                    CNPJ = "12ABC678000199",
                    InscricaoMunicipal = "123456",
                    RazaoSocial = "Prestador NFS-e Ltda",
                    Endereco = new EnderecoEmitente
                    {
                        Logradouro = "Rua Central",
                        Numero = "50",
                        Bairro = "Centro",
                        CodMunicipio = "3550308",
                        UF = "SP",
                        CEP = "01001000"
                    }
                },
                Valores = new ValoresNFSe
                {
                    ValorBc = 5000.00m,
                    ValorLiquido = 4750.00m,
                    Aliquota = 5.00m,
                    ValorISSQN = 250.00m
                }
            }
        };

        var xml = nfse.GetXml();
        await Assert.That(xml).Contains("<NFSe");
        await Assert.That(xml).Contains("<infNFSe");
        await Assert.That(xml).Contains("<nNFSe>12345</nNFSe>");
        await Assert.That(xml).Contains("<vBC>5000.00</vBC>");
        await Assert.That(xml).Contains("<vLiq>4750.00</vLiq>");

        var element = XElement.Parse(xml);
        var nfseLida = NotaFiscalServico.ReadFromXml(element);

        await Assert.That(nfseLida).IsNotNull();
        await Assert.That(nfseLida!.Informacoes.NumeroNFSe).IsEqualTo(12345);
        await Assert.That(nfseLida.Informacoes.Emitente.CNPJ).IsEqualTo("12ABC678000199");
        await Assert.That(nfseLida.Informacoes.Emitente.RazaoSocial).IsEqualTo("Prestador NFS-e Ltda");
        await Assert.That(nfseLida.Informacoes.Valores.ValorBc).IsEqualTo(5000.00m);
        await Assert.That(nfseLida.Informacoes.Valores.ValorLiquido).IsEqualTo(4750.00m);
    }

    #endregion NotaFiscalServico Tests

    #region Modelos Isolados Tests

    [Test]
    public async Task BeneficioMunicipal_SerializacaoEDesserializacao_Isolada()
    {
        var bm = new BeneficioMunicipal
        {
            NumeroBeneficio = "123456",
            ValorReducao = 150.75m,
            PorcentagemReducao = 10.00m
        };

        var elem = bm.WriteToXml("BM");
        await Assert.That(elem).IsNotNull();
        await Assert.That(elem.Element("nBM")?.Value).IsEqualTo("123456");
        await Assert.That(elem.Element("vRedBCBM")?.Value).IsEqualTo("150.75");
        await Assert.That(elem.Element("pRedBCBM")?.Value).IsEqualTo("10.00");

        var bmLido = BeneficioMunicipal.ReadFromXml(elem);
        await Assert.That(bmLido).IsNotNull();
        await Assert.That(bmLido!.NumeroBeneficio).IsEqualTo("123456");
        await Assert.That(bmLido.ValorReducao).IsEqualTo(150.75m);
        await Assert.That(bmLido.PorcentagemReducao).IsEqualTo(10.00m);
    }

    [Test]
    public async Task DocumentoDeducaoReducao_SerializacaoEDesserializacao_Isolada()
    {
        var doc = new DocumentoDeducaoReducao
        {
            TipoDeducaoReducao = TipoDeducaoReducao.Materiais,
            DataEmissao = new DateTime(2026, 8, 15),
            ValorDedutivelRedutivel = 1000m,
            ValorReducaoDeducao = 200m,
            ChaveNFe = "35260812345678000199550010000001231000000010"
        };

        var elem = doc.WriteToXml("docDedRed");
        await Assert.That(elem).IsNotNull();
        await Assert.That(elem.Element("chNFe")?.Value).IsEqualTo("35260812345678000199550010000001231000000010");
        await Assert.That(elem.Element("tpDedRed")?.Value).IsEqualTo("2");

        var docLido = DocumentoDeducaoReducao.ReadFromXml(elem);
        await Assert.That(docLido).IsNotNull();
        await Assert.That(docLido!.TipoDeducaoReducao).IsEqualTo(TipoDeducaoReducao.Materiais);
        await Assert.That(docLido.ChaveNFe).IsEqualTo("35260812345678000199550010000001231000000010");
        await Assert.That(docLido.ValorDedutivelRedutivel).IsEqualTo(1000m);
        await Assert.That(docLido.ValorReducaoDeducao).IsEqualTo(200m);
    }

    [Test]
    public async Task EnderecoNFSe_SerializacaoEDesserializacao_Isolada()
    {
        var end = new EnderecoNFSe
        {
            Logradouro = "Avenida Central",
            Numero = "500",
            Complemento = "Andar 10",
            Bairro = "Bela Vista",
            Municipio = new MunicipioNacional
            {
                CodMunicipio = "3550308",
                CEP = "01310100"
            }
        };

        var elem = end.WriteToXml("end");
        await Assert.That(elem).IsNotNull();
        await Assert.That(elem.Element("xLgr")?.Value).IsEqualTo("Avenida Central");
        await Assert.That(elem.Element("nro")?.Value).IsEqualTo("500");
        await Assert.That(elem.Element("xCpl")?.Value).IsEqualTo("Andar 10");
        await Assert.That(elem.Element("xBairro")?.Value).IsEqualTo("Bela Vista");
        await Assert.That(elem.Element("endNac")?.Element("cMun")?.Value).IsEqualTo("3550308");
        await Assert.That(elem.Element("endNac")?.Element("CEP")?.Value).IsEqualTo("01310100");

        var endLido = EnderecoNFSe.ReadFromXml(elem);
        await Assert.That(endLido).IsNotNull();
        await Assert.That(endLido!.Logradouro).IsEqualTo("Avenida Central");
        await Assert.That(endLido.Numero).IsEqualTo("500");
        await Assert.That(endLido.Complemento).IsEqualTo("Andar 10");
        await Assert.That(endLido.Bairro).IsEqualTo("Bela Vista");
        await Assert.That(endLido.Municipio is MunicipioNacional).IsTrue();
        var mun = (MunicipioNacional)endLido.Municipio!;
        await Assert.That(mun.CodMunicipio).IsEqualTo("3550308");
        await Assert.That(mun.CEP).IsEqualTo("01310100");
    }

    [Test]
    public async Task EnderecoExterior_SerializacaoEDesserializacao_Isolada()
    {
        var endExt = new EnderecoExterior
        {
            EnderecoPostal = "97477",
            Cidade = "SPRINGFIELD",
            EstadoProvincia = "OREGON"
        };

        var elem = endExt.WriteToXml("endExt");
        await Assert.That(elem).IsNotNull();
        await Assert.That(elem.Element("cEndPost")?.Value).IsEqualTo("97477");
        await Assert.That(elem.Element("xCidade")?.Value).IsEqualTo("SPRINGFIELD");
        await Assert.That(elem.Element("xEstProvReg")?.Value).IsEqualTo("OREGON");

        var endLido = EnderecoExterior.ReadFromXml(elem);
        await Assert.That(endLido).IsNotNull();
        await Assert.That(endLido!.EnderecoPostal).IsEqualTo("97477");
        await Assert.That(endLido.Cidade).IsEqualTo("SPRINGFIELD");
        await Assert.That(endLido.EstadoProvincia).IsEqualTo("OREGON");
    }

    [Test]
    public async Task ValoresDps_SerializacaoEDesserializacao_Isolada()
    {
        var val = new ValoresDps
        {
            ValoresServico = new ValoresServico
            {
                Valor = 1000m
            },
            ValoresDesconto = new ValoresDesconto
            {
                ValorIncodicional = 50m,
                ValorCondicional = 25m
            },
            Tributos = new TributosNFSe
            {
                Municipal = new TributoMunicipal
                {
                    ISSQN = TributoISSQN.OperacaoTributavel,
                    Aliquota = 3m
                }
            }
        };

        var elem = val.WriteToXml("valores");
        await Assert.That(elem).IsNotNull();
        await Assert.That(elem.Element("vServPrest")?.Element("vServ")?.Value).IsEqualTo("1000.00");
        await Assert.That(elem.Element("vDescCondIncond")?.Element("vDescCond")?.Value).IsEqualTo("25.00");
        await Assert.That(elem.Element("vDescCondIncond")?.Element("vDescIncond")?.Value).IsEqualTo("50.00");

        var valLido = ValoresDps.ReadFromXml(elem);
        await Assert.That(valLido).IsNotNull();
        await Assert.That(valLido!.ValoresServico.Valor).IsEqualTo(1000m);
        await Assert.That(valLido.ValoresDesconto!.ValorIncodicional).IsEqualTo(50m);
        await Assert.That(valLido.ValoresDesconto.ValorCondicional).IsEqualTo(25m);
        await Assert.That(valLido.Tributos.Municipal!.Aliquota).IsEqualTo(3m);
    }

    [Test]
    public async Task RTCIBSCBS_SerializacaoEDesserializacao_Isolada()
    {
        var ibsCbs = new RTCIBSCBS
        {
            CodigoLocalidadeIncidencia = "3550308",
            DescricaoLocalidadeIncidencia = "São Paulo",
            PercentualRedutor = 20m,
            Valores = new RTCValoresIBSCBS
            {
                ValorBaseCalculo = 2000m,
                Federal = new RTCValoresIBSCBSFederal
                {
                    PercentualCBS = 0.90m,
                    PercentualAliquotaEfetivaCBS = 0.90m
                },
                Estado = new RTCValoresIBSCBSUf
                {
                    PercentualIBSUf = 0.10m,
                    PercentualAliquotaEfetivaUf = 0.10m
                },
                Municipio = new RTCValoresIBSCBSMunicipio
                {
                    PercentualIBSMunicipal = 0.05m,
                    PercentualAliquotaEfetivaMunicipal = 0.05m
                }
            },
            Totais = new RTCTotalCIBS
            {
                ValorTotalNF = 2000m,
                TotalCBS = new RTCTotalCBS { ValorCBS = 18.00m },
                TotalIBS = new RTCTotalIBS { ValorTotalIBS = 3.00m }
            }
        };

        var elem = ibsCbs.WriteToXml("IBSCBS");
        await Assert.That(elem).IsNotNull();
        await Assert.That(elem.Element("cLocalidadeIncid")?.Value).IsEqualTo("3550308");
        await Assert.That(elem.Element("pRedutor")?.Value).IsEqualTo("20.00");
        await Assert.That(elem.Element("valores")?.Element("vBC")?.Value).IsEqualTo("2000.00");

        var ibsCbsLido = RTCIBSCBS.ReadFromXml(elem);
        await Assert.That(ibsCbsLido).IsNotNull();
        await Assert.That(ibsCbsLido!.CodigoLocalidadeIncidencia).IsEqualTo("3550308");
        await Assert.That(ibsCbsLido.PercentualRedutor).IsEqualTo(20m);
        await Assert.That(ibsCbsLido.Valores.ValorBaseCalculo).IsEqualTo(2000m);
        await Assert.That(ibsCbsLido.Totais.TotalCBS.ValorCBS).IsEqualTo(18.00m);
    }

    #endregion Modelos Isolados Tests

    #region Webservice Services Tests

    [Test]
    public async Task NFSeServices_CarregamentoDoRecursoEmbutido()
    {
        var manager = NFSeServiceManager.Instance;
        await Assert.That(manager).IsNotNull();
        await Assert.That(manager.Services).IsNotNull();
        await Assert.That(manager.Services.Webservices.Count).IsGreaterThan(0);

        // Verifica provedores conhecidos no arquivo de recursos
        var nac = manager.Services.Webservices.FirstOrDefault(w => w.Codigo == -1);
        await Assert.That(nac).IsNotNull();
        await Assert.That(nac!.Nome).Contains("Nacional");
        await Assert.That(nac.Ambientes.Count).IsGreaterThan(0);

        var doisCorregos = manager.Services.Webservices.FirstOrDefault(w => w.Codigo == 3514106);
        await Assert.That(doisCorregos).IsNotNull();
        await Assert.That(doisCorregos!.Nome).Contains("DOIS CÓRREGOS");
    }

    #endregion Webservice Services Tests
}
