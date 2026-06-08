using System;
using System.Threading.Tasks;
using Controllers;
using csharp_sprint1_stories.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace csharp_sprint1_stories.Tests.Controllers;

[TestFixture]
public class CheckingAccountsControllerTests
{
    private CsharpSprint1StoriesContext _context = null!;
    private CheckingAccountsController _controller = null!;
    private Account _account = null!;
    private CheckingAccount _checkingAccount = null!;

    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<CsharpSprint1StoriesContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new CsharpSprint1StoriesContext(options);

        _account = new Account
        {
            AccountId = 1000,
            CustomerId = 2000000,
            AccountType = "Checking",
            Balance = 100m,
            CreatedAt = DateTime.Now,
            IsActive = true
        };

        _checkingAccount = new CheckingAccount
        {
            AccountId = _account.AccountId,
            Account = _account,
            NextCheckNumber = 1,
            OverdraftLimit = 50m
        };

        _account.CheckingAccount = _checkingAccount;

        _context.Accounts.Add(_account);
        _context.CheckingAccounts.Add(_checkingAccount);
        _context.SaveChanges();

        _controller = new CheckingAccountsController(_context);
        _controller.TempData = new TempDataDictionary(
            new DefaultHttpContext(),
            new FakeTempDataProvider());
    }

    [TearDown]
    public void TearDown()
    {
        _controller.Dispose();
        _context.Dispose();
    }

    [Test]
    public async Task Edit_WithValidValues_RedirectsAndUpdatesCheckingAccount()
    {
        var formCheckingAccount = new CheckingAccount
        {
            AccountId = _checkingAccount.AccountId,
            NextCheckNumber = 10,
            OverdraftLimit = 100m
        };

        var result = await _controller.Edit(_checkingAccount.AccountId, formCheckingAccount);

        Assert.That(result, Is.TypeOf<RedirectToActionResult>());

        var updated = await _context.CheckingAccounts.FindAsync(_checkingAccount.AccountId);
        Assert.That(updated!.NextCheckNumber, Is.EqualTo(10));
        Assert.That(updated.OverdraftLimit, Is.EqualTo(100m));
    }

    [Test]
    public async Task Edit_WhenRouteIdDoesNotMatchFormId_ReturnsNotFound()
    {
        var formCheckingAccount = new CheckingAccount
        {
            AccountId = _checkingAccount.AccountId,
            NextCheckNumber = 10,
            OverdraftLimit = 100m
        };

        var result = await _controller.Edit(999999, formCheckingAccount);

        Assert.That(result, Is.TypeOf<NotFoundResult>());
    }

    [Test]
    public async Task Edit_WhenCheckingAccountDoesNotExist_ReturnsNotFound()
    {
        var formCheckingAccount = new CheckingAccount
        {
            AccountId = 999999,
            NextCheckNumber = 10,
            OverdraftLimit = 100m
        };

        var result = await _controller.Edit(999999, formCheckingAccount);

        Assert.That(result, Is.TypeOf<NotFoundResult>());
    }

    [Test]
    public async Task GetNextCheckNumber_WithValidCheckingAccount_RedirectsAndIncrementsNumber()
    {
        var result = await _controller.GetNextCheckNumber(_checkingAccount.AccountId);

        Assert.That(result, Is.TypeOf<RedirectToActionResult>());

        var updated = await _context.CheckingAccounts.FindAsync(_checkingAccount.AccountId);
        Assert.That(updated!.NextCheckNumber, Is.EqualTo(2));
    }

    [Test]
    public async Task GetNextCheckNumber_WhenNextCheckNumberIsBelowOne_ResetsToOneThenIncrements()
    {
        _checkingAccount.NextCheckNumber = 0;
        await _context.SaveChangesAsync();

        await _controller.GetNextCheckNumber(_checkingAccount.AccountId);

        var updated = await _context.CheckingAccounts.FindAsync(_checkingAccount.AccountId);
        Assert.That(updated!.NextCheckNumber, Is.EqualTo(2));
        Assert.That(_controller.TempData["IssuedCheckNumber"], Is.EqualTo(1));
    }

    [Test]
    public async Task GetNextCheckNumber_UpdatesParentAccountUpdatedAt()
    {
        await _controller.GetNextCheckNumber(_checkingAccount.AccountId);

        var updatedAccount = await _context.Accounts.FindAsync(_account.AccountId);
        Assert.That(updatedAccount!.UpdatedAt, Is.Not.Null);
    }

    [Test]
    public async Task GetNextCheckNumber_WithValidCheckingAccount_SetsTempData()
    {
        await _controller.GetNextCheckNumber(_checkingAccount.AccountId);

        Assert.That(_controller.TempData["IssuedCheckNumber"], Is.EqualTo(1));
    }

    [Test]
    public async Task GetNextCheckNumber_WithMissingId_ReturnsNotFound()
    {
        var result = await _controller.GetNextCheckNumber(null);

        Assert.That(result, Is.TypeOf<NotFoundResult>());
    }

    [Test]
    public async Task GetNextCheckNumber_WhenCheckingAccountDoesNotExist_ReturnsNotFound()
    {
        var result = await _controller.GetNextCheckNumber(999999);

        Assert.That(result, Is.TypeOf<NotFoundResult>());
    }

    private sealed class FakeTempDataProvider : ITempDataProvider
    {
        public System.Collections.Generic.IDictionary<string, object> LoadTempData(HttpContext context)
        {
            return new System.Collections.Generic.Dictionary<string, object>();
        }

        public void SaveTempData(HttpContext context, System.Collections.Generic.IDictionary<string, object> values)
        {
        }
    }
}
