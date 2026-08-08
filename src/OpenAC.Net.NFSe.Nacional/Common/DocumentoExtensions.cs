// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe.Nacional
// Author           : OpenAC .Net
// ***********************************************************************
// <copyright file="DocumentoExtensions.cs" company="OpenAC .Net">
//		        		   The MIT License (MIT)
//	     		    Copyright (c) 2014-2026 Grupo OpenAC.Net
//
//	 Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the "Software"),
// to deal in the Software without restriction, including without limitation
// the rights to use, copy, modify, merge, publish, distribute, sublicense,
// and/or sell copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following conditions:
//	 The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//	 THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,
// ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.
// </copyright>
// <summary></summary>
// ***********************************************************************

using System.Text;

namespace OpenAC.Net.NFSe.Nacional.Common;

/// <summary>
/// Normalização de documentos e chaves que admitem CNPJ alfanumérico.
/// </summary>
/// <remarks>
/// A partir do layout v1.01 o CNPJ deixou de ser exclusivamente numérico (<c>TSCNPJ</c> passou de
/// <c>[0-9]{14}</c> para <c>[0-9A-Z]{14}</c>), e o mesmo vale para as chaves e identificadores que o
/// embutem (<c>TSChaveNFSe</c>, <c>TSChaveNFe</c>, <c>TSIdNumEvento</c>, …). Esses campos não podem
/// mais ser serializados com <see cref="OpenAC.Net.DFe.Core.Serializer.TipoCampo.StrNumber"/>, que
/// descarta as letras. A limpeza de formatação, que o <c>StrNumber</c> fazia implicitamente, passa a
/// ser feita aqui preservando os caracteres alfanuméricos.
/// </remarks>
internal static class DocumentoExtensions
{
    #region Methods

    /// <summary>
    /// Remove qualquer caractere de formatação (pontos, barras, hífens, espaços) mantendo apenas
    /// dígitos e letras, estas convertidas para maiúsculas.
    /// </summary>
    /// <param name="valor">Documento ou chave a normalizar.</param>
    /// <returns>
    /// O valor contendo apenas <c>[0-9A-Z]</c>, ou <c>null</c> caso <paramref name="valor"/> seja nulo.
    /// </returns>
    public static string? SomenteAlfanumerico(this string? valor)
    {
        if (valor == null) return null;

        var builder = new StringBuilder(valor.Length);

        foreach (var caractere in valor)
        {
            if (caractere is >= '0' and <= '9' or >= 'A' and <= 'Z')
                builder.Append(caractere);
            else if (caractere is >= 'a' and <= 'z')
                builder.Append((char)(caractere - 32));
        }

        return builder.ToString();
    }

    #endregion Methods
}
