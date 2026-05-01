Feature: Client B - Deferred transaction with manual approval
  As a cashier using Client B POS
  I want transactions to be held as pending until debit payment is confirmed
  So that payment is only finalized when the card is processed

  Scenario: Add a transaction with debit card and it is saved as pending
    Given a cashier enters item "Tea" with quantity 1 and unit price 10000
    And the cashier enters debit card "4111111111111111"
    When the cashier submits the transaction for Client B
    Then the transaction status should be "Pending"

  Scenario: Finalize one or more pending transactions
    Given the following pending transactions exist:
      | Id | ItemName | Quantity | UnitPrice | DebitCard        |
      | 1  | Tea      | 1        | 10000     | 4111111111111111 |
      | 2  | Juice    | 2        | 8000      | 4222222222222222 |
    When the cashier approves transaction ids "1,2"
    Then all approved transactions should have status "Completed"
