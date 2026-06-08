using System;
using csharp_sprint1_stories.Models;
using NUnit.Framework;

namespace csharp_sprint1_stories.Tests.Models;

[TestFixture]
public class AccountTests
{
    private Account _account = null!;

    [SetUp]
    public void SetUp()
    {
        _account = new Account
        {
            AccountId = 1000,
            CustomerId = 2000000,
            AccountType = "Checking",
            Balance = 100m,
            CreatedAt = new DateTime(2025, 1, 1),
            IsActive = true
        };
    }

    [Test]
    public void Deposit_WithPositiveAmount_IncreasesBalance()
    {
        _account.Deposit(25m);

        Assert.That(_account.Balance, Is.EqualTo(125m));
    }

    [Test]
    public void Deposit_WithPositiveAmount_UpdatesUpdatedAt()
    {
        _account.Deposit(25m);

        Assert.That(_account.UpdatedAt, Is.Not.Null);
    }

    [Test]
    public void Deposit_WithZeroAmount_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => _account.Deposit(0m));
    }

    [Test]
    public void Deposit_WithNegativeAmount_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => _account.Deposit(-10m));
    }

    [Test]
    public void Withdraw_WithPositiveAmount_DecreasesBalance()
    {
        var withdrawn = _account.Withdraw(30m);

        Assert.That(withdrawn, Is.EqualTo(30m));
        Assert.That(_account.Balance, Is.EqualTo(70m));
    }

    [Test]
    public void Withdraw_WhenAmountExceedsBalance_AllowsOverdraw()
    {
        var withdrawn = _account.Withdraw(150m);

        Assert.That(withdrawn, Is.EqualTo(150m));
        Assert.That(_account.Balance, Is.EqualTo(-50m));
    }

    [Test]
    public void Withdraw_WithZeroAmount_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => _account.Withdraw(0m));
    }

    [Test]
    public void Withdraw_WithNegativeAmount_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => _account.Withdraw(-10m));
    }

    [Test]
    public void CorrectBalance_ForChecking_AllowsZero()
    {
        _account.CorrectBalance(0m);

        Assert.That(_account.Balance, Is.EqualTo(0m));
    }

    [Test]
    public void CorrectBalance_ForChecking_AllowsNegativeBalance()
    {
        _account.CorrectBalance(-25m);

        Assert.That(_account.Balance, Is.EqualTo(-25m));
    }

    [Test]
    public void CorrectBalance_ForSavings_WithNegativeBalance_ThrowsArgumentException()
    {
        _account.AccountType = "Savings";

        Assert.Throws<ArgumentException>(() => _account.CorrectBalance(-25m));
    }

    [Test]
    public void Deactivate_SetsIsActiveToFalse()
    {
        _account.Deactivate();

        Assert.That(_account.IsActive, Is.False);
    }
}
