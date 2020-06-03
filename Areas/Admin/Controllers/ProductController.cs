using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OnlineShop.Data;
using OnlineShop.Models;

namespace OnlineShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private ApplicationDbContext _db; //taking reference of ApplicationDbContext
        private IHostingEnvironment _he; // Hosting environment to find the wwwroot path
        public ProductController(ApplicationDbContext db,IHostingEnvironment he) //creating constructor
        {
            _db = db;
            _he = he;
        }
        public IActionResult Index()
        {
            return View(_db.Products.Include(c=>c.ProductTypes).Include(f=>f.SpecialTags).ToList());
        }
        //Post Method For Index
        [HttpPost]
        public IActionResult Index(decimal? lowAmount,decimal? largeAmount) //for price searching
        {
            var products = _db.Products.Include(c => c.ProductTypes).Include(c => c.SpecialTags)
                .Where(c => c.Price >= lowAmount && c.Price <= largeAmount).ToList();
            if (lowAmount == null || largeAmount == null)
            {
                products = _db.Products.Include(c => c.ProductTypes).Include(c => c.SpecialTags).ToList();
            }
            return View(products);
        }
        //Get Method for Create
        public IActionResult Create()
        {
            ViewData["ProductTypeId"] = new SelectList(_db.ProductTypes.ToList(), "Id", "ProductType"); //to show data in dropdown, 
            ViewData["TagId"] = new SelectList(_db.SpecialTags.ToList(), "Id", "TagName");//to show data in dropdown, 
            return View();
        }
        //post method for create
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task <IActionResult> Create (Products products,IFormFile image)
        {
            if(ModelState.IsValid)
            {
                var searchProduct = _db.Products.FirstOrDefault(c => c.Name == products.Name); //to check if the product has same name or not
                if(searchProduct!=null)
                {
                    ViewBag.message = "This Product Is Already Exist";
                    ViewData["ProductTypeId"] = new SelectList(_db.ProductTypes.ToList(), "Id", "ProductType"); //to show data in dropdown, 
                    ViewData["TagId"] = new SelectList(_db.SpecialTags.ToList(), "Id", "TagName");//to show data in dropdown, 
                    return View(products);
                }
                if(image!=null)
                {
                    var name = Path.Combine(_he.WebRootPath + "/Images", Path.GetFileName(image.FileName));
                    await image.CopyToAsync(new FileStream(name, FileMode.Create)); //
                    products.Image = "Images/" + image.FileName; //Save the file name
                }
                if(image==null)
                {
                    products.Image = "Images/noimage.PNG";

                }
                //add to db
                _db.Products.Add(products);
                //save changes
                await _db.SaveChangesAsync();
                //redirect to action
                return RedirectToAction(nameof(Index));
            }
            return View(products);
        }
        //Edit Action Method
        public ActionResult Edit(int?id)
        {
            ViewData["ProductTypeId"] = new SelectList(_db.ProductTypes.ToList(), "Id", "ProductType"); //to show/binding data in dropdown, 
            ViewData["TagId"] = new SelectList(_db.SpecialTags.ToList(), "Id", "TagName");//to show/binding data in dropdown, 
            if(id==null)
            {
                return NotFound();
            }
            var product = _db.Products.Include(c => c.ProductTypes).Include(c => c.SpecialTags).FirstOrDefault(c => c.Id == id);
            if(product==null)
            {
                return NotFound();
            }
            return View(product);
        }
        //Post Edit ACtion Method
        [HttpPost]
        public async Task<IActionResult> Edit(Products products, IFormFile image)
        {
            if (ModelState.IsValid)
            {
                if (image != null)
                {
                    var name = Path.Combine(_he.WebRootPath + "/Images", Path.GetFileName(image.FileName));
                    await image.CopyToAsync(new FileStream(name, FileMode.Create)); //
                    products.Image = "Images/" + image.FileName; //Save the file name
                }
                if (image == null)
                {
                    products.Image = "Images/noimage.PNG";

                }
                //add to db
                _db.Products.Update(products);
                //save changes
                await _db.SaveChangesAsync();
                //redirect to action
                return RedirectToAction(nameof(Index));
            }
            return View(products);
        }
        //Get Details Method
        public ActionResult Details(int?id)
        {
            if(id==null)
            {
                return NotFound();
            }
            var product = _db.Products.Include(c => c.ProductTypes).Include(c => c.SpecialTags).FirstOrDefault(c => c.Id == id);
            if(product==null)
            {
                return NotFound();
            }
            return View(product);
        }
        //Get Delete Method
        public ActionResult Delete(int?id)
        {
            if(id==null)
            {
                return NotFound();
            }
            var product = _db.Products.Include(c => c.ProductTypes).Include(c => c.SpecialTags).Where(c => c.Id == id).FirstOrDefault();
            if(product==null)
            {
                return NotFound();
            }

            return View(product);
        }
        //POST Delete Action Method

        [HttpPost]
        [ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirm(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = _db.Products.FirstOrDefault(c => c.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            _db.Products.Remove(product);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


    }
}