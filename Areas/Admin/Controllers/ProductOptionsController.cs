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

        [HttpGet]
        [Route("/admin/ProductOptions/details")]
        // GET: Admin/ProductOptions/Details/5
        public async Task<IActionResult> Details(string id,string option)
        {
            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(option))
            {
                return RedirectToAction("Index");
            }
            id = id.ToLower();
            var productoption = _context.ProductOptions
                .FirstOrDefault(m => m.ProductID == id && m.OptionValue == option);
            if (productoption == null)
            {
                return RedirectToAction("Index");
            }
            return View((productoption, _context.Product_Keys.Where(m => m.ProductID == id && m.OptionValue == option).ToList()));
        }
        // GET: Admin/ProductOptions/Create
        [HttpGet]
        [Route("/admin/productoptions/create")]
        public IActionResult Create(string? productid)
        {
            var product = new ProductOption();
            if (string.IsNullOrEmpty(productid))
            {
                productid = "";
                ViewBag.productList = (from m in _context.Products
                                       select new SelectListItem()
                                       {
                                           Text = m.ProductName,
                                           Value = m.ProductID
                                       }
                              ).ToList();
            }
            else
            {

                ViewBag.productList = (from m in _context.Products.Where(m => m.ProductID == productid)
                                       select new SelectListItem()
                                       {
                                           Text = m.ProductName,
                                           Value = m.ProductID
                                       }
                              ).ToList();
            }
            product.ProductID = productid;
            return View(product);
        }

        // POST: Admin/ProductOptions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductOption productOption)
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
        [HttpGet]
        [Route("/admin/ProductOptions/edit/{productid}/{optionvalue}")]
        public async Task<IActionResult> Edit(string productid, string optionvalue)
        {
            productid = productid.ToLower();
            optionvalue = optionvalue.ToLower();
            var productOption = _context.ProductOptions.Where(m => m.ProductID == productid && m.OptionValue == optionvalue).FirstOrDefault();
            if (productOption == null)
            {
                return NotFound();
            }
            ViewBag.productList = (from m in _context.Products
                                   select new SelectListItem()
                                   {
                                       Text = m.ProductName,
                                       Value = m.ProductID
                                   }
                          ).ToList();
            return View(productOption);
        }

        // POST: Admin/ProductOptions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("/admin/productoptions/edit/{productid}/{optionvalue}")]
        public async Task<IActionResult> Edit(string productid, string optionvalue, ProductOption productOption)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    productOption.CreateDate = DateTime.Now;
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
        [HttpGet]
        [Route("/admin/ProductOptions/Delete/{productid}/{optionvalue}")]
        public async Task<IActionResult> Delete(string productid, string optionvalue)
        {
            productid = productid.ToLower();
            optionvalue = optionvalue.ToLower();
            var productOption = await _context.ProductOptions
                .FirstOrDefaultAsync(m => m.ProductID == productid && m.OptionValue == optionvalue);
            if (productOption == null)
            {
                return NotFound();
            }

            return View(productOption);
        }

        // POST: Admin/ProductOptions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Route("/admin/ProductOptions/Delete/{productid}/{optionvalue}")]
        public async Task<IActionResult> DeleteConfirmed(ProductOption option)
        {
            var productOption = await _context.ProductOptions.FirstOrDefaultAsync(m => m.OptionValue == option.OptionValue && m.ProductID == option.ProductID);
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
