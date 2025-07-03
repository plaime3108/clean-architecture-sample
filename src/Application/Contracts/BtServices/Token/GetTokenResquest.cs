using Application.Contracts.BtServices.Common;

namespace Application.Contracts.BtServices.Token
{
    public class GetTokenResquest : BaseRequest
    {
        public string UserId { get; set; } = string.Empty;
        public string UserPassword { get; set; } = string.Empty;
    }
}
