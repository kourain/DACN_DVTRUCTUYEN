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
    public class ProductQuestionsController : Controller
    {
        private readonly DataContext _context;

        public ProductQuestionsController(DataContext context)
        {
            _context = context;
        }

        // GET: Admin/ProductQuestions
        public async Task<IActionResult> Index()
        {
            return View(await _context.Product_Questions.ToListAsync());
        }

        // GET: Admin/ProductQuestions/Create
        [HttpGet]
        [Route("/admin/ProductQuestions/create")]
        public IActionResult Create(string? productid)
        {
            var product = new Product_Question();
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

        // POST: Admin/ProductQuestions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("/admin/ProductQuestions/create")]
        public async Task<IActionResult> Create(Product_Question productquestion)
        {
            if (ModelState.IsValid)
            {
                _context.Add(productquestion);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(productquestion);
        }

        // GET: Admin/ProductQuestions/Edit/5
        [HttpGet]
        [Route("/admin/ProductQuestions/edit")]
        public async Task<IActionResult> Edit(string productid, int questionid)
        {
            productid = productid.ToLower();
            var productquestion = _context.Product_Questions.Where(m => m.ProductID == productid && m.QuestionId == questionid).FirstOrDefault();
            if (productquestion == null)
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
            return View(productquestion);
        }

        // POST: Admin/ProductQuestions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("/admin/ProductQuestions/edit")]
        public async Task<IActionResult> Edit(Product_Question productquestion)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(productquestion);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductQuestionExists(productquestion.ProductID))
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
            return View(productquestion);
        }

        // GET: Admin/ProductQuestions/Delete/5
        [HttpGet]
        [Route("/admin/ProductQuestions/Delete/{productid}/{questionid:int}")]
        public async Task<IActionResult> Delete(string productid, int questionid)
        {
            productid = productid.ToLower();
            var productquestion = await _context.Product_Questions
                .FirstOrDefaultAsync(m => m.ProductID == productid && m.QuestionId == questionid);
            if (productquestion == null)
            {
                return NotFound();
            }

            return View(productquestion);
        }

        // POST: Admin/ProductQuestions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Route("/admin/ProductQuestions/Delete/{productid}/{questionid:int}")]
        public async Task<IActionResult> DeleteConfirmed(string productid, int questionid)
        {
            var productquestion = await _context.Product_Questions.FirstOrDefaultAsync(m => m.ProductID == productid && m.QuestionId == questionid);
            if (productquestion != null)
            {
                _context.Product_Questions.Remove(productquestion);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductQuestionExists(string id)
        {
            return _context.Product_Questions.Any(e => e.ProductID == id);
        }
    }
}
