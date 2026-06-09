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
public class CustomersControllerTests
{
    private CsharpSprint1StoriesContext _context = null!;
    private CustomersController _controller = null!;

    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<CsharpSprint1StoriesContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new CsharpSprint1StoriesContext(options);
        _controller = new CustomersController(_context);
    }

    [TearDown]
    public void TearDown()
    {
        _controller.Dispose();
        _context.Dispose();
    }

    [Test]
    public async Task Create_WithValidPersonData_CreatesCustomerAndPerson()
    {
        var result = await _controller.Create(
            name: "Alice",
            address: "1 Main St",
            phoneNumber: "0400000000",
            email: "alice@example.com",
            customerType: "Person",
            dateOfBirth: new DateTime(1990, 1, 1),
            occupation: "Engineer",
            abn: null,
            acn: null,
            industry: null,
            contactPersonName: null,
            contactPersonPhone: null,
            contactPersonEmail: null
        );

        Assert.That(result, Is.TypeOf<RedirectToActionResult>());
        Assert.That(_context.Customers.Count(), Is.EqualTo(1));
        Assert.That(_context.Persons.Count(), Is.EqualTo(1));

        var customer = _context.Customers.Single();
        Assert.That(customer.CustomerType, Is.EqualTo("Person"));
        Assert.That(customer.CustomerId, Is.EqualTo(2000000));
    }

    [Test]
    public async Task Create_WithValidCompanyData_CreatesCustomerAndCompany()
    {
        var result = await _controller.Create(
            name: "Acme Pty Ltd",
            address: "2 Main St",
            phoneNumber: "0411111111",
            email: "acme@example.com",
            customerType: "Company",
            dateOfBirth: null,
            occupation: null,
            abn: "12345678901",
            acn: "123456789",
            industry: "Technology",
            contactPersonName: "Bob",
            contactPersonPhone: "0422222222",
            contactPersonEmail: "bob@example.com"
        );

        Assert.That(result, Is.TypeOf<RedirectToActionResult>());
        Assert.That(_context.Customers.Count(), Is.EqualTo(1));
        Assert.That(_context.Companies.Count(), Is.EqualTo(1));

        var customer = _context.Customers.Single();
        Assert.That(customer.CustomerType, Is.EqualTo("Company"));
        Assert.That(customer.CustomerId, Is.EqualTo(2000000));
    }

    [Test]
    public async Task Create_CompanyWithoutCustomerPhoneOrEmail_UsesContactDetails()
    {
        var result = await _controller.Create(
            name: "Acme Pty Ltd",
            address: "2 Main St",
            phoneNumber: "",
            email: "",
            customerType: "Company",
            dateOfBirth: null,
            occupation: null,
            abn: "12345678901",
            acn: "123456789",
            industry: "Technology",
            contactPersonName: "Bob",
            contactPersonPhone: "0422222222",
            contactPersonEmail: "bob@example.com"
        );

        Assert.That(result, Is.TypeOf<RedirectToActionResult>());

        var customer = _context.Customers.Single();
        Assert.That(customer.PhoneNumber, Is.EqualTo("0422222222"));
        Assert.That(customer.Email, Is.EqualTo("bob@example.com"));
    }

    [Test]
    public async Task Create_WithInvalidCustomerType_ReturnsViewAndDoesNotCreateCustomer()
    {
        var result = await _controller.Create(
            name: "Alice",
            address: "1 Main St",
            phoneNumber: "0400000000",
            email: "alice@example.com",
            customerType: "InvalidType",
            dateOfBirth: null,
            occupation: null,
            abn: null,
            acn: null,
            industry: null,
            contactPersonName: null,
            contactPersonPhone: null,
            contactPersonEmail: null
        );

        Assert.That(result, Is.TypeOf<ViewResult>());
        Assert.That(_context.Customers.Count(), Is.EqualTo(0));
        Assert.That(_controller.ModelState.IsValid, Is.False);
    }

    [Test]
    public async Task Create_PersonWithoutDateOfBirth_ReturnsViewAndDoesNotCreateCustomer()
    {
        var result = await _controller.Create(
            name: "Alice",
            address: "1 Main St",
            phoneNumber: "0400000000",
            email: "alice@example.com",
            customerType: "Person",
            dateOfBirth: null,
            occupation: "Engineer",
            abn: null,
            acn: null,
            industry: null,
            contactPersonName: null,
            contactPersonPhone: null,
            contactPersonEmail: null
        );

        Assert.That(result, Is.TypeOf<ViewResult>());
        Assert.That(_context.Customers.Count(), Is.EqualTo(0));
        Assert.That(_controller.ModelState.IsValid, Is.False);
    }

    [Test]
    public async Task Create_CompanyWithoutRequiredFields_ReturnsViewAndDoesNotCreateCustomer()
    {
        var result = await _controller.Create(
            name: "Acme Pty Ltd",
            address: "2 Main St",
            phoneNumber: "0411111111",
            email: "acme@example.com",
            customerType: "Company",
            dateOfBirth: null,
            occupation: null,
            abn: null,
            acn: null,
            industry: "Technology",
            contactPersonName: null,
            contactPersonPhone: null,
            contactPersonEmail: null
        );

        Assert.That(result, Is.TypeOf<ViewResult>());
        Assert.That(_context.Customers.Count(), Is.EqualTo(0));
        Assert.That(_controller.ModelState.IsValid, Is.False);
    }

    [Test]
    public async Task Create_WhenCustomerAlreadyExists_GeneratesNextCustomerIdBySeven()
    {
        _context.Customers.Add(new Customer
        {
            CustomerId = 2000000,
            Name = "Existing",
            Address = "Old Address",
            PhoneNumber = "0499999999",
            Email = "existing@example.com",
            CustomerType = "Person",
            CreatedAt = DateTime.Now,
            IsActive = true
        });
        await _context.SaveChangesAsync();

        await _controller.Create(
            name: "Alice",
            address: "1 Main St",
            phoneNumber: "0400000000",
            email: "alice2@example.com",
            customerType: "Person",
            dateOfBirth: new DateTime(1990, 1, 1),
            occupation: "Engineer",
            abn: null,
            acn: null,
            industry: null,
            contactPersonName: null,
            contactPersonPhone: null,
            contactPersonEmail: null
        );

        var createdCustomer = _context.Customers.Single(c => c.Email == "alice2@example.com");
        Assert.That(createdCustomer.CustomerId, Is.EqualTo(2000007));
    }

    [Test]
    public async Task Edit_WithValidCustomer_UpdatesAllowedFields()
    {
        var customer = new Customer
        {
            CustomerId = 2000000,
            Name = "Alice",
            Address = "1 Main St",
            PhoneNumber = "0400000000",
            Email = "alice@example.com",
            CustomerType = "Person",
            CreatedAt = DateTime.Now.AddDays(-1),
            IsActive = true
        };

        _context.Customers.Add(customer);
        await _context.SaveChangesAsync();

        var formCustomer = new Customer
        {
            CustomerId = customer.CustomerId,
            Name = "Alice Updated",
            Address = "99 New St",
            PhoneNumber = "0499999999",
            Email = "alice.updated@example.com",
            CustomerType = "Company",
            IsActive = false
        };

        var result = await _controller.Edit(customer.CustomerId, formCustomer);

        Assert.That(result, Is.TypeOf<RedirectToActionResult>());

        var updated = await _context.Customers.FindAsync(customer.CustomerId);
        Assert.That(updated, Is.Not.Null);
        Assert.That(updated!.Name, Is.EqualTo("Alice Updated"));
        Assert.That(updated.Address, Is.EqualTo("99 New St"));
        Assert.That(updated.PhoneNumber, Is.EqualTo("0499999999"));
        Assert.That(updated.Email, Is.EqualTo("alice.updated@example.com"));
        Assert.That(updated.IsActive, Is.False);
        Assert.That(updated.CustomerType, Is.EqualTo("Person"));
        Assert.That(updated.UpdatedAt, Is.Not.Null);
    }

    [Test]
    public async Task Edit_WhenRouteIdDoesNotMatchFormId_ReturnsNotFound()
    {
        var formCustomer = new Customer
        {
            CustomerId = 2000000,
            Name = "Alice Updated",
            Address = "99 New St",
            PhoneNumber = "0499999999",
            Email = "alice.updated@example.com",
            CustomerType = "Person",
            IsActive = false
        };

        var result = await _controller.Edit(999999, formCustomer);

        Assert.That(result, Is.TypeOf<NotFoundResult>());
    }

    [Test]
    public async Task Edit_WhenCustomerDoesNotExist_ReturnsNotFound()
    {
        var formCustomer = new Customer
        {
            CustomerId = 999999,
            Name = "Alice Updated",
            Address = "99 New St",
            PhoneNumber = "0499999999",
            Email = "alice.updated@example.com",
            CustomerType = "Person",
            IsActive = false
        };

        var result = await _controller.Edit(999999, formCustomer);

        Assert.That(result, Is.TypeOf<NotFoundResult>());
    }

    [Test]
    public async Task ChargeAllAccounts_ForPerson_ChargesAllRelatedAccounts()
    {
        var customer = new Customer
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

        var person = new Person
        {
            CustomerId = customer.CustomerId,
            Customer = customer,
            DateOfBirth = new DateTime(1990, 1, 1),
            Occupation = "Engineer"
        };

        var checkingAccount = new Account
        {
            AccountId = 1000,
            CustomerId = customer.CustomerId,
            Customer = customer,
            AccountType = "Checking",
            Balance = 100m,
            CreatedAt = DateTime.Now,
            IsActive = true
        };

        var savingsParentAccount = new Account
        {
            AccountId = 1005,
            CustomerId = customer.CustomerId,
            Customer = customer,
            AccountType = "Savings",
            Balance = 200m,
            CreatedAt = DateTime.Now,
            IsActive = true
        };

        var savingsAccount = new SavingsAccount
        {
            AccountId = savingsParentAccount.AccountId,
            Account = savingsParentAccount,
            InterestRate = 5m,
            MinimumBalance = 0m
        };

        savingsParentAccount.SavingsAccount = savingsAccount;

        _context.Customers.Add(customer);
        _context.Persons.Add(person);
        _context.Accounts.Add(checkingAccount);
        _context.Accounts.Add(savingsParentAccount);
        _context.SavingsAccounts.Add(savingsAccount);
        await _context.SaveChangesAsync();

        var result = await _controller.ChargeAllAccounts(customer.CustomerId, 25m);

        Assert.That(result, Is.TypeOf<RedirectToActionResult>());

        var updatedChecking = await _context.Accounts.FindAsync(checkingAccount.AccountId);
        var updatedSavings = await _context.Accounts.FindAsync(savingsParentAccount.AccountId);

        Assert.That(updatedChecking!.Balance, Is.EqualTo(75m));
        Assert.That(updatedSavings!.Balance, Is.EqualTo(175m));
    }

    [Test]
    public async Task ChargeAllAccounts_ForCompany_ChargesSavingsDouble()
    {
        var customer = new Customer
        {
            CustomerId = 2000007,
            Name = "Acme Pty Ltd",
            Address = "2 Main St",
            PhoneNumber = "0411111111",
            Email = "acme@example.com",
            CustomerType = "Company",
            CreatedAt = DateTime.Now,
            IsActive = true
        };

        var company = new Company
        {
            CustomerId = customer.CustomerId,
            Customer = customer,
            Abn = "12345678901",
            Acn = "123456789",
            Industry = "Technology",
            ContactPersonName = "Bob",
            ContactPersonPhone = "0422222222",
            ContactPersonEmail = "bob@example.com"
        };

        var checkingAccount = new Account
        {
            AccountId = 1010,
            CustomerId = customer.CustomerId,
            Customer = customer,
            AccountType = "Checking",
            Balance = 100m,
            CreatedAt = DateTime.Now,
            IsActive = true
        };

        var savingsParentAccount = new Account
        {
            AccountId = 1015,
            CustomerId = customer.CustomerId,
            Customer = customer,
            AccountType = "Savings",
            Balance = 200m,
            CreatedAt = DateTime.Now,
            IsActive = true
        };

        var savingsAccount = new SavingsAccount
        {
            AccountId = savingsParentAccount.AccountId,
            Account = savingsParentAccount,
            InterestRate = 5m,
            MinimumBalance = 0m
        };

        savingsParentAccount.SavingsAccount = savingsAccount;

        _context.Customers.Add(customer);
        _context.Companies.Add(company);
        _context.Accounts.Add(checkingAccount);
        _context.Accounts.Add(savingsParentAccount);
        _context.SavingsAccounts.Add(savingsAccount);
        await _context.SaveChangesAsync();

        var result = await _controller.ChargeAllAccounts(customer.CustomerId, 25m);

        Assert.That(result, Is.TypeOf<RedirectToActionResult>());

        var updatedChecking = await _context.Accounts.FindAsync(checkingAccount.AccountId);
        var updatedSavings = await _context.Accounts.FindAsync(savingsParentAccount.AccountId);

        Assert.That(updatedChecking!.Balance, Is.EqualTo(75m));
        Assert.That(updatedSavings!.Balance, Is.EqualTo(150m));
    }

    [Test]
    public async Task ChargeAllAccounts_WithMissingCustomer_ReturnsNotFound()
    {
        var result = await _controller.ChargeAllAccounts(999999, 25m);

        Assert.That(result, Is.TypeOf<NotFoundResult>());
    }

    [Test]
    public async Task ChargeAllAccounts_WithZeroAmount_ReturnsViewWithModelError()
    {
        var customer = new Customer
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

        var person = new Person
        {
            CustomerId = customer.CustomerId,
            Customer = customer,
            DateOfBirth = new DateTime(1990, 1, 1),
            Occupation = "Engineer"
        };

        _context.Customers.Add(customer);
        _context.Persons.Add(person);
        _context.SaveChanges();

        var result = await _controller.ChargeAllAccounts(customer.CustomerId, 0m);

        Assert.That(result, Is.TypeOf<ViewResult>());
        Assert.That(_controller.ModelState.IsValid, Is.False);
    }

    [Test]
    public async Task ChargeAllAccounts_WithNegativeAmount_ReturnsViewWithModelError()
    {
        var customer = new Customer
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

        var person = new Person
        {
            CustomerId = customer.CustomerId,
            Customer = customer,
            DateOfBirth = new DateTime(1990, 1, 1),
            Occupation = "Engineer"
        };

        _context.Customers.Add(customer);
        _context.Persons.Add(person);
        _context.SaveChanges();

        var result = await _controller.ChargeAllAccounts(customer.CustomerId, -5m);

        Assert.That(result, Is.TypeOf<ViewResult>());
        Assert.That(_controller.ModelState.IsValid, Is.False);
    }

    [Test]
    public async Task ChargeAllAccounts_ForCustomerWithNoAccounts_RedirectsAndDoesNothing()
    {
        var customer = new Customer
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

        var person = new Person
        {
            CustomerId = customer.CustomerId,
            Customer = customer,
            DateOfBirth = new DateTime(1990, 1, 1),
            Occupation = "Engineer"
        };

        _context.Customers.Add(customer);
        _context.Persons.Add(person);
        await _context.SaveChangesAsync();

        var result = await _controller.ChargeAllAccounts(customer.CustomerId, 25m);

        Assert.That(result, Is.TypeOf<RedirectToActionResult>());
        Assert.That(_context.Accounts.Count(a => a.CustomerId == customer.CustomerId), Is.EqualTo(0));
    }
}
