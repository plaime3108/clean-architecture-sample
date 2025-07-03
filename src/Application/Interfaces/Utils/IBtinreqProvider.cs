using Application.Contracts.BtServices.Common;

namespace Application.Interfaces.Utils
{
    public interface IBtinreqProvider
    {
        Btinreq GetBtinreqAsync(string token);
    }
}
