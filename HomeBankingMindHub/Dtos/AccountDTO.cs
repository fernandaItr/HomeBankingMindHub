using HomeBankingMindHub.Models;
using System;

namespace HomeBankingMindHub.Dtos
{
    public class AccountDTO
    {
        public long Id { get; set; }
        public string Number { get; set; }
        public DateTime CreationDate { get; set; }
        public double Balance { get; set; }
        public ICollection<TransactionDTO> Transactions { get; set; }

        public AccountDTO() { }

        public AccountDTO(Account account)
        {
            Id = account.Id;
            Number = account.Number;
            CreationDate = DateTime.Now;
            Balance = account.Balance;
            //Transactions = account.Transactions.Select(transaction => new TransactionDTO(transaction)).ToList();
        }
    }
}
