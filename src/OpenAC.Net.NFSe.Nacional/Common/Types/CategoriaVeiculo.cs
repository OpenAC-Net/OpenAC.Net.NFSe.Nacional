// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe.Nacional
// Author           : RFTD
// Created          : 09-09-2023
//
// Last Modified By : RFTD
// Last Modified On : 09-09-2023
// ***********************************************************************
// <copyright file="CategoriaVeiculo.cs" company="OpenAC .Net">
//		        		   The MIT License (MIT)
//	     		    Copyright (c) 2014-2023 Grupo OpenAC.Net
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

using OpenAC.Net.DFe.Core.Attributes;

namespace OpenAC.Net.NFSe.Nacional.Common.Types;

/// <summary>
/// Categorias de veículos para cobrança.
/// </summary>
public enum CategoriaVeiculo
{
    /// <summary>
    /// 00 - Categoria de veículos (tipo não informado na nota de origem)
    /// </summary>
    [DFeEnum("00")]
    NaoInformado,

    /// <summary>
    /// 01 - Automóvel, caminhonete e furgão
    /// </summary>
    [DFeEnum("01")]
    AutomovelCaminhoneteFurgao,

    /// <summary>
    /// 02 - Caminhão leve, ônibus, caminhão trator e furgão
    /// </summary>
    [DFeEnum("02")]
    CaminhaoLeveonibusCaminhaoTratorFurgao,

    /// <summary>
    /// 03 - Automóvel e caminhonete com semireboque
    /// </summary>
    [DFeEnum("03")]
    AutomovelCaminhoneteComSemireboque,

    /// <summary>
    /// 04 - Caminhão, caminhão-trator, caminhão-trator com semi-reboque e ônibus
    /// </summary>
    [DFeEnum("04")]
    CaminhaoCaminhãoTratorCaminhaoTratorComSemiReboqueOnibus,

    /// <summary>
    /// 05 - Automóvel e caminhonete com reboque
    /// </summary>
    [DFeEnum("05")]
    AutomovelCaminhoneteReboque,

    /// <summary>
    /// 06 - Caminhão com reboque
    /// </summary>
    [DFeEnum("06")]
    CaminhaoComReboque,

    /// <summary>
    /// 07 - Caminhão trator com semi-reboque
    /// </summary>
    [DFeEnum("07")]
    CaminhaoTratorComSemiReboque,

    /// <summary>
    /// 08 - Motocicletas, motonetas e bicicletas motorizadas
    /// </summary>
    [DFeEnum("08")]
    MotocicletasMotonetasBicicletasMotorizadas,

    /// <summary>
    /// 09 - Veículo especial
    /// </summary>
    [DFeEnum("09")]
    VeiculoSspecial,

    /// <summary>
    /// 10 - Veículo Isento
    /// </summary>
    [DFeEnum("10")]
    VeiculoIsento
}