namespace FinTrack.API.Entities
{
    public class MonobankTransaction
    {
        public string Id { get; set; }
        public int? Time { get; set; }
        public string Description { get; set; }
        public int? Mcc { get; set; }
        public int? OriginalMcc { get; set; }
        public object Hold { get; set; }
        public int? Amount { get; set; }
        public int? OperationAmount { get; set; }
        public int? CurrencyCode { get; set; }
        public object CommissionRate { get; set; }
        public int? CashbackAmount { get; set; }
        public int? Balance { get; set; }
        public string Comment { get; set; }
        public string ReceiptId { get; set; }
        public string InvoiceId { get; set; }
        public string CounterEdrpou { get; set; }
        public string CounterIban { get; set; }
        public string CounterName { get; set; }
    }
}
