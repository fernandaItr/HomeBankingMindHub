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

            //Loans data
            if (!context.Loans.Any())
            {
                var loans = new Loan[]
                {
                    new Loan { Name = "Hipotecario", MaxAmount = 500000, Payments = "12,24,36,48,60"},
                    new Loan { Name = "Personal", MaxAmount = 100000, Payments = "6,12,24"},
                    new Loan { Name = "Automotriz", MaxAmount = 300000, Payments = "6,12,24,36"},
                };

                foreach (Loan loan in loans)
                {
                    context.Loans.Add(loan);
                }
                context.SaveChanges();

                //adding loan to clients
                var client1 = context.Clients.FirstOrDefault(client => client.Email == "vcoronado@gmail.com");
                var client2 = context.Clients.FirstOrDefault(client => client.Email == "marcosGG@gmail.com");
                var client3 = context.Clients.FirstOrDefault(client => client.Email == "veronuñez@gmail.com");

                //Client 1
                if (client1 != null)
                {
                    var loanH = context.Loans.FirstOrDefault(lh => lh.Name == "Hipotecario");
                    if(loanH != null)
                    {
                        var clientLoan1 = new ClientLoan
                        {
                            Amount = 400000,
                            ClientId = client1.Id,
                            LoanId = loanH.Id,
                            Payments = "60"
                        };
                        context.ClientLoans.Add(clientLoan1);
                    }

                    var loanP = context.Loans.FirstOrDefault(lp => lp.Name == "Personal");
                    if(loanP != null)
                    {
                        var clientLoan2 = new ClientLoan
                        {
                            Amount = 50000,
                            ClientId = client1.Id,
                            LoanId = loanP.Id,
                            Payments = "12"
                        };
                        context.ClientLoans.Add(clientLoan2);
                    }

                    var loanA = context.Loans.FirstOrDefault(la => la.Name == "Automotriz");
                    if(loanA != null)
                    {
                        var clientLoan3 = new ClientLoan
                        {
                            Amount = 100000,
                            ClientId = client1.Id,
                            LoanId = loanA.Id,
                            Payments = "24"
                        };
                        context.ClientLoans.Add(clientLoan3);
                    }

                    context.SaveChanges();
                }

                //Client 2

                if (client2 != null)
                {
                    var loanP = context.Loans.FirstOrDefault(lp => lp.Name == "Personal");
                    if (loanP != null)
                    {
                        var clientLoan2 = new ClientLoan
                        {
                            Amount = 90000,
                            ClientId = client2.Id,
                            LoanId = loanP.Id,
                            Payments = "12"
                        };
                        context.ClientLoans.Add(clientLoan2);
                    }
                    context.SaveChanges();
                }

                //Client 3

                if (client3 != null)
                {
                    var loanH = context.Loans.FirstOrDefault(lh => lh.Name == "Hipotecario");
                    if (loanH != null)
                    {
                        var clientLoan3 = new ClientLoan
                        {
                            Amount = 280000,
                            ClientId = client3.Id,
                            LoanId = loanH.Id,
                            Payments = "48"
                        };
                        context.ClientLoans.Add(clientLoan3);
                    }
                    var loanA = context.Loans.FirstOrDefault(la => la.Name == "Automotriz");
                    if (loanA != null)
                    {
                        var clientLoan3 = new ClientLoan
                        {
                            Amount = 250000,
                            ClientId = client3.Id,
                            LoanId = loanA.Id,
                            Payments = "24"
                        };
                        context.ClientLoans.Add(clientLoan3);
                    }
                    context.SaveChanges();
                }

            }
        }
    }
}
