Feature: Client A - Add Transaction with immediate commit
  As a cashier using Client A POS
  I want transactions to be committed immediately after entry
  So that the sale is recorded without any extra steps

  Scenario: Add a transaction with notes and it is immediately completed
    Given a cashier enters item "Coffee" with quantity 2 and unit price 15000
    And the cashier adds notes "Extra sugar"
    When the cashier submits the transaction for Client A
    Then the transaction status should be "Completed"
    And the transaction total should be 30000
