using System;
using PosApp.Domain.Entities;
using PosApp.Tests.Support;
using TechTalk.SpecFlow;
using Xunit;

namespace PosApp.Tests.StepDefinitions
{
    [Binding]
    [Scope(Feature = "Client A - Add Transaction with immediate commit")]
    public class ClientASteps
    {
        private readonly InMemoryTransactionRepository _repo = new InMemoryTransactionRepository();
        private readonly TestTransactionService _service;
        private Transaction _transaction = new Transaction();
        private Transaction _result = new Transaction();

        public ClientASteps() { _service = new TestTransactionService(_repo); }

        [Given(@"a cashier enters item ""(.*)"" with quantity (\d+) and unit price (\d+)")]
        public void GivenCashierEntersItem(string item, int qty, decimal price)
        {
            _transaction = new Transaction { ItemName = item, Quantity = qty, UnitPrice = price };
        }

        [Given(@"the cashier adds notes ""(.*)""")]
        public void GivenCashierAddsNotes(string notes) { _transaction.Notes = notes; }

        [When(@"the cashier submits the transaction for Client A")]
        public void WhenCashierSubmitsClientA() { _result = _service.AddTransactionClientA(_transaction); }

        [Then(@"the transaction status should be ""(.*)""")]
        public void ThenTransactionStatusShouldBe(string expectedStatus)
        {
            Assert.Equal((TransactionStatus)Enum.Parse(typeof(TransactionStatus), expectedStatus), _result.Status);
        }

        [Then(@"the transaction total should be (\d+)")]
        public void ThenTransactionTotalShouldBe(decimal expectedTotal)
        {
            Assert.Equal(expectedTotal, _result.TotalAmount);
        }
    }
}
