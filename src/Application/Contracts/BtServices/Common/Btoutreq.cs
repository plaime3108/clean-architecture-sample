namespace Application.Contracts.BtServices.Common
{
    public class Btoutreq
    {
        public string Canal { get; set; } = string.Empty;
        public string Servicio { get; set; } = string.Empty;
        public string Fecha { get; set; } = string.Empty;
        public string Hora { get; set; } = string.Empty;
        public string Requerimiento { get; set; } = string.Empty;
        public long Numero { get; set; }
        public string Estado { get; set; } = string.Empty;
    }
}
