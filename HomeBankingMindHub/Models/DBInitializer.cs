using System;
using System.Linq;

namespace HomeBankingMindHub.Models
{
    public class DBInitializer
    {
        public static void Initialize(HomeBankingContext context)
        {
            //Clients data
            if (!context.Clients.Any())
            {
                var clients = new Client[]
                {
                    new Client { Email = "vcoronado@gmail.com", FirstName="Victor", LastName="Coronado", Password="123456"},
                    new Client { Email = "marcosGG@gmail.com", FirstName="Marcos", LastName="Gonzalez", Password="678910"},
                    new Client { Email = "veronuñez@gmail.com", FirstName="Veronica", LastName="Nuñez", Password="345623"}
                };

                context.Clients.AddRange(clients);

                context.SaveChanges();
            }

            //Accounts data
            if (!context.Accounts.Any())
            {
                var accountVictor = context.Clients.FirstOrDefault(client => client.Email == "vcoronado@gmail.com");
                var accountMarcos = context.Clients.FirstOrDefault(client => client.Email == "marcosGG@gmail.com");
                var accountVeronica = context.Clients.FirstOrDefault(client => client.Email == "veronuñez@gmail.com");

                if (accountVictor != null && accountMarcos != null && accountVeronica != null)
                {
                    var accounts = new Account[]
                    {
                        new Account {ClientId = accountVictor.Id, CreationDate = DateTime.Now, Number= "VIN001", Balance = 0 },
                        new Account {ClientId = accountMarcos.Id, CreationDate = DateTime.Now, Number= "MAN002", Balance = 1 },
                        new Account {ClientId = accountVeronica.Id, CreationDate = DateTime.Now, Number= "VEN003", Balance = 2 },
                    };

                    foreach (Account account in accounts)
                    {
                        context.Accounts.Add(account);
                    }
                    context.SaveChanges();
                }
            }

            //Transaction data
            if(!context.Transactions.Any())
            {
                var account1 = context.Accounts.FirstOrDefault(c => c.Number == "VIN001");
                var account2 = context.Accounts.FirstOrDefault(c => c.Number == "MAN002");
                var account3 = context.Accounts.FirstOrDefault(c => c.Number == "VEN003");

                if (account1 != null && account2 != null && account3 != null)
                {
                    var transactions = new Transaction[]
                    {
                        new Transaction { AccountId = account1.Id, Amount = 1000, Date = DateTime.Now.AddHours(-5), Description = "Transferencia recibida", Type = TransactionType.CREDIT },
                        new Transaction { AccountId = account1.Id, Amount = -2000, Date = DateTime.Now.AddHours(-6), Description = "Compra en tienda mercado libre", Type = TransactionType.DEBIT },
                        new Transaction { AccountId = account1.Id, Amount = -3000, Date = DateTime.Now.AddHours(-7), Description = "Compra en tienda xxxx", Type = TransactionType.DEBIT },

                        new Transaction { AccountId = account2.Id, Amount = -2000, Date = DateTime.Now.AddHours(-4), Description = "Compra en pedidos ya", Type = TransactionType.CREDIT },
                        
                        new Transaction { AccountId = account3.Id, Amount = 1000, Date = DateTime.Now.AddHours(-8), Description = "Transferencia recibida", Type = TransactionType.DEBIT },
                        new Transaction { AccountId = account3.Id, Amount = 5000, Date = DateTime.Now.AddHours(-9), Description = "Transferencia recibida", Type = TransactionType.DEBIT },
                    };
                    
                    foreach( Transaction transaction in transactions)
                    {
                        context.Transactions.Add(transaction);
                    }

                    context.SaveChanges();
                }
            }
        }
    }
}
