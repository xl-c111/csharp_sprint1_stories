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
        public async Task<IActionResult> Edit(long id, [Bind("AccountId,InterestRate,MinimumBalance")] SavingsAccount savingsAccount)
        {
            if (id != savingsAccount.AccountId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(savingsAccount);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SavingsAccountExists(savingsAccount.AccountId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["AccountId"] = new SelectList(_context.Accounts, "AccountId", "AccountId", savingsAccount.AccountId);
            return View(savingsAccount);
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
    }
}
