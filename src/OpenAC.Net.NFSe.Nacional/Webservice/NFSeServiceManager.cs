using OpenAC.Net.DFe.Core.Service;

namespace OpenAC.Net.NFSe.Nacional.Webservice;

public sealed class NFSeServiceManager : DFeServices<TipoServico>
{
    private static readonly NFSeServiceManager? instance;

    public static NFSeServiceManager Instance = instance ??= new NFSeServiceManager();

    private NFSeServiceManager()
    {
        //
    }
}