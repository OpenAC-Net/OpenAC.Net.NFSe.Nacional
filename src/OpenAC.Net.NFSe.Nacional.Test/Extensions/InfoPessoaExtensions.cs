using OpenAC.Net.NFSe.Nacional.Common.Model;

namespace OpenAC.Net.NFSe.Nacional.Test.Extensions;

public static class InfoPessoaExtensions
{
    public static RTCInfoDest ToRTCInfoDest(this InfoPessoaNFSe pessoa)
    {
        return new RTCInfoDest
        {
            CNPJ = pessoa?.CNPJ,
            Nome = pessoa?.Nome ?? string.Empty,
            Endereco = pessoa?.Endereco,
            Telefone = pessoa?.Telefone,
            Email = pessoa?.Email
        };
    }
}