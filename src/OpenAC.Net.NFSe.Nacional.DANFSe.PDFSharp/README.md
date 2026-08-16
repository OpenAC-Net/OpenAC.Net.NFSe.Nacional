# OpenAC.Net.NFSe.Nacional.DANFSe.PDFSharp

Biblioteca .NET para geração e impressão do **DANFSe (Documento Auxiliar da NFS-e) Padrão Nacional** em PDF utilizando **PDFsharp / PdfSharpCore** (100% Open Source, licença MIT).

Compatível com a **Nota Técnica 008 do Sistema Nacional da NFS-e** (layout DANFSe v2.0 com suporte à Reforma Tributária IBS/CBS).

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
