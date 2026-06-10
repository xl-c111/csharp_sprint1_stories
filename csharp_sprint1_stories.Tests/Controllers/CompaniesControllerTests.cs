using System;
using System.Threading.Tasks;
using Controllers;
using csharp_sprint1_stories.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace csharp_sprint1_stories.Tests.Controllers;

[TestFixture]
public class CompaniesControllerTests
{
    private CsharpSprint1StoriesContext _context = null!;
    private CompaniesController _controller = null!;
    private Customer _customer = null!;
    private Company _company = null!;
    private Account _account = null!;

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
            Name = "Acme Pty Ltd",
            Address = "1 Main St",
            PhoneNumber = "0400000000",
            Email = "acme@example.com",
            CustomerType = "Company",
            CreatedAt = DateTime.Now,
            IsActive = true
        };

        _company = new Company
        {
            CustomerId = _customer.CustomerId,
            Customer = _customer,
            Abn = "12345678901",
            Acn = "123456789",
            Industry = "Technology",
            ContactPersonName = "Bob",
            ContactPersonPhone = "0411111111",
            ContactPersonEmail = "bob@example.com"
        };

        _account = new Account
        {
            AccountId = 1000,
            CustomerId = _customer.CustomerId,
            Customer = _customer,
            AccountType = "Savings",
            Balance = 250m,
            CreatedAt = DateTime.Now,
            IsActive = true
        };

        _context.Customers.Add(_customer);
        _context.Companies.Add(_company);
        _context.Accounts.Add(_account);
        _context.SaveChanges();

        _controller = new CompaniesController(_context);
    }

    [TearDown]
    public void TearDown()
    {
        _controller.Dispose();
        _context.Dispose();
    }

    [Test]
    public async Task Create_WithValidCompany_RedirectsAndCreatesRecord()
    {
        var customer = new Customer
        {
            CustomerId = 2000007,
            Name = "Beta Pty Ltd",
            Address = "2 Main St",
            PhoneNumber = "0422222222",
            Email = "beta@example.com",
            CustomerType = "Company",
            CreatedAt = DateTime.Now,
            IsActive = true
        };

        _context.Customers.Add(customer);
        await _context.SaveChangesAsync();

        var newCompany = new Company
        {
            CustomerId = customer.CustomerId,
            Abn = "10987654321",
            Acn = "987654321",
            Industry = "Finance",
            ContactPersonName = "Carol",
            ContactPersonPhone = "0433333333",
            ContactPersonEmail = "carol@example.com"
        };

        var result = await _controller.Create(newCompany);

        Assert.That(result, Is.TypeOf<RedirectToActionResult>());
        Assert.That(await _context.Companies.FindAsync(customer.CustomerId), Is.Not.Null);
    }

    [Test]
    public async Task Edit_WithValidCompany_RedirectsAndUpdatesRecord()
    {
        _context.Entry(_company).State = EntityState.Detached;

        var formCompany = new Company
        {
            CustomerId = _company.CustomerId,
            Abn = _company.Abn,
            Acn = _company.Acn,
            Industry = "Consulting",
            ContactPersonName = "David",
            ContactPersonPhone = "0444444444",
            ContactPersonEmail = "david@example.com"
        };

        var result = await _controller.Edit(_company.CustomerId, formCompany);

        Assert.That(result, Is.TypeOf<RedirectToActionResult>());

        var updated = await _context.Companies.FindAsync(_company.CustomerId);
        Assert.That(updated!.Industry, Is.EqualTo("Consulting"));
        Assert.That(updated.ContactPersonName, Is.EqualTo("David"));
        Assert.That(updated.ContactPersonPhone, Is.EqualTo("0444444444"));
        Assert.That(updated.ContactPersonEmail, Is.EqualTo("david@example.com"));
    }

    [Test]
    public async Task Edit_WhenRouteIdDoesNotMatchFormId_ReturnsNotFound()
    {
        var formCompany = new Company
        {
            CustomerId = _company.CustomerId,
            Abn = _company.Abn,
            Acn = _company.Acn,
            Industry = "Consulting",
            ContactPersonName = "David",
            ContactPersonPhone = "0444444444",
            ContactPersonEmail = "david@example.com"
        };

        var result = await _controller.Edit(999999, formCompany);

        Assert.That(result, Is.TypeOf<NotFoundResult>());
    }

    [Test]
    public async Task DeleteConfirmed_WithExistingCompany_RemovesCustomerAndRelatedAccountRecords()
    {
        var result = await _controller.DeleteConfirmed(_company.CustomerId);

        Assert.That(result, Is.TypeOf<RedirectToActionResult>());
        Assert.That(await _context.Customers.FindAsync(_company.CustomerId), Is.Null);
        Assert.That(await _context.Companies.FindAsync(_company.CustomerId), Is.Null);
        Assert.That(await _context.Accounts.FindAsync(_account.AccountId), Is.Null);
    }
}
