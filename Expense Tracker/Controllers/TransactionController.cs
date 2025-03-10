using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Expense_Tracker.Models;
using Microsoft.AspNetCore.Http;

namespace Expense_Tracker.Controllers
{
    public class TransactionController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TransactionController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Transaction
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Transactions.OrderByDescending(p => p.Date).Include(t => t.Category);
            var error = HttpContext.Session.GetString("Error");
            if (!string.IsNullOrWhiteSpace(error))
            {
                ViewBag.Error = error;
                HttpContext.Session.SetString("Error", "");

            }
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Transaction/AddOrEdit
        public IActionResult AddOrEdit(int id = 0)
        {
            var CategoryCollection = _context.Categories.ToList();
            Category DefaultCategory = new Category() { CategoryId = 0, Title = "Choose a Category" };
            CategoryCollection.Insert(0, DefaultCategory);
            ViewBag.Categories = CategoryCollection;
            if (id == 0)
                return View(new Transaction());
            else
                return View(_context.Transactions.Find(id));
        }

        // POST: Transaction/AddOrEdit
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit([Bind("TransactionId,CategoryId,Amount,Note,Date")] Transaction transaction)
        {
            if (ModelState.IsValid)
            {


                if (transaction.TransactionId == 0)
                {
                    var inseredBefore = _context.Transactions.Where(p => p.Amount == transaction.Amount && p.Date.Date == transaction.Date.Date && p.CategoryId == transaction.CategoryId).FirstOrDefault();
                    if (inseredBefore != null)
                    {
                        HttpContext.Session.SetString("Error", "This was entered before");

                        return RedirectToAction(nameof(Index));

                    }
                    else
                    {
                        _context.Add(transaction);

                    }

                }
                else
                    _context.Update(transaction);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            var CategoryCollection = _context.Categories.ToList();
            Category DefaultCategory = new Category() { CategoryId = 0, Title = "Choose a Category" };
            CategoryCollection.Insert(0, DefaultCategory);
            ViewBag.Categories = CategoryCollection;
            return View(transaction);
        }



        [HttpPost]
        public async Task<IActionResult> FilterByDate([Bind("date1,date2")] DateTime date1, DateTime date2)
        {
            var applicationDbContext = _context.Transactions.Where(p => p.Date.Date >= date1.Date && p.Date.Date <= date2.Date).OrderByDescending(p => p.Date).Include(t => t.Category).ToList();


            return View("Index", applicationDbContext);
        }

        // POST: Transaction/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Transactions == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Transactions'  is null.");
            }
            var transaction = await _context.Transactions.FindAsync(id);
            if (transaction != null)
            {
                _context.Transactions.Remove(transaction);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }



    }
}
