using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DACN_DVTRUCTUYEN.Models;
using System.Runtime.CompilerServices;
using System.Reflection;
using static System.Net.Mime.MediaTypeNames;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using SixLabors.ImageSharp;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.Data.SqlClient;

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
            id = id.ToLower();
            var product = await _context.Products
                .FirstOrDefaultAsync(m => m.ProductID == id);
            if (product == null)
            {
                RedirectToAction("Index");
            }

            return View((product, _context.ProductOptions.Where(m => m.ProductID == id).ToList(),_context.Product_Questions.Where(m=> m.ProductID == id).ToList()));
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
                product.ProductID = product.ProductID.ToLower();
                if (_context.Products.FirstOrDefault(m => m.ProductID == product.ProductID) != null)
                {
                    ViewBag.error = $"Trùng lặp mã sản phẩm : {product.ProductID}";
                    return View(product);
                }
                int img_upload_ok = 1;
                if (FormFile != null)
                {
                    //giới hạn tải lên 5MB
                    if (FormFile.Length <= 5242880)
                    {
                        string extension = Path.GetExtension(FormFile.FileName);
                        if (string.IsNullOrEmpty(extension))
                        {
                            extension = ".png"; //đặt mặc định .png
                        }
                        try
                        {
                            SixLabors.ImageSharp.Image.DetectFormat(FormFile.OpenReadStream());
                            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Product/img", $"{product.ProductID}{extension}");
                            using (var stream = System.IO.File.Create(filePath))
                            {
                                await FormFile.CopyToAsync(stream);
                            }
                            // Lưu đường dẫn tệp vào cơ sở dữ liệu
                            product.ProductImg = $"/Product/img/{product.ProductID}{extension}";
                        }
                        catch (UnknownImageFormatException)
                        {
                            img_upload_ok = 0;
                        }
                    }
                    else
                    {
                        img_upload_ok = 0;
                    }
                }
                product.ProductID = product.ProductID.ToLower();
                _context.Products.Add(product);
                _context.SaveChanges();
                if (img_upload_ok == 0)
                {
                    ViewBag.error = "Tệp hình ảnh không được chấp nhận, vui lòng thử 1 tệp khác";
                    return View(product);
                }
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
        public async Task<IActionResult> Edit(Product product, IFormFile FormFile, string productidold)
        {
            if (!string.IsNullOrEmpty(productidold) && productidold.IndexOf("'") != -1)
            {
                return BadRequest();
            }
            if (!string.IsNullOrEmpty(product.ProductID) && product.ProductID.IndexOf("'") != -1)
            {
                return BadRequest();
            }
            ModelState.Remove(nameof(FormFile));
            if (ModelState.IsValid)
            {
                product.ProductID = product.ProductID.ToLower();
                productidold = productidold.ToLower();
                if (product.ProductID != productidold)
                    if (_context.Products.FirstOrDefault(m => m.ProductID == product.ProductID) != null)
                    {
                        ViewBag.error = $"Trùng lặp mã sản phẩm : {product.ProductID}";
                        return View(product);
                    }
                int img_upload_ok = 1;
                if (FormFile != null)
                {
                    //giới hạn tải lên 5MB
                    if (FormFile.Length <= 5242880)
                    {
                        string extension = Path.GetExtension(FormFile.FileName);
                        if (string.IsNullOrEmpty(extension))
                        {
                            extension = ".png"; //đặt mặc định .png
                        }
                        try
                        {
                            SixLabors.ImageSharp.Image.DetectFormat(FormFile.OpenReadStream());
                            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Product/img", $"{product.ProductID}{extension}");
                            using (var stream = System.IO.File.Create(filePath))
                            {
                                await FormFile.CopyToAsync(stream);
                            }
                            // Lưu đường dẫn tệp vào cơ sở dữ liệu
                            product.ProductImg = $"/Product/img/{product.ProductID}{extension}";
                        }
                        catch (UnknownImageFormatException)
                        {
                            img_upload_ok = 0;
                        }
                    }
                    else
                    {
                        img_upload_ok = 0;
                    }
                }
                //nếu sử dụng Linq hay entiframework sẽ khiến viewcount trong thời gian admin chỉnh sửa không được lưu vào csdl
                try
                {
                    if (product.ProductID != productidold)
                        _context.Database.ExecuteSql(FormattableStringFactory.Create($"UPDATE [dbo].[PRODUCT] SET " +
                            $"PRODUCTID = '{product.ProductID}' " +
                            $" WHERE PRODUCTID = '{productidold}'"));
                    _context.Update(product);
                }
                catch (SqlException)
                {
                    ViewBag.error = "Sản phẩm đã và đang được sử dụng, không thể thay đổi dữ liệu!!!";
                    return View(product);
                }
                _context.SaveChanges();
                if (img_upload_ok == 0)
                {
                    ViewBag.error = "Tệp hình ảnh không được chấp nhận, vui lòng thử 1 tệp khác";
                    return View(product);
                }
                return RedirectToAction("Index");
            }
            return View(product);
        }

        // GET: Admin/Products/Delete/5
        [HttpGet]
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
        public async Task<IActionResult> DeleteConfirmed(string ProductID)
        {
            var product = await _context.Products.FirstOrDefaultAsync(m=> m.ProductID == ProductID);
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
