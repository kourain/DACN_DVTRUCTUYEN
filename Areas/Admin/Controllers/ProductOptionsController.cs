using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DACN_DVTRUCTUYEN.Models;

namespace DACN_DVTRUCTUYEN.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductOptionsController : Controller
    {
        private readonly DataContext _context;

        public ProductOptionsController(DataContext context)
        {
            _context = context;
        }

        // GET: Admin/ProductOptions
        public async Task<IActionResult> Index()
        {
            return View(await _context.ProductOptions.ToListAsync());
        }

        // GET: Admin/ProductOptions/Details/5
        public async Task<IActionResult> Details(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productOption = await _context.ProductOptions
                .FirstOrDefaultAsync(m => m.ProductID == id);
            if (productOption == null)
            {
                return NotFound();
            }

            return View(productOption);
        }

        // GET: Admin/ProductOptions/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/ProductOptions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProductID,OptionName,OptionValue,Quantity,SoldCount,PriceOld,PriceNow,UseUserAccount")] ProductOption productOption)
        {
            if (ModelState.IsValid)
            {
                _context.Add(productOption);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(productOption);
        }

        // GET: Admin/ProductOptions/Edit/5
        public async Task<IActionResult> Edit(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productOption = await _context.ProductOptions.FindAsync(id);
            if (productOption == null)
            {
                return NotFound();
            }
            return View(productOption);
        }

        // POST: Admin/ProductOptions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("ProductID,OptionName,OptionValue,Quantity,SoldCount,PriceOld,PriceNow,UseUserAccount")] ProductOption productOption)
        {
            if (id != productOption.ProductID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(productOption);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductOptionExists(productOption.ProductID))
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
            return View(productOption);
        }

        // GET: Admin/ProductOptions/Delete/5
        public async Task<IActionResult> Delete(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productOption = await _context.ProductOptions
                .FirstOrDefaultAsync(m => m.ProductID == id);
            if (productOption == null)
            {
                return NotFound();
            }

            return View(productOption);
        }

        // POST: Admin/ProductOptions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var productOption = await _context.ProductOptions.FindAsync(id);
            if (productOption != null)
            {
                _context.ProductOptions.Remove(productOption);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductOptionExists(string id)
        {
            return _context.ProductOptions.Any(e => e.ProductID == id);
        }
    }
}
