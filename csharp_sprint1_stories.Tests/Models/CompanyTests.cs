using System;
using csharp_sprint1_stories.Models;
using NUnit.Framework;

namespace csharp_sprint1_stories.Tests.Models;

[TestFixture]
public class CompanyTests
{
    private Customer _customer = null!;
    private Company _company = null!;

    [SetUp]
    public void SetUp()
    {
        _customer = new Customer
        {
            CustomerId = 2000007,
            Name = "Acme Pty Ltd",
            Address = "2 Main St",
            PhoneNumber = "0411111111",
            Email = "acme@example.com",
            CustomerType = "Company"
        };

        _company = new Company
        {
            CustomerId = _customer.CustomerId,
            Customer = _customer,
            Abn = "12345678901",
            Acn = "123456789",
            Industry = "Technology",
            ContactPersonName = "Bob",
            ContactPersonPhone = "0422222222",
            ContactPersonEmail = "bob@example.com"
        };
    }

    [Test]
    public void ChargeAllAccounts_WithPositiveAmount_ChargesCheckingNormallyAndSavingsDouble()
    {
        var checkingAccount = new Account
        {
            AccountId = 1010,
            CustomerId = _customer.CustomerId,
            Customer = _customer,
            AccountType = "Checking",
            Balance = 100m
        };

        var savingsParentAccount = new Account
        {
            AccountId = 1015,
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

        _company.ChargeAllAccounts(25m);

        Assert.That(checkingAccount.Balance, Is.EqualTo(75m));
        Assert.That(savingsParentAccount.Balance, Is.EqualTo(150m));
    }

    [Test]
    public void ChargeAllAccounts_WithZeroAmount_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => _company.ChargeAllAccounts(0m));
    }

    [Test]
    public void ChargeAllAccounts_WithNegativeAmount_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => _company.ChargeAllAccounts(-10m));
    }

    [Test]
    public void ChargeAllAccounts_WhenCustomerHasNoAccounts_DoesNothing()
    {
        Assert.DoesNotThrow(() => _company.ChargeAllAccounts(10m));
        Assert.That(_customer.Accounts.Count, Is.EqualTo(0));
    }

    [Test]
    public void ChargeAllAccounts_WhenSavingsCannotCoverDoubleCharge_DoesNotChangeSavingsBalance()
    {
        var savingsParentAccount = new Account
        {
            AccountId = 1020,
            CustomerId = _customer.CustomerId,
            Customer = _customer,
            AccountType = "Savings",
            Balance = 40m
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

        _company.ChargeAllAccounts(25m);

        Assert.That(savingsParentAccount.Balance, Is.EqualTo(40m));
    }

    [Test]
    public void ChargeAllAccounts_WhenCheckingBalanceIsTooLow_StillOverdrawsCheckingAccount()
    {
        var checkingAccount = new Account
        {
            AccountId = 1025,
            CustomerId = _customer.CustomerId,
            Customer = _customer,
            AccountType = "Checking",
            Balance = 10m
        };

        _customer.Accounts.Add(checkingAccount);

        _company.ChargeAllAccounts(25m);

        Assert.That(checkingAccount.Balance, Is.EqualTo(-15m));
    }
}
