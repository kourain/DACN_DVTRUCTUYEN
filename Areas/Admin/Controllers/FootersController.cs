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
    public class FootersController : Controller
    {
        private readonly DataContext _Context;
        public FootersController(DataContext context)
        {
            _Context = context;
        }
        public IActionResult Index()
        {

            var mnlist = _Context.Footers.OrderBy(m => m.FooterId).ToList();
            return View(mnlist);
        }
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var mn = _Context.Footers.Find(id);
            if (mn == null)
            {
                return NotFound();
            }
            return View(mn);
        }
        [HttpPost]
        public IActionResult Delete(int id)
        {
            var deleFooter = _Context.Footers.Find(id);
            if (deleFooter == null)
            {
                return NotFound();
            }
            _Context.Footers.Remove(deleFooter);
            _Context.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult Create()
        {
            var mnlist = (from m in _Context.Footers.Where(m => m.ParentID == 0)
                          select new SelectListItem()
                          {
                              Text = m.ItemText,
                              Value = m.FooterId.ToString()
                          }
                          ).ToList();
            mnlist.Insert(0, new SelectListItem()
            {
                Text = "Default",
                Value = "0"
            });
            ViewBag.mnlist = mnlist;
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Footer mn)
        {
            if (ModelState.IsValid)
            {
                _Context.Footers.Add(mn);
                _Context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View();
        }

        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var mn = _Context.Footers.Find(id);
            if (mn == null)
            {
                return NotFound();
            }
            var mnlist = (from m in _Context.Footers.Where(m => m.ParentID == 0)
                          select new SelectListItem()
                          {
                              Text = m.ItemText,
                              Value = m.FooterId.ToString()
                          }
                        ).ToList();
            mnlist.Insert(0, new SelectListItem()
            {
                Text = "Default",
                Value = "0"
            });
            ViewBag.mnlist = mnlist;
            return View(mn);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Footer mn)
        {
            if (ModelState.IsValid)
            {
                _Context.Footers.Update(mn);
                _Context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View();
        }
    }
}
