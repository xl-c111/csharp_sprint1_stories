using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using csharp_sprint1_stories.Models;

namespace Controllers
{
    public class AccountsController : Controller
    {
        private readonly CsharpSprint1StoriesContext _context;

        public AccountsController(CsharpSprint1StoriesContext context)
        {
            _context = context;
        }

        // GET: Accounts
        public async Task<IActionResult> Index()
        {
            var csharpSprint1StoriesContext = _context.Accounts
                .Include(a => a.Customer)
                .Include(a => a.SavingsAccount)
                .Include(a => a.CheckingAccount);

            return View(await csharpSprint1StoriesContext.ToListAsync());
        }

        // GET: Accounts/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var account = await _context.Accounts
                .Include(a => a.Customer)
                .Include(a => a.SavingsAccount)
                .Include(a => a.CheckingAccount)
                .FirstOrDefaultAsync(m => m.AccountId == id);

            if (account == null)
            {
                return NotFound();
            }

            return View(account);
        }

        // GET: Accounts/Create
        public IActionResult Create()
        {
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "Name");
            ViewData["AccountTypes"] = new SelectList(new List<string> { "Checking", "Savings" });
            return View();
        }

        // POST: Accounts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        // step 1: user submits Create Account form. 
        // step 2: customerId, accountType, and balance from the form.
        public async Task<IActionResult> Create(long customerId, string accountType, decimal balance)
        {
            // step 3: check whether selected customer exists in the database.
            var customer = await _context.Customers.FindAsync(customerId);

            if (customer == null)
            {
                return NotFound();
            }

            // step 4: clean the account type input.
            accountType = accountType.Trim().ToLower();

            // step 5: check accountType is either "checking" or "savings".
            if (accountType != "checking" && accountType != "savings")
            {
                ModelState.AddModelError("", "Account type must be checking or savings.");

                // rebuild the customer dropdown
                ViewData["CustomerId"] = new SelectList(
                    _context.Customers, // dataSource 
                    "CustomerId", // valueField
                    "Name",  // displayTextField
                    customerId  // selectedValue 
                );
                ViewData["AccountTypes"] = new SelectList(new List<string> { "Checking", "Savings" });

                // step 6: if invalid, show the form again with an error.
                return View();
            }

            long nextAccountId;

            if (await _context.Accounts.AnyAsync())
            {
                nextAccountId = await _context.Accounts.MaxAsync(a => a.AccountId) + 5;
            }
            else
            {
                nextAccountId = 1000;
            }

            //step 7: create the main Account object.
            var account = new Account
            {
                AccountId = nextAccountId,
                CustomerId = customerId,
                AccountType = accountType == "checking" ? "Checking" : "Savings",
                Balance = balance,
                CreatedAt = DateTime.Now,
                IsActive = true
            };

            //step 8: Add the main account record to the database context.
            // This prepares EF Core to insert the account into the Accounts table.
            _context.Accounts.Add(account);
            // Save the main account first so the child account can reference its AccountId.
            await _context.SaveChangesAsync();

            // step 9: if checking, create one CheckingAccount row.
            if (accountType == "checking")
            {
                var checkingAccount = new CheckingAccount
                {
                    // connect it to the main account
                    // child table AccountId = Main table AccountId
                    AccountId = account.AccountId,
                    NextCheckNumber = 1
                };

                // add the checking account to the database context
                _context.CheckingAccounts.Add(checkingAccount);
            }
            else
            {
                // step 10: if savings, create one SavingsAccount row.
                var savingsAccount = new SavingsAccount
                {
                    AccountId = account.AccountId,
                    InterestRate = 0,
                    MinimumBalance = 0
                };

                _context.SavingsAccounts.Add(savingsAccount);
            }

            // step 11: Save the child account record to the database.
            // This inserts either the CheckingAccount row or the SavingsAccount row.
            await _context.SaveChangesAsync();

            // step 12: Redirect to the account list page after the account is created successfully.
            return RedirectToAction(nameof(Index));
        }

        // GET: Accounts/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var account = await _context.Accounts.FindAsync(id);
            if (account == null)
            {
                return NotFound();
            }
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "CustomerId", account.CustomerId);
            return View(account);
        }

        // POST: Accounts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("AccountId,CustomerId,AccountType,Balance,CreatedAt,IsActive")] Account formAccount)
        {
            // step 1: make sure the route id matches the account id submitted from the form.
            // This helps prevent updating the wrong account.
            if (id != formAccount.AccountId)
            {
                return NotFound();
            }

            // step 2: find the existing account record in the database.
            // We update this tracked entity instead of replacing it directly.
            var account = await _context.Accounts.FindAsync(id);
            if (account == null)
            {
                return NotFound();
            }

            // step 3: remove validation for navigation properties that are not edited in this form.
            ModelState.Remove("Customer");
            ModelState.Remove("CheckingAccount");
            ModelState.Remove("SavingsAccount");

            // step 4: check whether the submitted form data is valid.
            // If not, rebuild the customer dropdown and return the form again.
            if (!ModelState.IsValid)
            {
                ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "Name", formAccount.CustomerId);

                return View(formAccount);
            }

            // step 5: update only the fields that are allowed to change
            // in the main Accounts table.
            account.CustomerId = formAccount.CustomerId;
            account.Balance = formAccount.Balance;
            account.IsActive = formAccount.IsActive;

            // step 6: refresh the last updated timestamp.
            account.UpdatedAt = DateTime.Now;

            try
            {
                // step 7: save the edited account back to the database.
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                // step 8: if the account no longer exists, return NotFound.
                if (!_context.Accounts.Any(e => e.AccountId == id))
                {
                    return NotFound();
                }

                throw;
            }

            // step 9: if everything succeeds, return to the account list page.
            return RedirectToAction(nameof(Index));
        }

        // GET: Accounts/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var account = await _context.Accounts
                .Include(a => a.Customer)
                .FirstOrDefaultAsync(m => m.AccountId == id);
            if (account == null)
            {
                return NotFound();
            }

            return View(account);
        }

        // POST: Accounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var account = await _context.Accounts.FindAsync(id);
            if (account != null)
            {
                _context.Accounts.Remove(account);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AccountExists(long id)
        {
            return _context.Accounts.Any(e => e.AccountId == id);
        }


        // GET: Accounts/Deposit/5
        public async Task<IActionResult> Deposit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var account = await _context.Accounts
                .Include(a => a.Customer)
                .FirstOrDefaultAsync(a => a.AccountId == id);

            if (account == null)
            {
                return NotFound();
            }

            return View(account);
        }

        // POST: Accounts/Deposit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Deposit(long id, decimal amount)
        {
            var account = await _context.Accounts
                .Include(a => a.Customer)
                .FirstOrDefaultAsync(a => a.AccountId == id);

            if (account == null)
            {
                return NotFound();
            }

            try
            {
                account.Deposit(amount);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Details), new { id = account.AccountId });
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(account);
            }
        }
    }
}
