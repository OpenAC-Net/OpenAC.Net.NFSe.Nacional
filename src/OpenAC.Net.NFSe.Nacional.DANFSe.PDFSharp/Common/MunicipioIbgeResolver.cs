// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe.Nacional.DANFSe.PDFSharp
// Author           : OpenAC.Net Team
// Created          : 2026-09-01
// ***********************************************************************
// <copyright file="MunicipioIbgeResolver.cs" company="OpenAC .Net">
//		         The MIT License (MIT)
//	            Copyright (c) 2014-2026 Grupo OpenAC.Net
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OpenAC.Net.NFSe.Nacional.DANFSe.PDFSharp.Common;

/// <summary>
/// Resolve códigos de municípios segundo o catálogo oficial de localidades do IBGE.
/// </summary>
internal static class MunicipioIbgeResolver
{
    private const string ResourceName =
        "OpenAC.Net.NFSe.Nacional.DANFSe.PDFSharp.Resources.MunicipiosIBGE.json";

    private static readonly Lazy<IReadOnlyDictionary<string, MunicipioIbge>> Municipios =
        new(CarregarMunicipios);

    /// <summary>
    /// Retorna o nome e a UF do município para o código IBGE informado.
    /// </summary>
    public static string? ObterMunicipioUf(string? codigoIbge)
    {
        if (string.IsNullOrWhiteSpace(codigoIbge)) return null;

        return Municipios.Value.TryGetValue(codigoIbge!, out var municipio)
            ? $"{municipio.Nome} / {municipio.Uf}"
            : null;
    }

    private static IReadOnlyDictionary<string, MunicipioIbge> CarregarMunicipios()
    {
        var assembly = typeof(MunicipioIbgeResolver).GetTypeInfo().Assembly;
        using var stream = assembly.GetManifestResourceStream(ResourceName);
        if (stream == null) return new Dictionary<string, MunicipioIbge>();

        return JsonSerializer.Deserialize<Dictionary<string, MunicipioIbge>>(stream) ??
               new Dictionary<string, MunicipioIbge>();
    }

    private sealed class MunicipioIbge
    {
        [JsonPropertyName("nome")]
        public string Nome { get; set; } = string.Empty;

        [JsonPropertyName("uf")]
        public string Uf { get; set; } = string.Empty;
    }
}
