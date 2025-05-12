namespace Domain.Entities
{
    public class JSBN05
    {
        public short JSBN05Pais { get; set; }
        public short JSBN05TDoc { get; set; }
        public string? JSBN05NDoc { get; set; } = string.Empty;
        public short JSBN05Pai2 { get; set; }
        public short JSBN05TDo2 { get; set; }
        public string? JSBN05Raiz { get; set; } = string.Empty;
        public string? JSBN05Comp { get; set; } = string.Empty;
        public string? JSBN05Exte { get; set; } = string.Empty;
        public short JSBN05Regu { get; set; }
        public string? JSBN05AuC1 { get; set; } = string.Empty;
        public string? JSBN05AuC2 { get; set; } = string.Empty;
        public int JSBN05AuN1 { get; set; }
        public decimal JSBN05AuI1 { get; set; }
        public DateTime JSBN05AuF1 { get; set; }
        public int JSBN05CPer { get; set; }
    }
}
