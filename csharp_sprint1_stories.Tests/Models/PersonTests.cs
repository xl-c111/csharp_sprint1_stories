using System;
using csharp_sprint1_stories.Models;
using NUnit.Framework;

namespace csharp_sprint1_stories.Tests.Models;

[TestFixture]
public class PersonTests
{
    private Customer _customer = null!;
    private Person _person = null!;

    [SetUp]
    public void SetUp()
    {
        _customer = new Customer
        {
            CustomerId = 2000000,
            Name = "Alice",
            Address = "1 Main St",
            PhoneNumber = "0400000000",
            Email = "alice@example.com",
            CustomerType = "Person"
        };

        _person = new Person
        {
            CustomerId = _customer.CustomerId,
            Customer = _customer,
            DateOfBirth = new DateTime(1990, 1, 1),
            Occupation = "Engineer"
        };
    }

    [Test]
    public void ChargeAllAccounts_WithPositiveAmount_ChargesCheckingAndSavingsAccounts()
    {
        var checkingAccount = new Account
        {
            AccountId = 1000,
            CustomerId = _customer.CustomerId,
            Customer = _customer,
            AccountType = "Checking",
            Balance = 100m
        };

        var savingsParentAccount = new Account
        {
            AccountId = 1005,
            CustomerId = _customer.CustomerId,
            Customer = _customer,
            AccountType = "Savings",
            Balance = 200m
        };

        var savingsAccount = new SavingsAccount
        {
            AccountId = savingsParentAccount.AccountId,
            Account = savingsParentAccount,
            InterestRate = 5m,
            MinimumBalance = 0m
        };

        savingsParentAccount.SavingsAccount = savingsAccount;

        _customer.Accounts.Add(checkingAccount);
        _customer.Accounts.Add(savingsParentAccount);

        _person.ChargeAllAccounts(25m);

        Assert.That(checkingAccount.Balance, Is.EqualTo(75m));
        Assert.That(savingsParentAccount.Balance, Is.EqualTo(175m));
    }

    [Test]
    public void ChargeAllAccounts_WithZeroAmount_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => _person.ChargeAllAccounts(0m));
    }

    [Test]
    public void ChargeAllAccounts_WithNegativeAmount_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => _person.ChargeAllAccounts(-10m));
    }

    [Test]
    public void ChargeAllAccounts_WhenCustomerHasNoAccounts_DoesNothing()
    {
        Assert.DoesNotThrow(() => _person.ChargeAllAccounts(10m));
        Assert.That(_customer.Accounts.Count, Is.EqualTo(0));
    }

    [Test]
    public void ChargeAllAccounts_WhenSavingsBalanceIsTooLow_DoesNotChangeSavingsBalance()
    {
        var savingsParentAccount = new Account
        {
            AccountId = 1010,
            CustomerId = _customer.CustomerId,
            Customer = _customer,
            AccountType = "Savings",
            Balance = 20m
        };

        var savingsAccount = new SavingsAccount
        {
            AccountId = savingsParentAccount.AccountId,
            Account = savingsParentAccount,
            InterestRate = 5m,
            MinimumBalance = 0m
        };

        savingsParentAccount.SavingsAccount = savingsAccount;
        _customer.Accounts.Add(savingsParentAccount);

        _person.ChargeAllAccounts(25m);

        Assert.That(savingsParentAccount.Balance, Is.EqualTo(20m));
    }
}
