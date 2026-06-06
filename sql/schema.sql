DROP DATABASE IF EXISTS csharp_sprint1_stories;
CREATE DATABASE csharp_sprint1_stories;
USE csharp_sprint1_stories;

CREATE TABLE Customers (
    CustomerId BIGINT PRIMARY KEY,
    Name VARCHAR(100) NOT NULL,
    Address VARCHAR(255) NOT NULL,
    PhoneNumber VARCHAR(30) NOT NULL,
    Email VARCHAR(100) NOT NULL,
    CustomerType VARCHAR(20) NOT NULL,
    CreatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt DATETIME NULL,
    IsActive BOOLEAN NOT NULL DEFAULT TRUE, 
    
    CONSTRAINT UQ_Customers_Email UNIQUE(Email),
    CONSTRAINT CHK_Customers_CustomerType
        CHECK (CustomerType IN('Person','Company'))
);

CREATE TABLE Persons(
    CustomerId BIGINT PRIMARY KEY,
    DateOfBirth DATE NOT NULL,
    Occupation VARCHAR(100) NULL,
    
    CONSTRAINT FK_Persons_Customers
        FOREIGN KEY (CustomerId) 
        REFERENCES Customers(CustomerId)
        ON DELETE CASCADE
);

CREATE TABLE Companies(
    CustomerId BIGINT PRIMARY KEY,
    ABN VARCHAR(20) NOT NULL,
    ACN VARCHAR(20) NOT NULL,
    Industry VARCHAR(100) NULL,
    ContactPersonName VARCHAR(100) NOT NULL,
    ContactPersonPhone VARCHAR(30) NOT NULL,
    ContactPersonEmail VARCHAR(100) NOT NULL,
    
    CONSTRAINT UQ_Companies_ABN UNIQUE(ABN),
    CONSTRAINT UQ_Companies_ACN UNIQUE(ACN),
    
    CONSTRAINT FK_Companies_Customers
        FOREIGN KEY (CustomerId) 
        REFERENCES Customers(CustomerId)
        ON DELETE CASCADE
);

CREATE TABLE Accounts(
    AccountId BIGINT PRIMARY KEY,
    CustomerId BIGINT NOT NULL, 
    AccountType VARCHAR(20) NOT NULL,
    Balance DECIMAL (18, 2) NOT NULL DEFAULT 0.00,
    CreatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt DATETIME NULL,
    IsActive BOOLEAN NOT NULL DEFAULT TRUE, 
    
     CONSTRAINT FK_Accounts_Customers
        FOREIGN KEY (CustomerId) 
        REFERENCES Customers(CustomerId)
        ON DELETE CASCADE,
        
    CONSTRAINT CHK_Accounts_AccountType
        CHECK (AccountType IN('Savings','Checking'))
);

CREATE TABLE SavingsAccounts(
    AccountId BIGINT PRIMARY KEY,
    InterestRate DECIMAL (5, 2) NOT NULL DEFAULT 0.00,
    MinimumBalance DECIMAL (18, 2) NOT NULL DEFAULT 0.00,
    
    CONSTRAINT FK_SavingsAccounts_Accounts
        FOREIGN KEY (AccountId)
        REFERENCES Accounts(AccountId)
        ON DELETE CASCADE,
	
    CONSTRAINT CHK_SavingsAccounts_InterestRate
        CHECK (InterestRate >= 0),
	CONSTRAINT CHK_SavingsAccounts_MinimumBalance
        CHECK (MinimumBalance >= 0)
);

CREATE TABLE CheckingAccounts(
    AccountId BIGINT PRIMARY KEY,
    NextCheckNumber INT NOT NULL DEFAULT 1,
    OverdraftLimit DECIMAL (18, 2) NOT NULL DEFAULT 0.00,
    
    CONSTRAINT FK_CheckingAccounts_Accounts
        FOREIGN KEY (AccountId)
        REFERENCES Accounts(AccountId)
        ON DELETE CASCADE,
	
    CONSTRAINT CHK_CheckingAccounts_NextCheckNumber
        CHECK (NextCheckNumber >= 1),
	CONSTRAINT CHK_CheckingAccounts_OverdraftLimit
        CHECK (OverdraftLimit >= 0)
);
