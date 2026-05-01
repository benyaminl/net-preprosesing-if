using System;
using System.Collections.Generic;
using System.Linq;
using PosApp.Domain.Entities;
using PosApp.Console.Services;

namespace PosApp.Console.Menus
{
    public class MainMenu
    {
        private readonly TransactionService _service;
        public MainMenu(TransactionService service) { _service = service; }

        public void Run()
        {
#if CLIENT_A
            const string header = "=== POS Client A ===";
#elif CLIENT_B
            const string header = "=== POS Client B ===";
#endif
            while (true)
            {
                System.Console.WriteLine();
                System.Console.WriteLine(header);
                System.Console.WriteLine("1. Add Transaction");
#if CLIENT_B
                System.Console.WriteLine("2. Process Pending Payments");
#endif
                System.Console.WriteLine("0. Exit");
                System.Console.Write("Choice: ");

                var choice = System.Console.ReadLine();
                choice = choice == null ? "" : choice.Trim();

                switch (choice)
                {
                    case "1": AddTransaction(); break;
#if CLIENT_B
                    case "2": ProcessPending(); break;
#endif
                    case "0": return;
                    default: System.Console.WriteLine("Invalid option."); break;
                }
            }
        }

        private void AddTransaction()
        {
            var t = new Transaction();
            System.Console.Write("Item name: ");
            t.ItemName = System.Console.ReadLine() ?? "";
            System.Console.Write("Quantity: ");
            int q; int.TryParse(System.Console.ReadLine(), out q); t.Quantity = q;
            System.Console.Write("Unit price: ");
            decimal p; decimal.TryParse(System.Console.ReadLine(), out p); t.UnitPrice = p;
#if CLIENT_A
            System.Console.Write("Notes: ");
            t.Notes = System.Console.ReadLine() ?? "";
            var result = _service.AddTransaction(t);
            System.Console.WriteLine("[OK] Transaction #" + result.Id + " committed. Total: " + result.TotalAmount.ToString("N0"));
#elif CLIENT_B
            System.Console.Write("Debit card number: ");
            t.DebitCard = System.Console.ReadLine() ?? "";
            var result = _service.AddPendingTransaction(t);
            System.Console.WriteLine("[PENDING] Transaction #" + result.Id + " saved. Total: " + result.TotalAmount.ToString("N0"));
#endif
        }

#if CLIENT_B
        private void ProcessPending()
        {
            var pending = _service.GetPending().ToList();
            if (pending.Count == 0) { System.Console.WriteLine("No pending transactions."); return; }
            System.Console.WriteLine("\nPending transactions:");
            foreach (var t in pending)
                System.Console.WriteLine("  [" + t.Id + "] " + t.ItemName + " x" + t.Quantity + " = " + t.TotalAmount.ToString("N0") + "  Card: " + t.DebitCard);
            System.Console.Write("Enter IDs to approve (comma-separated): ");
            var input = System.Console.ReadLine() ?? "";
            var ids = new List<int>();
            foreach (var s in input.Split(','))
            {
                int id;
                if (int.TryParse(s.Trim(), out id) && id > 0) ids.Add(id);
            }
            if (ids.Count == 0) { System.Console.WriteLine("No valid IDs entered."); return; }
            _service.FinalizeTransactions(ids);
            System.Console.WriteLine("[OK] Finalized " + ids.Count + " transaction(s).");
        }
#endif
    }
}
