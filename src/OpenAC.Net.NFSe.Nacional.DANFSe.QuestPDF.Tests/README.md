# OpenAC.Net.NFSe.Nacional.DANFSe.QuestPDF.Tests

Projeto de testes para a biblioteca `OpenAC.Net.NFSe.Nacional.DANFSe.QuestPDF`, responsável por validar a geração do **DANFSe (Documento Auxiliar da NFS-e) Padrão Nacional** em PDF utilizando **QuestPDF**.

## 🎯 Objetivo

Garantir que os PDFs do DANFSe sejam gerados corretamente para diferentes cenários de NFSe, como:

- Nota fiscal padrão
- Com/sem tomador
- Com/sem destinatário (IBS/CBS)
- Com/sem intermediário
- Com/sem informações complementares
- Com/sem IBS/CBS
- Variações de logo (nacional/prestador)
- Modo homologação
- Configurações de canhoto e QR Code

## 🛠️ Requisitos

- .NET 8.0 ou .NET 10.0
- xUnit v3 com Microsoft.Testing.Platform

## ▶️ Como executar

Para rodar todos os testes do projeto:

```bash
dotnet test src/OpenAC.Net.NFSe.Nacional.DANFSe.QuestPDF.Tests/OpenAC.Net.NFSe.Nacional.DANFSe.QuestPDF.Tests.csproj
```

Para executar em uma framework específica:

```bash
dotnet test src/OpenAC.Net.NFSe.Nacional.DANFSe.QuestPDF.Tests/OpenAC.Net.NFSe.Nacional.DANFSe.QuestPDF.Tests.csproj --framework net10.0
```

## 📁 Estrutura dos artefatos

### `DadosTeste/`

Contém o XML base (`nota.xml`) usado para gerar as variações de NFSe durante os testes.

> Apenas o `nota.xml` é mantido no repositório. As variações (`sem_tomador.xml`, `com_destinatario.xml`, etc.) são geradas dinamicamente em uma pasta temporária e descartadas após a execução.

### `Resultados/`

Após executar os testes, esta pasta conterá os **PDFs gerados** para inspeção visual:

```
Resultados/
├── nota_canhoto_True_qrcode_True.pdf
├── com_destinatario_destinatario_True.pdf
├── sem_ibs_cbs.pdf
├── nota_homologacao.pdf
└── ...
```

> Os PDFs são artefatos gerados em tempo de execução e **não devem ser versionados**.

## 🧪 Tecnologias

- **xUnit v3**: framework de testes
- **Microsoft.Testing.Platform**: executor de testes (conforme `global.json`)
- **QuestPDF**: geração dos PDFs do DANFSe

## 📄 Licença

Distribuído sob a licença **MIT**.
