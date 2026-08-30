# Exemplo ASP.NET Core com Minimal API

Este projeto demonstra o uso conjunto das bibliotecas:

- `OpenAC.Net.NFSe.Nacional.Web`, para comunicação com os webservices da NFS-e;
- `OpenAC.Net.NFSe.Nacional.DANFSe.PDFSharp.Web`, para geração local do DANFSe em PDF.

## Configuração

Edite o `appsettings.json` e informe o município, o certificado PFX e sua senha. Caminhos relativos
são resolvidos a partir da raiz da aplicação. Cada entrada em `DANFSe:Tenants` possui configuração
própria de logo, nome de arquivo, QR Code e comportamento de download.

Não adicione certificados ou senhas reais ao controle de versão. Em produção, prefira secret
stores, variáveis de ambiente ou um `INFSeConfigurationProvider<TConfiguration>` próprio.

Também é possível sobrescrever valores por variáveis de ambiente:

```text
NFSe__CodigoMunicipio=5002704
NFSe__Certificado__Caminho=/run/secrets/certificado.pfx
NFSe__Certificado__Senha=senha
DANFSe__Tenants__default__LogoPrestador=/app/imagens/logo-padrao.png
DANFSe__Tenants__empresa-a__LogoPrestador=/app/imagens/logo-empresa-a.png
```

O `DANFSePorEmpresaConfigurationProvider` implementa
`INFSeConfigurationProvider<DANFSeNacionalWebOptions>` e cria uma configuração independente a cada
operação. Dessa forma, os bytes do logo e as demais opções nunca são compartilhados entre tenants.
As rotas sem `empresaId` utilizam `NFSeTenant.Padrao`; no `appsettings.json`, ele corresponde à
seção `DANFSe:Tenants:default`.

## Execução

```bash
dotnet run --project OpenAC.Net.NFSe.Nacional.Web.Exemplo
```

## Endpoints

| Método | Rota | Finalidade |
| --- | --- | --- |
| `POST` | `/nfse/emissoes` | Envia uma DPS e retorna a resposta fiscal. |
| `POST` | `/nfse/emissoes/pdf` | Envia uma DPS e devolve o DANFSe local quando a emissão for autorizada. |
| `POST` | `/danfse/pdf` | Gera o PDF local a partir de uma `NotaFiscalServico`. |
| `POST` | `/danfse/pdf/lote` | Gera um PDF local contendo várias notas. |
| `POST` | `/empresas/{empresaId}/danfse/pdf` | Gera o PDF com logo e opções da empresa. |
| `POST` | `/empresas/{empresaId}/danfse/pdf/lote` | Gera um lote com a configuração da empresa. |
| `GET` | `/nfse/{chave}/danfse-oficial` | Baixa o DANFSe disponibilizado pelo webservice. |

O arquivo `OpenAC.Net.NFSe.Nacional.Web.Exemplo.http` contém requisições iniciais. Os objetos DPS e
NFS-e precisam respeitar integralmente o layout fiscal; os corpos mínimos do arquivo servem apenas
para demonstrar as rotas e devem ser substituídos por documentos válidos.
