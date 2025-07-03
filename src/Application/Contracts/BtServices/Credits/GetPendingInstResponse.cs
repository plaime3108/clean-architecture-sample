using Application.Contracts.BtServices.Common;

namespace Application.Contracts.BtServices.Credits
{
    public class GetPendingInstResponse : BaseResponse
    {
        public int Principalbalance { get; set; }
        public int Totalinst { get; set; }
        public int Legalexpenses { get; set; }
        public string Pendinginst { get; set; } = string.Empty;
        public short Error { get; set; }
        public string Descerror { get; set; } = string.Empty;
    }
}
