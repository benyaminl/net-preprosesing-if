using Microsoft.Extensions.Configuration;
using PosApp.Console.Infrastructure;
using PosApp.Console.Menus;
using PosApp.Console.Services;

namespace PosApp.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false)
#if CLIENT_A
                .AddJsonFile("appsettings.ClientA.json", optional: true)
#elif CLIENT_B
                .AddJsonFile("appsettings.ClientB.json", optional: true)
#endif
                .Build();

            var dbPath  = config["Database:Path"] ?? "pos.db";
            var repo    = new SqliteTransactionRepository(dbPath);
            var service = new TransactionService(repo);
            var menu    = new MainMenu(service);
            menu.Run();
        }
    }
}
