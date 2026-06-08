using System;
using csharp_sprint1_stories.Models;
using NUnit.Framework;

namespace csharp_sprint1_stories.Tests.Models;

[TestFixture]
public class SavingsAccountTests
{
    private Account _account = null!;
    private SavingsAccount _savingsAccount = null!;

    [SetUp]
    public void SetUp()
    {
        _account = new Account
        {
            AccountId = 1005,
            CustomerId = 2000000,
            AccountType = "Savings",
            Balance = 200m,
            CreatedAt = new DateTime(2025, 1, 1),
            IsActive = true
        };

        _savingsAccount = new SavingsAccount
        {
            AccountId = _account.AccountId,
            Account = _account,
            InterestRate = 5m,
            MinimumBalance = 0m
        };

        _account.SavingsAccount = _savingsAccount;
    }

    [Test]
    public void Withdraw_WhenAmountIsWithinBalance_DecreasesBalance()
    {
        var withdrawn = _savingsAccount.Withdraw(50m);

        Assert.That(withdrawn, Is.EqualTo(50m));
        Assert.That(_account.Balance, Is.EqualTo(150m));
    }

    [Test]
    public void Withdraw_WhenAmountEqualsBalance_AllowsWithdrawal()
    {
        var withdrawn = _savingsAccount.Withdraw(200m);

        Assert.That(withdrawn, Is.EqualTo(200m));
        Assert.That(_account.Balance, Is.EqualTo(0m));
    }

    [Test]
    public void Withdraw_WhenAmountExceedsBalance_ReturnsZero()
    {
        var withdrawn = _savingsAccount.Withdraw(250m);

        Assert.That(withdrawn, Is.EqualTo(0m));
        Assert.That(_account.Balance, Is.EqualTo(200m));
    }

    [Test]
    public void Withdraw_WithZeroAmount_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => _savingsAccount.Withdraw(0m));
    }

    [Test]
    public void Withdraw_WithNegativeAmount_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => _savingsAccount.Withdraw(-10m));
    }

    [Test]
    public void AddInterest_WithPositiveRate_IncreasesBalanceByCalculatedInterest()
    {
        _savingsAccount.AddInterest();

        Assert.That(_account.Balance, Is.EqualTo(210m));
    }

    [Test]
    public void AddInterest_WithPositiveRate_UpdatesUpdatedAt()
    {
        _savingsAccount.AddInterest();

        Assert.That(_account.UpdatedAt, Is.Not.Null);
    }

    [Test]
    public void AddInterest_WithZeroRate_DoesNotChangeBalance()
    {
        _savingsAccount.InterestRate = 0m;

        _savingsAccount.AddInterest();

        Assert.That(_account.Balance, Is.EqualTo(200m));
    }
}
