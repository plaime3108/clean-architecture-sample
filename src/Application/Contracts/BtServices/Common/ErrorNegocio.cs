namespace Application.Contracts.BtServices.Common
{
    public class ErrorNegocio
    {
        public IEnumerable<Error> BTErrorNegocio { get; set; } = new List<Error>();
    }
    public class Error
    {
        public int Codigo { get; set; }
        public string Descripcion { get; set; } = string.Empty;
        public string Severidad { get; set; } = string.Empty;
    }
}
