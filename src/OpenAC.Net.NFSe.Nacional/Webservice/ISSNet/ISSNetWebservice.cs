using OpenAC.Net.NFSe.Nacional.Common;
using OpenAC.Net.NFSe.Nacional.Webservice.Nacional;

namespace OpenAC.Net.NFSe.Nacional.Webservice.ISSNet
{
    /// <summary>
    /// Provedor: ISSNet (Nota Control).
    /// Autor: Adriano Trentim
    /// Criação: 03/01/2025
    /// </summary>
    public class ISSNetWebservice(ConfiguracaoNFSe configuracaoNFSe, NFSeServiceInfo serviceInfo) : NacionalWebservice(configuracaoNFSe, serviceInfo)
    {
        
    }
}
