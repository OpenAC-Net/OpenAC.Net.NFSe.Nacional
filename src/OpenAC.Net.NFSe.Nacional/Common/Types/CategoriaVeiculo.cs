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
/// Categorias de veículos para cobrança:
/// 00 - Categoria de veículos (tipo não informado na nota de origem)
/// 01 - Automóvel, caminhonete e furgão;
/// 02 - Caminhão leve, ônibus, caminhão trator e furgão;
/// 03 - Automóvel e caminhonete com semireboque;
/// 04 - Caminhão, caminhão-trator, caminhão-trator com semi-reboque e ônibus;
/// 05 - Automóvel e caminhonete com reboque;
/// 06 - Caminhão com reboque;
/// 07 - Caminhão trator com semi-reboque;
/// 08 - Motocicletas, motonetas e bicicletas motorizadas;
/// 09 - Veículo especial;
/// 10 - Veículo Isento;
/// </summary>
public enum CategoriaVeiculo
{
    [DFeEnum("00")]
    NaoInformado,
    
    [DFeEnum("01")]
    AutomovelCaminhoneteFurgao,
    
    [DFeEnum("02")]
    CaminhaoLeveonibusCaminhaoTratorFurgao,
    
    [DFeEnum("03")]
    AutomovelCaminhoneteComSemireboque,
    
    [DFeEnum("04")]
    CaminhaoCaminhãoTratorCaminhaoTratorComSemiReboqueOnibus,
    
    [DFeEnum("05")]
    AutomovelCaminhoneteReboque,
    
    [DFeEnum("06")]
    CaminhaoComReboque,
    
    [DFeEnum("07")]
    CaminhaoTratorComSemiReboque,
    
    [DFeEnum("08")]
    MotocicletasMotonetasBicicletasMotorizadas,
    
    [DFeEnum("09")]
    VeiculoSspecial,
    
    [DFeEnum("10")]
    VeiculoIsento
}