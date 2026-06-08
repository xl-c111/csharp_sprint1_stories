using System;
using System.Threading.Tasks;
using Controllers;
using csharp_sprint1_stories.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace csharp_sprint1_stories.Tests.Controllers;

[TestFixture]
public class SavingsAccountsControllerTests
{
    private CsharpSprint1StoriesContext _context = null!;
    private SavingsAccountsController _controller = null!;
    private Account _account = null!;
    private SavingsAccount _savingsAccount = null!;

    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<CsharpSprint1StoriesContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new CsharpSprint1StoriesContext(options);

        _account = new Account
        {
            AccountId = 1005,
            CustomerId = 2000000,
            AccountType = "Savings",
            Balance = 200m,
            CreatedAt = DateTime.Now,
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

        _context.Accounts.Add(_account);
        _context.SavingsAccounts.Add(_savingsAccount);
        _context.SaveChanges();

        _controller = new SavingsAccountsController(_context);
    }

    [TearDown]
    public void TearDown()
    {
        _controller.Dispose();
        _context.Dispose();
    }

    [Test]
    public async Task Edit_WithValidValues_RedirectsAndUpdatesSavingsAccount()
    {
        var formSavingsAccount = new SavingsAccount
        {
            AccountId = _savingsAccount.AccountId,
            InterestRate = 7.5m,
            MinimumBalance = 50m
        };

        var result = await _controller.Edit(_savingsAccount.AccountId, formSavingsAccount);

        Assert.That(result, Is.TypeOf<RedirectToActionResult>());

        var updated = await _context.SavingsAccounts.FindAsync(_savingsAccount.AccountId);
        Assert.That(updated!.InterestRate, Is.EqualTo(7.5m));
        Assert.That(updated.MinimumBalance, Is.EqualTo(50m));
    }

    [Test]
    public async Task Edit_WhenRouteIdDoesNotMatchFormId_ReturnsNotFound()
    {
        var formSavingsAccount = new SavingsAccount
        {
            AccountId = _savingsAccount.AccountId,
            InterestRate = 7.5m,
            MinimumBalance = 50m
        };

        var result = await _controller.Edit(999999, formSavingsAccount);

        Assert.That(result, Is.TypeOf<NotFoundResult>());
    }

    [Test]
    public async Task Edit_WhenSavingsAccountDoesNotExist_ReturnsNotFound()
    {
        var formSavingsAccount = new SavingsAccount
        {
            AccountId = 999999,
            InterestRate = 7.5m,
            MinimumBalance = 50m
        };

        var result = await _controller.Edit(999999, formSavingsAccount);

        Assert.That(result, Is.TypeOf<NotFoundResult>());
    }

    [Test]
    public async Task AddInterest_WithValidSavingsAccount_RedirectsAndUpdatesBalance()
    {
        var result = await _controller.AddInterest(_savingsAccount.AccountId);

        Assert.That(result, Is.TypeOf<RedirectToActionResult>());

        var updatedAccount = await _context.Accounts.FindAsync(_account.AccountId);
        Assert.That(updatedAccount!.Balance, Is.EqualTo(210m));
        Assert.That(updatedAccount.UpdatedAt, Is.Not.Null);
    }

    [Test]
    public async Task AddInterest_WithZeroInterestRate_KeepsBalanceUnchanged()
    {
        _savingsAccount.InterestRate = 0m;
        await _context.SaveChangesAsync();

        var result = await _controller.AddInterest(_savingsAccount.AccountId);

        Assert.That(result, Is.TypeOf<RedirectToActionResult>());

        var updatedAccount = await _context.Accounts.FindAsync(_account.AccountId);
        Assert.That(updatedAccount!.Balance, Is.EqualTo(200m));
    }

    [Test]
    public async Task AddInterest_WithMissingId_ReturnsNotFound()
    {
        var result = await _controller.AddInterest(null);

        Assert.That(result, Is.TypeOf<NotFoundResult>());
    }

    [Test]
    public async Task AddInterest_WhenSavingsAccountDoesNotExist_ReturnsNotFound()
    {
        var result = await _controller.AddInterest(999999);

        Assert.That(result, Is.TypeOf<NotFoundResult>());
    }
}
