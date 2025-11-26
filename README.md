# Simple ATM

This is a simple web-based ATM application written in C#.

## How To Run

- Prerequisites:
  - .NET 10 SDK
  - Node.js 20+
  - A Container Runtime, maybe Docker Desktop. I'm using Rancher Desktop.
- Run Aspire
  - Open a terminal in the project root directory
  - Run `dotnet run --project .\SimpleATM.AppHost\`
  - Click the link in the terminal to open the Aspire dashboard in your browser
  - From the Aspire dashboard, you can browse the backend project and database
  - Use the api.http file in the backend/SimpleATM.Api folder to test the API endpoints

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

- [x] Deposits, withdrawals, and transfers need to be atomic operations
- [ ] Testability for all core features
- [x] Data persistence using an in-memory or lightweight database. Chose MySQL because it's easy to set up with Aspire.
- [x] Elegant error handling
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

- [x] GET /accounts -> Account[]
- [x] GET /accounts/{id} -> Account + Transaction[]
- [x] POST /accounts/{id}/deposit -> Account
  - Request body: { amount: decimal }
- [x] POST /accounts/{id}/withdrawal -> Account
  - Request body: { amount: decimal }
- [ ] POST /accounts/transfer -> Account from, Account to
  - Request body: { from_account_id: int, to_account_id: int, amount: decimal, description?: string }
  - Needs work. The response is funny and transferring negative money does not error properly.

## Decision Log

Usually these would be in a dedicated folder for Architecture Decision Records, but for this simple project we'll keep them here and abbreviate them (no status, for example).

- 2025-11-23: Do NOT Drive API and client implementation from an OpenAPI spec
  - Usually I would opt to start with an OpenAPI spec to ensure clear API contract between frontend and backend
  - In this case, it's more complication than it's worth given the simplicity of the API and the time constraints
- ~~2025-11-23: Use SQLite for data persistence~~
  - ~~SQLite is lightweight and easy to set up, making it ideal for this simple application~~
  - ~~It provides sufficient functionality to meet the data persistence requirements without the overhead of a full database server~~
- 2025-11-23: Use Angular for frontend
  - Considered Vue, but the time limitations and my familiarity with Angular made it the better choice
- 2025-11-25: Use MySQL for data persistence
  - Aspire makes it easy to set up a MySQL container
