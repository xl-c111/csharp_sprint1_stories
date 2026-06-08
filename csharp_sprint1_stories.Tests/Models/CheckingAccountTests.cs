using csharp_sprint1_stories.Models;
using NUnit.Framework;

namespace csharp_sprint1_stories.Tests.Models;

[TestFixture]
public class CheckingAccountTests
{
    private Account _account = null!;
    private CheckingAccount _checkingAccount = null!;

    [SetUp]
    public void SetUp()
    {
        _account = new Account
        {
            AccountId = 1010,
            CustomerId = 2000000,
            AccountType = "Checking",
            Balance = 100m,
            CreatedAt = new System.DateTime(2025, 1, 1),
            IsActive = true
        };

        _checkingAccount = new CheckingAccount
        {
            AccountId = _account.AccountId,
            Account = _account,
            NextCheckNumber = 1,
            OverdraftLimit = 0m
        };

        _account.CheckingAccount = _checkingAccount;
    }

    [Test]
    public void GetNextCheckNumber_WithValidStartingValue_ReturnsCurrentNumber()
    {
        var checkNumber = _checkingAccount.GetNextCheckNumber();

        Assert.That(checkNumber, Is.EqualTo(1));
    }

    [Test]
    public void GetNextCheckNumber_WithValidStartingValue_IncrementsNextCheckNumber()
    {
        _checkingAccount.GetNextCheckNumber();

        Assert.That(_checkingAccount.NextCheckNumber, Is.EqualTo(2));
    }

    [Test]
    public void GetNextCheckNumber_WhenNextCheckNumberIsBelowOne_ResetsToOne()
    {
        _checkingAccount.NextCheckNumber = 0;

        var checkNumber = _checkingAccount.GetNextCheckNumber();

        Assert.That(checkNumber, Is.EqualTo(1));
        Assert.That(_checkingAccount.NextCheckNumber, Is.EqualTo(2));
    }

    [Test]
    public void GetNextCheckNumber_UpdatesParentAccountUpdatedAt()
    {
        _checkingAccount.GetNextCheckNumber();

        Assert.That(_account.UpdatedAt, Is.Not.Null);
    }
}
