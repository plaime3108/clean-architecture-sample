using Application.Contracts.BtServices.Common;

namespace Application.Contracts.BtServices.Credits
{
    public class GetPendingInstRequest : BaseRequest
    {
        public short Company { get; set; }
        public short Branch { get; set; }
        public short Module { get; set; }
        public short Currency { get; set; }
        public short Paper { get; set; }
        public int Account { get; set; }
        public int Operation { get; set; }
        public short Suboperation { get; set; }
        public short Operationtype { get; set; }
    }
}
