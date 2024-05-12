using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DACN_DVTRUCTUYEN.Models;
using System.Runtime.CompilerServices;

namespace DACN_DVTRUCTUYEN.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductsController : Controller
    {
        private readonly DataContext _context;

        public ProductsController(DataContext context)
        {
            _context = context;
        }

        // GET: Admin/Products
        public async Task<IActionResult> Index()
        {
            return View(await _context.Products.ToListAsync());
        }

        [HttpGet]
        [Route("/admin/products/details")]
        // GET: Admin/Products/Details/5
        public async Task<IActionResult> Details(string? id)
        {
            if (id == null)
            {
                RedirectToAction("Index");
            }

            var product = await _context.Products
                .FirstOrDefaultAsync(m => m.ProductID == id);
            if (product == null)
            {
                RedirectToAction("Index");
            }

            return View(product);
        }

        // GET: Admin/Products/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("/admin/products/create")]
        public async Task<IActionResult> Create(Product product, IFormFile FormFile)
        {
            ModelState.Remove(nameof(FormFile));
            if (ModelState.IsValid)
            {
                if (FormFile != null)
                {
                    string extension = Path.GetExtension(FormFile.FileName);
                    if (string.IsNullOrEmpty(extension))
                    {
                        extension = ".png"; //đặt mặc định .png
                    }
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Product/img", $"{product.ProductID}{extension}");
                    using (var stream = System.IO.File.Create(filePath))
                    {
                        await FormFile.CopyToAsync(stream);
                    }
                    // Lưu đường dẫn tệp vào cơ sở dữ liệu
                    product.ProductImg = $"/Product/img/{product.ProductID}{extension}";
                }
                _context.Products.Add(product);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(product);
        }
        // GET: Admin/Products/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var data = _context.Products.Where(m => m.ProductID == id).FirstOrDefault();
            if (data == null)
            {
                return NotFound();
            }
            return View(data);
        }

        // POST: Admin/Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("/admin/products/edit")]
        public async Task<IActionResult> Edit(Product product, IFormFile FormFile)
        {
            ModelState.Remove(nameof(FormFile));
            if (ModelState.IsValid)
            {
                if (FormFile != null)
                {
                    string extension = Path.GetExtension(FormFile.FileName);
                    if (string.IsNullOrEmpty(extension))
                    {
                        extension = ".png"; //đặt mặc định .png
                    }
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Product/img", $"{product.ProductID}{extension}");
                    using (var stream = System.IO.File.Create(filePath))
                    {
                        await FormFile.CopyToAsync(stream);
                    }
                    // Lưu đường dẫn tệp vào cơ sở dữ liệu
                    product.ProductImg = $"/Product/img/{product.ProductID}{extension}";
                }
                //nếu sử dụng Linq hay entiframework sẽ khiến viewcount trong thời gian admin chỉnh sửa không được lưu vào csdl
                _context.Database.ExecuteSql(FormattableStringFactory.Create($"UPDATE [dbo].[PRODUCT] SET " +
                    $"PRODUCTID = '{product.ProductID}', " +
                    $"PRODUCTNAME= N'{product.ProductName}'," +
                    $"ProductDescription = N'{product.ProductDescription}'," +
                    $"ProductImg = '{product.ProductImg}'" +
                    $" WHERE PRODUCTID = '{product.ProductID}'"));
                _context.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(product);
        }

        // GET: Admin/Products/Delete/5
        public async Task<IActionResult> Delete(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .FirstOrDefaultAsync(m => m.ProductID == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Admin/Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(string id)
        {
            return _context.Products.Any(e => e.ProductID == id);
        }
    }
}
