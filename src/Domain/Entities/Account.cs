namespace Domain.Entities
{
    public class Account
    {
        public decimal BTSIO00Id { get; set; }
        public short BTSIO00Emp { get; set; }
        public short BTSIO00Mod { get; set; }
        public short BTSIO00Suc { get; set; }
        public short BTSIO00Mda { get; set; }
        public short BTSIO00Pap { get; set; }
        public int BTSIO00Cta { get; set; }
        public int BTSIO00Ope { get; set; }
        public short BTSIO00Sub { get; set; }
        public short BTSIO00Top { get; set; }
        public Guid BTSIO00Guid { get; set; }
        public string BTSIO00Fac { get; set; } = string.Empty;
        public short BTSIO00Est { get; set; }
    }
}
