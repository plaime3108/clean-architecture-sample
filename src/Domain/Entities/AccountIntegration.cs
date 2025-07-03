namespace Domain.Entities
{
    public class AccountIntegration
    {
        public short Pgcod { get; set; }
        public int Ctnro { get; set; }
        public short Pepais { get; set; }
        public short Petdoc { get; set; }
        public string Pendoc { get; set; } = string.Empty;
        public short Ttcod { get; set; }
        public string Cttfir { get; set; } = string.Empty;
    }
}