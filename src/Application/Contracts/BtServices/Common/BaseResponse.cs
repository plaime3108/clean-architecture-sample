namespace Application.Contracts.BtServices.Common
{
    public abstract class BaseResponse
    {
        public required Btinreq Btinreq { get; set; }
        public required Btoutreq Btoutreq { get; set; }
        public required ErrorNegocio Erroresnegocio { get; set; }
    }
}
