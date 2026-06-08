using System;
using System.Linq;
using System.Threading.Tasks;
using Controllers;
using csharp_sprint1_stories.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace csharp_sprint1_stories.Tests.Controllers;

[TestFixture]
public class AccountsControllerTests
{
    private CsharpSprint1StoriesContext _context = null!;
    private AccountsController _controller = null!;
    private Customer _customer = null!;
    private Account _checkingAccount = null!;
    private Account _savingsParentAccount = null!;
    private SavingsAccount _savingsAccount = null!;

    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<CsharpSprint1StoriesContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new CsharpSprint1StoriesContext(options);

        _customer = new Customer
        {
            CustomerId = 2000000,
            Name = "Alice",
            Address = "1 Main St",
            PhoneNumber = "0400000000",
            Email = "alice@example.com",
            CustomerType = "Person",
            CreatedAt = DateTime.Now,
            IsActive = true
        };

        _checkingAccount = new Account
        {
            AccountId = 1000,
            CustomerId = _customer.CustomerId,
            Customer = _customer,
            AccountType = "Checking",
            Balance = 100m,
            CreatedAt = DateTime.Now,
            IsActive = true
        };

        _savingsParentAccount = new Account
        {
            AccountId = 1005,
            CustomerId = _customer.CustomerId,
            Customer = _customer,
            AccountType = "Savings",
            Balance = 200m,
            CreatedAt = DateTime.Now,
            IsActive = true
        };

        _savingsAccount = new SavingsAccount
        {
            AccountId = _savingsParentAccount.AccountId,
            Account = _savingsParentAccount,
            InterestRate = 5m,
            MinimumBalance = 0m
        };

        _savingsParentAccount.SavingsAccount = _savingsAccount;

        _context.Customers.Add(_customer);
        _context.Accounts.Add(_checkingAccount);
        _context.Accounts.Add(_savingsParentAccount);
        _context.SavingsAccounts.Add(_savingsAccount);
        _context.SaveChanges();

        _controller = new AccountsController(_context);
    }

    [TearDown]
    public void TearDown()
    {
        _controller.Dispose();
        _context.Dispose();
    }

    [Test]
    public async Task Deposit_WithPositiveAmount_RedirectsAndUpdatesBalance()
    {
        var result = await _controller.Deposit(_checkingAccount.AccountId, 25m);

        Assert.That(result, Is.TypeOf<RedirectToActionResult>());

        var updatedAccount = await _context.Accounts.FindAsync(_checkingAccount.AccountId);
        Assert.That(updatedAccount!.Balance, Is.EqualTo(125m));
    }

    [Test]
    public async Task Create_WithValidCheckingAccount_CreatesAccountAndCheckingAccount()
    {
        var result = await _controller.Create(_customer.CustomerId, "Checking", 300m);

        Assert.That(result, Is.TypeOf<RedirectToActionResult>());
        Assert.That(_context.Accounts.Count(), Is.EqualTo(3));
        Assert.That(_context.CheckingAccounts.Count(), Is.EqualTo(1));

        var createdAccount = await _context.Accounts.FindAsync(1010L);
        Assert.That(createdAccount, Is.Not.Null);
        Assert.That(createdAccount!.AccountType, Is.EqualTo("Checking"));
        Assert.That(createdAccount.Balance, Is.EqualTo(300m));

        var checkingChild = await _context.CheckingAccounts.FindAsync(1010L);
        Assert.That(checkingChild, Is.Not.Null);
        Assert.That(checkingChild!.NextCheckNumber, Is.EqualTo(1));
    }

    [Test]
    public async Task Create_WithValidSavingsAccount_CreatesAccountAndSavingsAccount()
    {
        var result = await _controller.Create(_customer.CustomerId, "Savings", 400m);

        Assert.That(result, Is.TypeOf<RedirectToActionResult>());
        Assert.That(_context.Accounts.Count(), Is.EqualTo(3));
        Assert.That(_context.SavingsAccounts.Count(), Is.EqualTo(2));

        var createdAccount = await _context.Accounts.FindAsync(1010L);
        Assert.That(createdAccount, Is.Not.Null);
        Assert.That(createdAccount!.AccountType, Is.EqualTo("Savings"));
        Assert.That(createdAccount.Balance, Is.EqualTo(400m));

        var savingsChild = await _context.SavingsAccounts.FindAsync(1010L);
        Assert.That(savingsChild, Is.Not.Null);
        Assert.That(savingsChild!.InterestRate, Is.EqualTo(0m));
        Assert.That(savingsChild.MinimumBalance, Is.EqualTo(0m));
    }

    [Test]
    public async Task Create_WithInvalidAccountType_ReturnsViewAndDoesNotCreateAccount()
    {
        var result = await _controller.Create(_customer.CustomerId, "TermDeposit", 300m);

        Assert.That(result, Is.TypeOf<ViewResult>());
        Assert.That(_context.Accounts.Count(), Is.EqualTo(2));
        Assert.That(_controller.ModelState.IsValid, Is.False);
    }

    [Test]
    public async Task Create_WithMissingCustomer_ReturnsNotFoundAndDoesNotCreateAccount()
    {
        var result = await _controller.Create(999999, "Checking", 300m);

        Assert.That(result, Is.TypeOf<NotFoundResult>());
        Assert.That(_context.Accounts.Count(), Is.EqualTo(2));
    }

    [Test]
    public async Task Deposit_WithZeroAmount_ReturnsViewAndDoesNotChangeBalance()
    {
        var result = await _controller.Deposit(_checkingAccount.AccountId, 0m);

        Assert.That(result, Is.TypeOf<ViewResult>());

        var updatedAccount = await _context.Accounts.FindAsync(_checkingAccount.AccountId);
        Assert.That(updatedAccount!.Balance, Is.EqualTo(100m));
        Assert.That(_controller.ModelState.IsValid, Is.False);
    }

    [Test]
    public async Task Withdraw_ForCheckingAccount_RedirectsAndDecreasesBalance()
    {
        var result = await _controller.Withdraw(_checkingAccount.AccountId, 40m);

        Assert.That(result, Is.TypeOf<RedirectToActionResult>());

        var updatedAccount = await _context.Accounts.FindAsync(_checkingAccount.AccountId);
        Assert.That(updatedAccount!.Balance, Is.EqualTo(60m));
    }

    [Test]
    public async Task Withdraw_ForSavingsAccount_WhenAmountExceedsBalance_KeepsBalanceUnchanged()
    {
        var result = await _controller.Withdraw(_savingsParentAccount.AccountId, 250m);

        Assert.That(result, Is.TypeOf<RedirectToActionResult>());

        var updatedAccount = await _context.Accounts.FindAsync(_savingsParentAccount.AccountId);
        Assert.That(updatedAccount!.Balance, Is.EqualTo(200m));
    }

    [Test]
    public async Task Withdraw_WithNegativeAmount_ReturnsViewAndDoesNotChangeBalance()
    {
        var result = await _controller.Withdraw(_checkingAccount.AccountId, -10m);

        Assert.That(result, Is.TypeOf<ViewResult>());

        var updatedAccount = await _context.Accounts.FindAsync(_checkingAccount.AccountId);
        Assert.That(updatedAccount!.Balance, Is.EqualTo(100m));
        Assert.That(_controller.ModelState.IsValid, Is.False);
    }

    [Test]
    public async Task CorrectBalance_ForCheckingAccount_WithNegativeAmount_RedirectsAndUpdatesBalance()
    {
        var result = await _controller.CorrectBalance(_checkingAccount.AccountId, -25m);

        Assert.That(result, Is.TypeOf<RedirectToActionResult>());

        var updatedAccount = await _context.Accounts.FindAsync(_checkingAccount.AccountId);
        Assert.That(updatedAccount!.Balance, Is.EqualTo(-25m));
    }

    [Test]
    public async Task CorrectBalance_ForSavingsAccount_WithNegativeAmount_ReturnsViewAndDoesNotChangeBalance()
    {
        var result = await _controller.CorrectBalance(_savingsParentAccount.AccountId, -25m);

        Assert.That(result, Is.TypeOf<ViewResult>());

        var updatedAccount = await _context.Accounts.FindAsync(_savingsParentAccount.AccountId);
        Assert.That(updatedAccount!.Balance, Is.EqualTo(200m));
        Assert.That(_controller.ModelState.IsValid, Is.False);
    }

    [Test]
    public async Task CorrectBalance_WithMissingAccount_ReturnsNotFound()
    {
        var result = await _controller.CorrectBalance(999999, 50m);

        Assert.That(result, Is.TypeOf<NotFoundResult>());
    }

    [Test]
    public async Task Edit_WithValidAccount_UpdatesAllowedFields()
    {
        var formAccount = new Account
        {
            AccountId = _checkingAccount.AccountId,
            CustomerId = _customer.CustomerId,
            AccountType = "Savings",
            Balance = 180m,
            CreatedAt = _checkingAccount.CreatedAt,
            IsActive = false
        };

        var result = await _controller.Edit(_checkingAccount.AccountId, formAccount);

        Assert.That(result, Is.TypeOf<RedirectToActionResult>());

        var updated = await _context.Accounts.FindAsync(_checkingAccount.AccountId);
        Assert.That(updated, Is.Not.Null);
        Assert.That(updated!.Balance, Is.EqualTo(180m));
        Assert.That(updated.IsActive, Is.False);
        Assert.That(updated.AccountType, Is.EqualTo("Checking"));
        Assert.That(updated.UpdatedAt, Is.Not.Null);
    }

    [Test]
    public async Task Edit_WhenRouteIdDoesNotMatchFormId_ReturnsNotFound()
    {
        var formAccount = new Account
        {
            AccountId = _checkingAccount.AccountId,
            CustomerId = _customer.CustomerId,
            AccountType = "Checking",
            Balance = 180m,
            CreatedAt = _checkingAccount.CreatedAt,
            IsActive = false
        };

        var result = await _controller.Edit(999999, formAccount);

        Assert.That(result, Is.TypeOf<NotFoundResult>());
    }

    [Test]
    public async Task Edit_WhenAccountDoesNotExist_ReturnsNotFound()
    {
        var formAccount = new Account
        {
            AccountId = 999999,
            CustomerId = _customer.CustomerId,
            AccountType = "Checking",
            Balance = 180m,
            CreatedAt = DateTime.Now,
            IsActive = false
        };

        var result = await _controller.Edit(999999, formAccount);

        Assert.That(result, Is.TypeOf<NotFoundResult>());
    }
}
