using System;
using csharp_sprint1_stories.Models;
using NUnit.Framework;

namespace csharp_sprint1_stories.Tests.Models;

[TestFixture]
public class CustomerTests
{
    private Customer _customer = null!;

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
    }

    [Test]
    public void AddAccount_WithValidAccount_AddsAccountToCustomerCollection()
    {
        var account = new Account
        {
            AccountId = 1000,
            AccountType = "Checking",
            Balance = 100m
        };

        _customer.AddAccount(account);

        Assert.That(_customer.Accounts.Count, Is.EqualTo(1));
        Assert.That(_customer.Accounts.Contains(account), Is.True);
    }

    [Test]
    public void AddAccount_WithValidAccount_SetsAccountCustomerAndCustomerId()
    {
        var account = new Account
        {
            AccountId = 1005,
            AccountType = "Savings",
            Balance = 250m
        };

        _customer.AddAccount(account);

        Assert.That(account.Customer, Is.EqualTo(_customer));
        Assert.That(account.CustomerId, Is.EqualTo(_customer.CustomerId));
    }

    [Test]
    public void AddAccount_WithNullAccount_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => _customer.AddAccount(null!));
    }

    [Test]
    public void RemoveAccount_WithExistingAccount_RemovesAccountFromCustomerCollection()
    {
        var account = new Account
        {
            AccountId = 1010,
            CustomerId = _customer.CustomerId,
            Customer = _customer,
            AccountType = "Checking",
            Balance = 500m
        };

        _customer.Accounts.Add(account);

        _customer.RemoveAccount(account);

        Assert.That(_customer.Accounts.Contains(account), Is.False);
        Assert.That(_customer.Accounts.Count, Is.EqualTo(0));
    }

    [Test]
    public void RemoveAccount_WithNullAccount_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => _customer.RemoveAccount(null!));
    }

    [Test]
    public void RemoveAccount_WhenAccountIsNotInCollection_DoesNothing()
    {
        var account = new Account
        {
            AccountId = 1015,
            AccountType = "Checking",
            Balance = 100m
        };

        Assert.DoesNotThrow(() => _customer.RemoveAccount(account));
        Assert.That(_customer.Accounts.Count, Is.EqualTo(0));
    }

    [Test]
    public void AddAccount_WhenSameAccountAddedTwice_AddsTwoEntries()
    {
        var account = new Account
        {
            AccountId = 1020,
            AccountType = "Savings",
            Balance = 300m
        };

        _customer.AddAccount(account);
        _customer.AddAccount(account);

        Assert.That(_customer.Accounts.Count, Is.EqualTo(2));
    }

    [Test]
    public void ChargeAllAccounts_OnBaseCustomer_ThrowsNotImplementedException()
    {
        Assert.Throws<NotImplementedException>(() => _customer.ChargeAllAccounts(10m));
    }
}
