namespace FinTrack.API.Entities
{
    public class MonobankTransaction
    {
        public string Id { get; set; }
        public Int64 Time { get; set; }
        public string Description { get; set; }
        public int Mcc { get; set; }
        public int OriginalMcc { get; set; }
        public bool Hold { get; set; }
        public Int64 Amount { get; set; }
        public Int64 OperationAmount { get; set; }
        public int CurrencyCode { get; set; }
        public Int64 CommissionRate { get; set; }
        public Int64 CashbackAmount { get; set; }
        public Int64 Balance { get; set; }
        public string? Comment { get; set; }
        public string? ReceiptId { get; set; }
        public string? InvoiceId { get; set; }
        public string? CounterEdrpou { get; set; }
        public string? CounterIban { get; set; }
        public string? CounterName { get; set; }
        public string? AccountId { get; set; }
    }
}
