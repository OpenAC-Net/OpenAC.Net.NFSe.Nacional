using System;

namespace OpenAC.Net.NFSe.Nacional.Community;

/// <summary>
/// Métodos de extensão para a classe `OpenNFSeNacional`.
/// Fornece acesso facilitado ao provedor de NFSe associado às configurações/webservice.
/// </summary>
public static class OpenNFSeNacionalExtension
{
    extension(OpenNFSeNacional nfSeNacional)
    {
        /// <summary>
        /// Obtém ou define o <see cref="NFSeProvider"/> usado pela instância de <see cref="OpenNFSeNacional"/>.
        /// </summary>
        /// <remarks>
        /// O getter resolve o provedor a partir do tipo do webservice atual. Se o webservice não
        /// estiver inicializado, uma <see cref="InvalidOperationException"/> será lançada.
        /// O setter atribui um webservice compatível com o provedor informado usando as configurações atuais.
        /// </remarks>
        public NFSeProvider Provider
        {
            get => ProviderManager.GetProvider(nfSeNacional.Webservice?.GetType() ?? throw new InvalidOperationException("Webservice não foi inicializado. Verifique as configurações."));
            set => nfSeNacional.Webservice = ProviderManager.GetProvider(value, nfSeNacional.Configuracoes);
        }
    }
}