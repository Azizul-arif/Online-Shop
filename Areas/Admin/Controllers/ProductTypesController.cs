using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Diagnostics;
using OnlineShop.Data;
using OnlineShop.Models;

namespace OnlineShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize] // to access only authorized people/admin
    public class ProductTypesController : Controller
    {
        private ApplicationDbContext _db; //taking reference of ApplicationDbContext
        public ProductTypesController(ApplicationDbContext db) //creating constructor
        {
            _db = db;
        }
        [AllowAnonymous] //anonymous user can view the index page only
        public IActionResult Index()
        {
            return View(_db.ProductTypes.ToList()); //see my product types in a list way
        }

        //Use get Method for Create
        
        public ActionResult Create()
        {
            return View();
        }
        //use Post Method for Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult>Create(ProductTypes productTypes)
        {
            if(ModelState.IsValid)
            {
                _db.ProductTypes.Add(productTypes); //add to db
                await _db.SaveChangesAsync(); //savechangesasync
                //code for alertify js that shows notification
                //TempData["save"] = "Data Save Successfully";
                TempData["save"] = " Data Save Successfully";
                //ViewData["save"] = "Save";
                return RedirectToAction(actionName: nameof(Index)); //RedirectToAction
            }
            return View(productTypes);
        }
        //Use get Method for Update/Edit
        public ActionResult Edit(int?id)
        {
            if(id==null)
            {
                return NotFound();
            }
            var productType = _db.ProductTypes.Find(id);
            if(productType==null) // this productType came from var productType
            {
                return NotFound();
            }
            return View(productType);
        }
        //use Post Method for Update/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit (ProductTypes productTypes)
        {
            if (ModelState.IsValid)
            {
                _db.Update(productTypes); //Update  to db
                await _db.SaveChangesAsync(); //savechangesasync
                TempData["edit"] = "Product type has been updated"; //this is for notification
                return RedirectToAction(actionName: nameof(Index)); //RedirectToAction
            }
            return View(productTypes);
        }
        //Use get Method for Details
        public ActionResult Details(int?id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var productType = _db.ProductTypes.Find(id);
            if (productType == null) // this productType came from var productType
            {
                return NotFound();
            }
            return View(productType);
        }
        //use Post Method for Details
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Details (ProductTypes productTypes)
        {
            //if (ModelState.IsValid)
            //{
               // _db.Update(productTypes); //Update  to db
               // await _db.SaveChangesAsync(); //savechangesasync
                return RedirectToAction(actionName: nameof(Index)); //RedirectToAction
            //}
            //return View(productTypes);
        }
        //Use get Method for Delete
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var productType = _db.ProductTypes.Find(id);
            if (productType == null) // this productType came from var productType
            {
                return NotFound();
            }
            return View(productType);
        }
        //use Post Method for Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete (int? id, ProductTypes productTypes)
        {
            if(id == null)
            {
                return NotFound();
            }
            if(id!= productTypes.Id)
            {
                return NotFound();
            }
            var productType = _db.ProductTypes.Find(id);
            if(productType.ProductType == null)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                _db.Remove(productType); //delete from DB; productType came from var 
                await _db.SaveChangesAsync(); //savechangesasync
                TempData["delete"] = "Product type has been deleted"; //show the message when delete
                return RedirectToAction(actionName: nameof(Index)); //RedirectToAction
            }
            return View(productTypes);
        }
    }
}