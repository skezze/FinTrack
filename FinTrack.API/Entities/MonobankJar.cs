namespace FinTrack.API.Entities
{
    public class MonobankJar
    {
        public string Id { get; set; }
        public string SendId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int? CurrencyCode { get; set; }
        public int? Balance { get; set; }
        public int? Goal { get; set; }
    }
}
