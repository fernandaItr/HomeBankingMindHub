using HomeBankingMindHub.Models;

namespace HomeBankingMindHub.Dtos
{
    public class TransactionDTO
    {
        public long Id { get; set; }
        public TransactionType Type { get; set; }
        public double Amount { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
    }
}
