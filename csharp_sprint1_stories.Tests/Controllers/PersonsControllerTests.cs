using System;
using System.Threading.Tasks;
using Controllers;
using csharp_sprint1_stories.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace csharp_sprint1_stories.Tests.Controllers;

[TestFixture]
public class PersonsControllerTests
{
    private CsharpSprint1StoriesContext _context = null!;
    private PersonsController _controller = null!;
    private Customer _customer = null!;
    private Person _person = null!;
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
            Name = "Alice",
            Address = "1 Main St",
            PhoneNumber = "0400000000",
            Email = "alice@example.com",
            CustomerType = "Person",
            CreatedAt = DateTime.Now,
            IsActive = true
        };

        _person = new Person
        {
            CustomerId = _customer.CustomerId,
            Customer = _customer,
            DateOfBirth = new DateTime(1990, 1, 1),
            Occupation = "Engineer"
        };

        _account = new Account
        {
            AccountId = 1000,
            CustomerId = _customer.CustomerId,
            Customer = _customer,
            AccountType = "Checking",
            Balance = 100m,
            CreatedAt = DateTime.Now,
            IsActive = true
        };

        _context.Customers.Add(_customer);
        _context.Persons.Add(_person);
        _context.Accounts.Add(_account);
        _context.SaveChanges();

        _controller = new PersonsController(_context);
    }

    [TearDown]
    public void TearDown()
    {
        _controller.Dispose();
        _context.Dispose();
    }

    [Test]
    public async Task Create_WithValidPerson_RedirectsAndCreatesRecord()
    {
        var customer = new Customer
        {
            CustomerId = 2000007,
            Name = "Bob",
            Address = "2 Main St",
            PhoneNumber = "0411111111",
            Email = "bob@example.com",
            CustomerType = "Person",
            CreatedAt = DateTime.Now,
            IsActive = true
        };

        _context.Customers.Add(customer);
        await _context.SaveChangesAsync();

        var newPerson = new Person
        {
            CustomerId = customer.CustomerId,
            DateOfBirth = new DateTime(1985, 5, 5),
            Occupation = "Teacher"
        };

        var result = await _controller.Create(newPerson);

        Assert.That(result, Is.TypeOf<RedirectToActionResult>());
        Assert.That(await _context.Persons.FindAsync(customer.CustomerId), Is.Not.Null);
    }

    [Test]
    public async Task Edit_WithValidPerson_RedirectsAndUpdatesRecord()
    {
        _context.Entry(_person).State = EntityState.Detached;

        var formPerson = new Person
        {
            CustomerId = _person.CustomerId,
            DateOfBirth = new DateTime(1991, 2, 2),
            Occupation = "Architect"
        };

        var result = await _controller.Edit(_person.CustomerId, formPerson);

        Assert.That(result, Is.TypeOf<RedirectToActionResult>());

        var updated = await _context.Persons.FindAsync(_person.CustomerId);
        Assert.That(updated!.DateOfBirth, Is.EqualTo(new DateTime(1991, 2, 2)));
        Assert.That(updated.Occupation, Is.EqualTo("Architect"));
    }

    [Test]
    public async Task Edit_WhenRouteIdDoesNotMatchFormId_ReturnsNotFound()
    {
        var formPerson = new Person
        {
            CustomerId = _person.CustomerId,
            DateOfBirth = new DateTime(1991, 2, 2),
            Occupation = "Architect"
        };

        var result = await _controller.Edit(999999, formPerson);

        Assert.That(result, Is.TypeOf<NotFoundResult>());
    }

    [Test]
    public async Task DeleteConfirmed_WithExistingPerson_RemovesCustomerAndRelatedAccountRecords()
    {
        var result = await _controller.DeleteConfirmed(_person.CustomerId);

        Assert.That(result, Is.TypeOf<RedirectToActionResult>());
        Assert.That(await _context.Customers.FindAsync(_person.CustomerId), Is.Null);
        Assert.That(await _context.Persons.FindAsync(_person.CustomerId), Is.Null);
        Assert.That(await _context.Accounts.FindAsync(_account.AccountId), Is.Null);
    }
}
