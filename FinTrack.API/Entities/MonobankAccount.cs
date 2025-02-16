namespace FinTrack.API.Entities
{
    public class MonobankAccount
    {
        public string Id { get; set; }
        public string SendId { get; set; }
        public int? Balance { get; set; }
        public int? CreditLimit { get; set; }
        public string Type { get; set; }
        public int? CurrencyCode { get; set; }
        public string CashbackType { get; set; }
        public List<string> MaskedPan { get; set; }
        public string Iban { get; set; }
    }
}
