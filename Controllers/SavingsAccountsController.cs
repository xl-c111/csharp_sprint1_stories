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
    public class SavingsAccountsController : Controller
    {
        private readonly CsharpSprint1StoriesContext _context;

        public SavingsAccountsController(CsharpSprint1StoriesContext context)
        {
            _context = context;
        }

        // GET: SavingsAccounts
        public async Task<IActionResult> Index()
        {
            var csharpSprint1StoriesContext = _context.SavingsAccounts.Include(s => s.Account);
            return View(await csharpSprint1StoriesContext.ToListAsync());
        }

        // GET: SavingsAccounts/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var savingsAccount = await _context.SavingsAccounts
                .Include(s => s.Account)
                .FirstOrDefaultAsync(m => m.AccountId == id);
            if (savingsAccount == null)
            {
                return NotFound();
            }

            return View(savingsAccount);
        }

        // GET: SavingsAccounts/Create
        public IActionResult Create()
        {
            ViewData["AccountId"] = new SelectList(_context.Accounts, "AccountId", "AccountId");
            return View();
        }

        // POST: SavingsAccounts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AccountId,InterestRate,MinimumBalance")] SavingsAccount savingsAccount)
        {
            if (ModelState.IsValid)
            {
                _context.Add(savingsAccount);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AccountId"] = new SelectList(_context.Accounts, "AccountId", "AccountId", savingsAccount.AccountId);
            return View(savingsAccount);
        }

        // GET: SavingsAccounts/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var savingsAccount = await _context.SavingsAccounts.FindAsync(id);
            if (savingsAccount == null)
            {
                return NotFound();
            }
            ViewData["AccountId"] = new SelectList(_context.Accounts, "AccountId", "AccountId", savingsAccount.AccountId);
            return View(savingsAccount);
        }

        // POST: SavingsAccounts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("AccountId,InterestRate,MinimumBalance")] SavingsAccount formSavingsAccount)
        {
            // step 1: make sure the route id matches the savings account id submitted from the form.
            if (id != formSavingsAccount.AccountId)
            {
                return NotFound();
            }

            // step 2: find the existing savings account record in the database.
            var savingsAccount = await _context.SavingsAccounts.FindAsync(id);

            if (savingsAccount == null)
            {
                return NotFound();
            }

            // step 3: remove validation for the navigation property that is not edited in this form.
            ModelState.Remove("Account");

            // step 4: check whether the submitted form data is valid.
            if (!ModelState.IsValid)
            {
                ViewData["AccountId"] = new SelectList(_context.Accounts, "AccountId", "AccountId", formSavingsAccount.AccountId);
                return View(formSavingsAccount);
            }

            // step 5: update only the editable savings account fields.
            savingsAccount.InterestRate = formSavingsAccount.InterestRate;
            savingsAccount.MinimumBalance = formSavingsAccount.MinimumBalance;

            try
            {
                // step 6: save the changes to the database.
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                // step 7: if the savings account no longer exists, return NotFound.
                if (!SavingsAccountExists(formSavingsAccount.AccountId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            // step 8: if everything succeeds, return to the savings account list page.
            return RedirectToAction(nameof(Index));
        }

        // GET: SavingsAccounts/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var savingsAccount = await _context.SavingsAccounts
                .Include(s => s.Account)
                .FirstOrDefaultAsync(m => m.AccountId == id);
            if (savingsAccount == null)
            {
                return NotFound();
            }

            return View(savingsAccount);
        }

        // POST: SavingsAccounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var savingsAccount = await _context.SavingsAccounts.FindAsync(id);
            if (savingsAccount != null)
            {
                _context.SavingsAccounts.Remove(savingsAccount);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SavingsAccountExists(long id)
        {
            return _context.SavingsAccounts.Any(e => e.AccountId == id);
        }

        // POST: SavingsAccounts/AddInterest/5
        /// <summary>
        /// Applies the configured interest rate to a savings account and saves the updated balance.
        /// </summary>
        /// <param name="id">The account id of the savings account.</param>
        /// <returns>
        /// Redirects back to the savings account details page after interest is applied.
        /// </returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddInterest(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var savingsAccount = await _context.SavingsAccounts
                .Include(s => s.Account)
                .FirstOrDefaultAsync(s => s.AccountId == id);

            if (savingsAccount == null)
            {
                return NotFound();
            }

            savingsAccount.AddInterest();
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id = savingsAccount.AccountId });
        }
    }
}
