namespace Excel.Adapter.Models
{
    public class AccountReport
    {
        public string AccountName { get; set; }
        public decimal Balance { get; set; }
        public IList<ConsolidateAccount> ConsolidateAccount { get; set; } = new List<ConsolidateAccount>();
        public IList<ConsolidateRelease> ConsolidateRelease { get; set; } = new List<ConsolidateRelease>();
    }
    public class ConsolidateAccount
    {
        public decimal Balance { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public DateTime Date { get; set; }
    }

    public class ConsolidateRelease
    {
        public string Operation { get; set; }
        public string Description { get; set; }
        public decimal Value { get; set; }
        public DateTime Date { get; set; }
    }
}
