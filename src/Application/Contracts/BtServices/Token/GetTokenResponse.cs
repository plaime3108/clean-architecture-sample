using Application.Contracts.BtServices.Common;

namespace Application.Contracts.BtServices.Token
{
    public class GetTokenResponse : BaseResponse
    {
        public string SessionToken { get; set; } = string.Empty;
    }
}
