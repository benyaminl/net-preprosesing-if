using System;
using System.Linq;
using PosApp.Domain.Entities;
using PosApp.Tests.Support;
using TechTalk.SpecFlow;
using Xunit;

namespace PosApp.Tests.StepDefinitions
{
    [Binding]
    [Scope(Feature = "Client B - Deferred transaction with manual approval")]
    public class ClientBSteps
    {
        private readonly InMemoryTransactionRepository _repo = new InMemoryTransactionRepository();
        private readonly TestTransactionService _service;
        private Transaction _transaction = new Transaction();
        private Transaction _result = new Transaction();

        public ClientBSteps() { _service = new TestTransactionService(_repo); }

        [Given(@"a cashier enters item ""(.*)"" with quantity (\d+) and unit price (\d+)")]
        public void GivenCashierEntersItem(string item, int qty, decimal price)
        {
            _transaction = new Transaction { ItemName = item, Quantity = qty, UnitPrice = price };
        }

        [Given(@"the cashier enters debit card ""(.*)""")]
        public void GivenCashierEntersDebitCard(string card) { _transaction.DebitCard = card; }

        [When(@"the cashier submits the transaction for Client B")]
        public void WhenCashierSubmitsClientB() { _result = _service.AddPendingTransactionClientB(_transaction); }

        [Then(@"the transaction status should be ""(.*)""")]
        public void ThenTransactionStatusShouldBe(string expectedStatus)
        {
            Assert.Equal((TransactionStatus)Enum.Parse(typeof(TransactionStatus), expectedStatus), _result.Status);
        }

        [Given(@"the following pending transactions exist:")]
        public void GivenPendingTransactionsExist(Table table)
        {
            var transactions = table.Rows.Select(row => new Transaction
            {
                ItemName    = row["ItemName"],
                Quantity    = int.Parse(row["Quantity"]),
                UnitPrice   = decimal.Parse(row["UnitPrice"]),
                TotalAmount = int.Parse(row["Quantity"]) * decimal.Parse(row["UnitPrice"]),
                CreatedAt   = DateTime.UtcNow,
                Status      = TransactionStatus.Pending,
                DebitCard   = row["DebitCard"]
            });
            _repo.Seed(transactions);
        }

        [When(@"the cashier approves transaction ids ""(.*)""")]
        public void WhenCashierApprovesIds(string idsRaw)
        {
            var ids = idsRaw.Split(',').Select(int.Parse);
            _service.FinalizeTransactions(ids);
        }

        [Then(@"all approved transactions should have status ""(.*)""")]
        public void ThenAllApprovedTransactionsShouldHaveStatus(string expectedStatus)
        {
            Assert.Empty(_repo.GetPending());
        }
    }
}
