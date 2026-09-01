# OpenAC.Net.NFSe.Nacional.DANFSe.PDFSharp

Biblioteca .NET para geração e impressão do **DANFSe (Documento Auxiliar da NFS-e) Padrão Nacional** em PDF utilizando **PDFsharp / PdfSharpCore** (100% Open Source, licença MIT).

Compatível com a **Nota Técnica 008, versão 1.02, de 14/07/2026**, do Sistema Nacional da NFS-e (DANFSe v2.0 com suporte à Reforma Tributária IBS/CBS).

## Referência normativa

- [Nota Técnica SE/CGNFS-e nº 008, versão 1.02](https://www.gov.br/nfse/pt-br/biblioteca/documentacao-tecnica/rtc/nt-008-se-cgnfse-danfse-20260714-v1-02.pdf)
- [Documentação Técnica RTC do Sistema Nacional da NFS-e](https://www.gov.br/nfse/pt-br/biblioteca/documentacao-tecnica/rtc)

## 🚀 Como usar

```csharp
using OpenAC.Net.NFSe.Nacional.Common.Model;
using OpenAC.Net.NFSe.Nacional.DANFSe.PDFSharp;
using OpenAC.Net.NFSe.Nacional.DANFSe.PDFSharp.Configuracao;

var nota = NotaFiscalServico.Load("nfse.xml");

var config = new DANFSeNacionalConfig
{
    Homologacao = false,
    ExibirQRCode = true,
    ExibirCanhoto = false
};

```

### 🔒 Criptografia e Proteção por Senha

Você pode proteger o PDF gerado contra edição, cópia ou exigir senha para abertura:

```csharp
var config = new DANFSeNacionalConfig
{
    Seguranca = new DANFSeSegurancaConfig
    {
        SenhaUsuario = "123456",            // Senha para abrir e visualizar o PDF
        SenhaProprietario = "adminMaster",  // Senha do proprietário / administrador
        PermitirImpressao = true,           // Permite imprimir
        PermitirModificacao = false,        // Bloqueia edição do PDF
        PermitirCopiarConteudo = false      // Bloqueia cópia de texto/gráficos
    }
};

OpenDANFSeNacional.GerarPDF(nota, "danfse_protegido.pdf", config);
```

## 📄 Licença
Distribuído sob a licença **MIT**.
