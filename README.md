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

### Steps

1. Configure the database connection string in `appsettings.json`
2. Ensure the database schema has been created
3. Run the application

```bash
dotnet run
```

Then open the local URL shown in the terminal.

## Notes

- This project uses scaffolded EF Core entities and MVC controllers as a base.
- Some business logic is implemented directly in model classes, while creation and workflow orchestration remain in controllers.
- Delete behavior mainly relies on EF Core and database cascade rules.
- `MinimumBalance` is stored for savings accounts but is not yet part of the withdrawal rule.

## Known Limitations

- ID generation is controller-based rather than domain/service-based
- Delete behavior is database-driven rather than wrapped in dedicated business methods
- `MinimumBalance` is not yet enforced during savings withdrawals

