using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using csharp_sprint1_stories.Models;
using Microsoft.EntityFrameworkCore.Migrations.Operations;

namespace Controllers
{
    public class CustomersController : Controller
    {
        private readonly CsharpSprint1StoriesContext _context;

        public CustomersController(CsharpSprint1StoriesContext context)
        {
            _context = context;
        }

        // GET: Customers
        public async Task<IActionResult> Index()
        {
            return View(await _context.Customers.ToListAsync());
        }

        // GET: Customers/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers
                .FirstOrDefaultAsync(m => m.CustomerId == id);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        // GET: Customers/Create
        public IActionResult Create()
        {
            ViewData["CustomerTypes"] = new SelectList(new List<string>
            {
                "Person",
                "Company"
            });

            return View();
        }

        // POST: Customers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            string name,
            string address,
            string phoneNumber,
            string email,
            string customerType,
            DateTime? dateOfBirth,
            string? occupation,
            string? abn,
            string? acn,
            string? industry,
            string? contactPersonName,
            string? contactPersonPhone,
            string? contactPersonEmail
            )
        {
            // step 1: clean the customer type input.
            customerType = customerType.Trim().ToLower();

            // step 2: make sure the customer type is either person or company.
            if (customerType != "person" && customerType != "company")
            {
                ModelState.AddModelError("", "Customer type must be person or company.");
                ViewData["CustomerTypes"] = new SelectList(new List<string> { "Person", "Company" });
                return View();
            }

            // step 3: if the customer is a person, make sure date of birth is provided.
            if (customerType == "person" && dateOfBirth == null)
            {
                ModelState.AddModelError("", "Date of birth is required for a person customer.");
                return View();
            }

            // step 4: if the customer is a company, make sure all required company fields are provided.
            if (customerType == "company")
            {
                if (string.IsNullOrWhiteSpace(abn) ||
                    string.IsNullOrWhiteSpace(acn) ||
                    string.IsNullOrWhiteSpace(contactPersonName) ||
                    string.IsNullOrWhiteSpace(contactPersonPhone) ||
                    string.IsNullOrWhiteSpace(contactPersonEmail))
                {
                    ModelState.AddModelError("", "ABN, ACN, and contact person details are required for a company customer.");
                    return View();
                }
            }

            // step 5: generate the next customer id.

            long nextCustomerId;

            // The first customer starts at 2000000, then each new customer increases by 7.
            if (await _context.Customers.AnyAsync())
            {
                nextCustomerId = await _context.Customers.MaxAsync(c => c.CustomerId) + 7;
            }
            else
            {
                nextCustomerId = 2000000;
            }

            // step 6: create the main customer record.
            var customer = new Customer
            {
                CustomerId = nextCustomerId,
                Name = name,
                Address = address,
                PhoneNumber = phoneNumber,
                Email = email,
                CustomerType = customerType == "person" ? "Person" : "Company",
                CreatedAt = DateTime.Now,
                IsActive = true
            };

            // step 7: add the customer to the database context.
            _context.Customers.Add(customer);

            // step 8: save the customer first so the related person or company record can use the same CustomerId as a foreign key.
            await _context.SaveChangesAsync();

            // step 9: create the matching child record based on the customer type.
            if (customerType == "person")
            {
                var person = new Person
                {
                    CustomerId = customer.CustomerId,
                    DateOfBirth = dateOfBirth.Value,
                    Occupation = occupation
                };

                _context.Persons.Add(person);
            }
            else
            {
                var company = new Company
                {
                    CustomerId = customer.CustomerId,
                    Abn = abn!,
                    Acn = acn!,
                    Industry = industry,
                    ContactPersonName = contactPersonName!,
                    ContactPersonPhone = contactPersonPhone!,
                    ContactPersonEmail = contactPersonEmail!
                };

                _context.Companies.Add(company);
            }

            // step 10: save the person or company record to the database.
            await _context.SaveChangesAsync();

            // step 11: redirect back to the customer list after successful creation.
            return RedirectToAction(nameof(Index));
        }

        // GET: Customers/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }
            return View(customer);
        }

        // POST: Customers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("CustomerId,Name,Address,PhoneNumber,Email,CustomerType,IsActive")] Customer formCustomer)
        {
            // step 1: make sure the route id matches the customer id submitted from the form.
            if (id != formCustomer.CustomerId)
            {
                return NotFound();
            }

            // step 2: find the existing customer record in the database.
            // We update this tracked entity instead of replacing it directly.
            var customer = await _context.Customers.FindAsync(id);

            if (customer == null)
            {
                return NotFound();
            }

            // step 3: check whether the submitted form data is valid.
            if (!ModelState.IsValid)
            {
                return View(formCustomer);
            }

            // step 4: update only the fields that are allowed to change
            // in the main Customers table.
            customer.Name = formCustomer.Name;
            customer.Address = formCustomer.Address;
            customer.PhoneNumber = formCustomer.PhoneNumber;
            customer.Email = formCustomer.Email;
            customer.IsActive = formCustomer.IsActive;

            // step 5: refresh the last updated timestamp.
            customer.UpdatedAt = DateTime.Now;

            try
            {
                // step 6: save the edited customer back to the database.
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                // step 7: if the customer no longer exists, return NotFound.
                // Otherwise, rethrow the exception.
                if (!CustomerExists(formCustomer.CustomerId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            // step 8: if everything succeeds, return to the customer list page.
            return RedirectToAction(nameof(Index));
        }

        // GET: Customers/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers
                .FirstOrDefaultAsync(m => m.CustomerId == id);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        // POST: Customers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer != null)
            {
                _context.Customers.Remove(customer);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CustomerExists(long id)
        {
            return _context.Customers.Any(e => e.CustomerId == id);
        }


        // GET: Customers/ChargeAllAccounts/5
        public async Task<IActionResult> ChargeAllAccounts(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers
                .Include(c => c.Person)
                .Include(c => c.Company)
                .Include(c => c.Accounts)
                .ThenInclude(a => a.SavingsAccount)
                .Include(c => c.Accounts)
                .ThenInclude(a => a.CheckingAccount)
                .FirstOrDefaultAsync(c => c.CustomerId == id);

            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        // POST: Customers/ChargeAllAccounts/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChargeAllAccounts(long id, decimal amount)
        {
            var customer = await _context.Customers
                .Include(c => c.Person)
                .Include(c => c.Company)
                .Include(c => c.Accounts)
                .ThenInclude(a => a.SavingsAccount)
                .Include(c => c.Accounts)
                .ThenInclude(a => a.CheckingAccount)
                .FirstOrDefaultAsync(c => c.CustomerId == id);

            if (customer == null)
            {
                return NotFound();
            }

            if (customer.Person != null)
            {
                customer.Person.ChargeAllAccounts(amount);
            }
            else if (customer.Company != null)
            {
                customer.Company.ChargeAllAccounts(amount);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Details), new { id = customer.CustomerId });
        }
    }
}
