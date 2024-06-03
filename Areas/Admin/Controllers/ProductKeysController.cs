using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DACN_DVTRUCTUYEN.Models;
using System.Runtime.CompilerServices;
using Microsoft.CodeAnalysis;

namespace DACN_DVTRUCTUYEN.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductKeys : Controller
    {
        private readonly DataContext _context;

        public ProductKeys(DataContext context)
        {
            _context = context;
        }

        // GET: Admin/ProductKeys
        public async Task<IActionResult> Index()
        {
            return View(await _context.Product_Keys.ToListAsync());
        }

        // GET: Admin/ProductKeys/Create
        [HttpGet]
        [Route("/admin/ProductKeys/create")]
        public async Task<IActionResult> Create([FromQuery]string id, [FromQuery]string optionvalue)
        {
            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(optionvalue))
            {
                return NotFound();
            }
            id = id.ToLower();
            optionvalue = optionvalue.ToLower();
            var product = new Product_Key();
            ViewBag.productList = (from m in _context.Products.Where(m => m.ProductID == id)
                                   select new SelectListItem()
                                   {
                                       Text = m.ProductName,
                                       Value = m.ProductID
                                   }
                          ).ToList();
            var temp = _context.ProductOptions.Where(m => m.ProductID == id && m.OptionValue == optionvalue).ToList();
            if (temp.FirstOrDefault() == null)
                return Redirect($"/admin/productoptions/details?id={id}&option={optionvalue}");
            if(temp.FirstOrDefault().Type != 1)
                return Redirect($"/admin/productoptions/details?id={id}&option={optionvalue}");
            ViewBag.ProductOption = (from m in temp
                                     select new SelectListItem()
                                     {
                                         Text = m.OptionName,
                                         Value = m.OptionValue
                                     }
                          ).ToList();
            product.ProductID = id;
            product.OptionValue = optionvalue;
            return View(product);
        }

        // POST: Admin/ProductKeys/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("/admin/ProductKeys/create")]
        public async Task<IActionResult> Create(Product_Key productkey)
        {
            if (ModelState.IsValid)
            {
                _context.Add(productkey);
                await _context.SaveChangesAsync();
                return Redirect($"/admin/productoptions/details?id={productkey.ProductID}&option={productkey.OptionValue}");
            }
            Update_quantity(productkey.ProductID, productkey.OptionValue);
            return View(productkey);
        }

        // GET: Admin/ProductKeys/Edit/5
        [HttpGet]
        [Route("/admin/ProductKeys/edit")]
        public async Task<IActionResult> Edit(string productid, string optionvalue, string key1)
        {
            if (string.IsNullOrEmpty(productid) || string.IsNullOrEmpty(optionvalue) || string.IsNullOrEmpty(key1))
            {
                return NotFound();
            }
            productid = productid.ToLower();
            optionvalue = optionvalue.ToLower();
            key1 = key1.ToLower();
            var productkey = _context.Product_Keys.Where(m => m.ProductID == productid && m.OptionValue == optionvalue && m.Key1 == key1).FirstOrDefault();
            if (productkey == null)
            {
                return NotFound();
            }
            ViewBag.productList = (from m in _context.Products.Where(m => m.ProductID == productid)
                                   select new SelectListItem()
                                   {
                                       Text = m.ProductName,
                                       Value = m.ProductID
                                   }
                          ).ToList();
            ViewBag.ProductOption = (from m in _context.Products.Where(m => m.ProductID == productid)
                                     select new SelectListItem()
                                     {
                                         Text = m.ProductName,
                                         Value = m.ProductID
                                     }
                          ).ToList();
            return View(productkey);
        }

        // POST: Admin/ProductKeys/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("/admin/ProductKeys/edit")]
        public async Task<IActionResult> Edit(Product_Key productkey, string oldkey1)
        {
            if (ModelState.IsValid)
            {
                oldkey1 = oldkey1.ToLower();
                if (_context.Product_Keys.Where(m => m.ProductID == productkey.ProductID && productkey.OptionValue == m.OptionValue && m.Key1 == oldkey1).FirstOrDefault() == null)
                {
                    return NotFound();
                }
                try
                {
                    if (productkey.Key1 != oldkey1)
                        _context.Database.ExecuteSql(FormattableStringFactory.Create($"UPDATE [dbo].[Product_Key] SET " +
                            $"Key1 = '{productkey.Key1}', " +
                            $" WHERE PRODUCTID = '{productkey.ProductID}' AND OPTIONVALUE = '{productkey.OptionValue}' AND KEY1 = '{oldkey1}';"));
                    _context.Update(productkey);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductKeyExists(productkey.ProductID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return Redirect($"/admin/productoptions/details?id={productkey.ProductID}&option={productkey.OptionValue}");
            }
            return View(productkey);
        }

        // GET: Admin/ProductKeys/Delete/5
        [HttpGet]
        [Route("/admin/ProductKeys/Delete/{productid}/{optionvalue}/{key1}")]
        public async Task<IActionResult> Delete(string productid, string optionvalue, string key1)
        {
            productid = productid.ToLower();
            var productkey = await _context.Product_Keys
                .FirstOrDefaultAsync(m => m.ProductID == productid && m.OptionValue == optionvalue && m.Key1 == key1);
            if (productkey == null)
            {
                return NotFound();
            }

            return View(productkey);
        }

        // POST: Admin/ProductKeys/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Route("/admin/ProductKeys/Delete/{productid}/{optionvalue}/{key1}")]
        public async Task<IActionResult> DeleteConfirmed(string productid, string optionvalue, string key1)
        {
            var productkey = await _context.Product_Keys.FirstOrDefaultAsync(m => m.ProductID == productid && m.OptionValue == optionvalue && m.Key1 == key1);
            if (productkey != null)
            {
                _context.Product_Keys.Remove(productkey);
            }

            await _context.SaveChangesAsync();
            Update_quantity(productid, optionvalue);
            return Redirect($"/admin/productoptions/details?id={productkey.ProductID}&option={productkey.OptionValue}");
        }

        private bool ProductKeyExists(string id)
        {
            return _context.Product_Keys.Any(e => e.ProductID == id);
        }
        //MOVE TO PRODUCT OPTIONCONTROLLER
        // GET: Admin/ProductKeys/Update
        //[HttpGet]
        //[Route("/admin/ProductKeys/Update")]
        public bool Update_quantity(string id, string optionvalue)
        {
            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(optionvalue))
            {
                return false;
            }
            id = id.ToLower();
            optionvalue = optionvalue.ToLower();
            var productoption = _context.ProductOptions.FirstOrDefault(m => m.ProductID == id && m.OptionValue == optionvalue);
            if (productoption != null)
            {
                productoption.SoldCount = _context.Product_Keys.Count(m => m.OrderID != "0");
                productoption.Quantity = _context.Product_Keys.Count(m => m.OrderID == "0");
                _context.Update(productoption);
                _context.SaveChanges();
            }
            return true;
        }
    }
}
