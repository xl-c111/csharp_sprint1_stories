# CSharp Sprint 1 Stories

ASP.NET Core MVC banking application built with a **Database First** approach using **Entity Framework Core** and **MySQL**.

This project implements Sprint 1 customer and account management stories, including customer/account creation, balance operations, account-type-specific behavior, and related MVC flows.

## Overview

The application models a simple banking system with:

- `Customer` records with `Person` and `Company` sub-records
- `Account` records with `SavingsAccount` and `CheckingAccount` sub-records
- MVC flows for creating, viewing, editing, and deleting records
- Business logic for:
  - deposit
  - withdraw
  - correct balance
  - add interest
  - get next check number
  - charge all customer accounts

The project is implemented in a practical **C# MVC + DB-first** style rather than a pure in-memory OOP/UML design.

## Tech Stack

- ASP.NET Core MVC
- Entity Framework Core
- MySQL
- Razor Views
- C#

## Project Structure

```text
Controllers/     MVC controllers and user flows
Models/          EF Core database-first entities plus business methods
Views/           Razor views
sql/schema.sql   Database schema
docs/            Project notes and implementation planning
Program.cs       App startup and DI configuration
```

## Current Business Logic

### Customer

- Create customer as `Person` or `Company`
- Auto-generate `CustomerId`
  - starts at `2000000`
  - increments by `7`
- Edit core customer details
- Delete a customer together with related subtype and account records
- Charge all accounts for a customer
  - `Person`: charges the same amount to all accounts
  - `Company`: charges normal amount to checking accounts and double amount to savings accounts

### Account

- Create account as `Checking` or `Savings`
- Auto-generate `AccountId`
  - starts at `1000`
  - increments by `5`
- Deposit funds
- Withdraw funds
  - base account logic allows overdrawing
  - savings accounts cannot overdraw
- Correct balance
  - `Savings` accounts cannot be corrected to a negative balance
  - `Checking` accounts can still be corrected directly

### Savings Account

- Edit `InterestRate` and `MinimumBalance`
- Add interest through MVC flow

### Checking Account

- Edit `NextCheckNumber` and `OverdraftLimit`
- Issue next check number through MVC flow

## MVC Flows Currently Available

- Customer
  - Create
  - Edit
  - Delete
  - Charge All Accounts

- Account
  - Create
  - Edit
  - Delete
  - Deposit
  - Withdraw
  - Correct Balance

- SavingsAccount
  - Edit
  - Add Interest

- CheckingAccount
  - Edit
  - Get Next Check Number

## Database Setup

The schema is defined in:

- [sql/schema.sql](sql/schema.sql)

Before running the application:

1. Create the MySQL database using `sql/schema.sql`
2. Set the connection string in `appsettings.json`
3. Use your own local MySQL username and password

Example:

```json
"ConnectionStrings": {
  "CsharpSprint1StoriesContext": "server=localhost;port=3306;database=csharp_sprint1_stories;user=YOUR_USER;password=YOUR_PASSWORD;"
}
```

## Run the Project

### Requirements

- .NET SDK compatible with `net10.0`
- MySQL server

### First-Time Setup

1. Install a compatible .NET SDK
2. Install and start MySQL locally
3. Run [sql/schema.sql](sql/schema.sql) to create the database and tables
4. Update `appsettings.json` with your own MySQL username and password
5. Run the project from the repository root

### Steps

1. Configure the database connection string in `appsettings.json`
2. Ensure the database schema has been created
3. Run the application

```bash
dotnet run
```

Then open the local URL shown in the terminal.

## Submission Notes

If submitting this project as a `.zip` file, include at least:

- the full project source
- [README.md](README.md)
- [sql/schema.sql](sql/schema.sql)

The person running the project will still need:

- a compatible .NET SDK installed locally
- a local MySQL server
- their own MySQL credentials configured in `appsettings.json`

This project does not require sharing your personal database password.

## Testing

This project includes an NUnit test project:

- `csharp_sprint1_stories.Tests`

### Test Stack

- NUnit
- NUnit3TestAdapter
- Microsoft.EntityFrameworkCore.InMemory

### Current Test Coverage

#### Model tests

- `Customer`
  - add/remove account behavior
  - null handling
  - duplicate add behavior
- `Person`
  - charge all accounts
  - invalid amount handling
  - savings insufficient-funds edge case
- `Company`
  - checking vs savings charging rules
  - invalid amount handling
  - overdraft and insufficient-funds edge cases
- `Account`
  - deposit
  - withdraw
  - correct balance
  - deactivate
- `SavingsAccount`
  - withdraw restrictions
  - add interest
- `CheckingAccount`
  - next check number behavior

#### Controller tests

- `AccountsController`
  - create
  - edit
  - deposit
  - withdraw
  - correct balance
- `CustomersController`
  - create
  - edit
  - charge all accounts
- `SavingsAccountsController`
  - edit
  - add interest
- `CheckingAccountsController`
  - edit
  - get next check number
- `PersonsController`
  - create
  - edit
  - delete
- `CompaniesController`
  - create
  - edit
  - delete

### Run Tests

From the project root:

```bash
dotnet test
```
