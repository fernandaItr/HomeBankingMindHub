using System;
using System.Linq;

namespace HomeBankingMindHub.Models
{
    public class DBInitializer
    {
        public static void Initialize(HomeBankingContext context)
        {
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

        }
    }
}
