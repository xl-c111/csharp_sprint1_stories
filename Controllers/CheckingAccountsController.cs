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
    public class CheckingAccountsController : Controller
    {
        private readonly CsharpSprint1StoriesContext _context;

        public CheckingAccountsController(CsharpSprint1StoriesContext context)
        {
            _context = context;
        }

        // GET: CheckingAccounts
        public async Task<IActionResult> Index()
        {
            var csharpSprint1StoriesContext = _context.CheckingAccounts.Include(c => c.Account);
            return View(await csharpSprint1StoriesContext.ToListAsync());
        }

        // GET: CheckingAccounts/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var checkingAccount = await _context.CheckingAccounts
                .Include(c => c.Account)
                .FirstOrDefaultAsync(m => m.AccountId == id);
            if (checkingAccount == null)
            {
                return NotFound();
            }

            return View(checkingAccount);
        }

        // GET: CheckingAccounts/Create
        public IActionResult Create()
        {
            ViewData["AccountId"] = new SelectList(_context.Accounts, "AccountId", "AccountId");
            return View();
        }

        // POST: CheckingAccounts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AccountId,NextCheckNumber,OverdraftLimit")] CheckingAccount checkingAccount)
        {
            if (ModelState.IsValid)
            {
                _context.Add(checkingAccount);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AccountId"] = new SelectList(_context.Accounts, "AccountId", "AccountId", checkingAccount.AccountId);
            return View(checkingAccount);
        }

        // GET: CheckingAccounts/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var checkingAccount = await _context.CheckingAccounts.FindAsync(id);
            if (checkingAccount == null)
            {
                return NotFound();
            }
            ViewData["AccountId"] = new SelectList(_context.Accounts, "AccountId", "AccountId", checkingAccount.AccountId);
            return View(checkingAccount);
        }

        // POST: CheckingAccounts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("AccountId,NextCheckNumber,OverdraftLimit")] CheckingAccount checkingAccount)
        {
            if (id != checkingAccount.AccountId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(checkingAccount);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CheckingAccountExists(checkingAccount.AccountId))
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
            ViewData["AccountId"] = new SelectList(_context.Accounts, "AccountId", "AccountId", checkingAccount.AccountId);
            return View(checkingAccount);
        }

        // GET: CheckingAccounts/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var checkingAccount = await _context.CheckingAccounts
                .Include(c => c.Account)
                .FirstOrDefaultAsync(m => m.AccountId == id);
            if (checkingAccount == null)
            {
                return NotFound();
            }

            return View(checkingAccount);
        }

        // POST: CheckingAccounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var checkingAccount = await _context.CheckingAccounts.FindAsync(id);
            if (checkingAccount != null)
            {
                _context.CheckingAccounts.Remove(checkingAccount);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CheckingAccountExists(long id)
        {
            return _context.CheckingAccounts.Any(e => e.AccountId == id);
        }
    }
}
