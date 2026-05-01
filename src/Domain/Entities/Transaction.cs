using System;

namespace PosApp.Domain.Entities
{
    public enum TransactionStatus { Pending, Completed }

    public class Transaction
    {
        public int Id { get; set; }
        public string ItemName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime CreatedAt { get; set; }
        public TransactionStatus Status { get; set; }
        public string Notes { get; set; }
        public string DebitCard { get; set; }
    }
}
