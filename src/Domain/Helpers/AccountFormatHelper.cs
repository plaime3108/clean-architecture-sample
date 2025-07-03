namespace Domain.Helpers
{
    public class AccountFormatHelper
    {
        public short Cmp { get; }
        public short Mod { get; }
        public short Brn { get; }
        public short Ccy { get; }
        public short Doc { get; }
        public int Acc { get; }
        public int Opr { get; }
        public short Sop { get; }
        public short Opt { get; }

        public AccountFormatHelper(string rawValue)
        {
            Cmp = short.Parse(rawValue.Substring(0, 3));
            Brn = short.Parse(rawValue.Substring(3, 3));
            Mod = short.Parse(rawValue.Substring(6, 3));
            Ccy = short.Parse(rawValue.Substring(9, 4));
            Doc = short.Parse(rawValue.Substring(13, 4));
            Acc = int.Parse(rawValue.Substring(17, 9));
            Sop = short.Parse(rawValue.Substring(26, 3));
            Opr = int.Parse(rawValue.Substring(29, 9));
            Opt = short.Parse(rawValue.Substring(38, 3));
        }
        public static string BuildUnformattedAccount(short Cmp, short Mod, short Brn, short Ccy, short Doc, int Acc, int Opr, short Sop, short Opt)
        {
            return (1000 + Cmp).ToString("D4").Substring(1, 3) + Brn.ToString("D3").Trim() + (1000 + Mod).ToString("D4").Substring(1, 3) + (10000 + Ccy).ToString("D5").Substring(1, 4) + (10000 + Doc).ToString("D5").Substring(1, 4) +
                   (1000000000 + Acc).ToString("D10").Substring(1, 9) + (1000 + Sop).ToString("D4").Substring(1, 3) + (1000000000 + Opr).ToString("D10").Substring(1, 9) + (1000 + Opt).ToString("D4").Substring(1, 3);
        }
        public static string BuildMaskedAccount(int Acc, short Sop, int Opr, string type)
        {
            string CharactersMask = "****************";
            if (type == "AHO")
                return string.Concat(Acc.ToString().AsSpan(0, 3), CharactersMask.AsSpan(0, 3), Sop.ToString("D4").AsSpan(1, 3));
            else
                return string.Concat(Acc.ToString().AsSpan(0, 3), CharactersMask.AsSpan(0, 3), Opr.ToString("D10").AsSpan(7, 3));
        }
        public static string BuildFormattedAccount(string accountId)
        {
            if (string.IsNullOrEmpty(accountId) || accountId.Length < 30)
                return accountId;
            return $"{accountId.Substring(0, 3)}-{accountId.Substring(3, 3)}-{accountId.Substring(6, 4)}-{accountId.Substring(10, 4)}-{accountId.Substring(14, 9)}-{accountId.Substring(23, 3)}-{accountId.Substring(26, 9)}-{accountId.Substring(35, 3)}";
        }
    }
}
