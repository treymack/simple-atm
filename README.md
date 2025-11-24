# Simple ATM

This is a simple web-based ATM application written in C# and Angular.

## How To Run

TBD

## Features

### Functional Requirements

- [x] Two types of accounts
  - Checking Account
  - Savings Account
- [x] List accounts with balance
- [x] Deposit funds to an account
- [x] Withdraw funds from an account
- [x] Transfer funds between two accounts
- [x] Track account balances and transaction history
  - [x] Transaction history with timestamps and descriptions

---

Out of Scope

- Authentication (assume a single user application)
- Model accounts separately (checking vs savings)
  - In a real world application there would be many differences between account types, but we'll keep it simple here and use a discriminator field on a single account model

### Non-Functional Requirements

- [ ] Deposits, withdrawals, and transfers need to be atomic operations
- [ ] Testability for all core features
- [ ] Data persistence using an in-memory or lightweight database (e.g., SQLite)
- [ ] Elegant error handling
  - Use a `Result<T>` pattern for operations that can fail

## Core Entities

- Account
  - id -- we'll let this be `int` so test cases are simple
  - type -- `checking`, `savings`, or `atm` -- atm in the non-transfer case is a special account representing cash on hand
  - balance -- `decimal`
- Transaction
  - id -- `int`
  - from_account_id -- `int`
  - to_account_id -- `int`
  - type -- `deposit`, `withdrawal`, `transfer`
  - amount -- `decimal`
  - timestamp -- `datetime`
  - description -- `string`

## APIs

- GET /accounts -> Account[]
- POST /accounts/{id}/deposit -> Result<Account>
  - Request body: { amount: decimal }
- POST /accounts/{id}/withdraw -> Result<Account>
  - Request body: { amount: decimal }
- POST /accounts/transfer -> Result<(Account from, Account to)>
  - Request body: { from_account_id: int, to_account_id: int, amount: decimal, description?: string }
- GET /accounts/{id}/transactions -> Transaction[]
