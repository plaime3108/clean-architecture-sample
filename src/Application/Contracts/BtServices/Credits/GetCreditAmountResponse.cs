using Application.Contracts.BtServices.Common;

namespace Application.Contracts.BtServices.Credits
{
    public class GetCreditAmountResponse : BaseResponse
    {
        public int Amountcreditdebtbs { get; set; }
        public int Amountcreditdebtusd { get; set; }
        public short Endcurrency { get; set; }
    }
}
