using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OnlineShop.Data;
using OnlineShop.Models;

namespace OnlineShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SpecialTagsController : Controller
    {
        private ApplicationDbContext _db; //taking reference of ApplicationDbContext
        public SpecialTagsController(ApplicationDbContext db) //creating constructor
        {
            _db = db;
        }

        public IActionResult Index()
        {
            return View(_db.SpecialTags.ToList()); //see my product types in a list way
        }
        //get methd action for Create in SpecialTags
        public ActionResult Create()
        {
            return View();
        }
         //post methd action for Create  in SpecialTags
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task <IActionResult> Create(SpecialTags specialTags)
        {
            if(ModelState.IsValid)
            {

                //add to db
                _db.SpecialTags.Add(specialTags);
                //save to db
                await _db.SaveChangesAsync();
                //redirect to index page
                return RedirectToAction(actionName:nameof(Index));
            }
            return View(specialTags);
        }
        //get methd action for Create in SpecialTags
        public ActionResult Edit(int?id )
        {
            if(id==null)
            {
                return NotFound();
            }
            var specialTags = _db.SpecialTags.Find(id);
            if(specialTags==null)
            {
                return NotFound();
            }

            return View(specialTags);
        }
         //post methd action for Edit in SpecialTags
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int?id,SpecialTags specialTags)
        {
            if (ModelState.IsValid)
            {

                //Update/Edit to db
                _db.SpecialTags.Update(specialTags);
                //save to db
                await _db.SaveChangesAsync();
                //redirect to index page
                return RedirectToAction(actionName: nameof(Index));
            }
            return View(specialTags);
        }
        //get method for Details
        public ActionResult Details(int?id)
        {
            if(id==null)
            {
                return NotFound();
            }
            var specialTags = _db.SpecialTags.Find(id);
            if(specialTags== null)
            {
                return NotFound();
            }
            return View(specialTags);

        }
        //get action method for Delete
        public ActionResult Delete(int?id)
        {
            if(id==null)
            {
                return NotFound();
            }
            var specialTags = _db.SpecialTags.Find(id);
            if(specialTags==null)
            {
                return NotFound();
            }
            return  View (specialTags);
        }
        //post action method for delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult>Delete(int?id, SpecialTags specialTags)
        {
            if(id==null)
            {
                return NotFound();
            }
            if(id!= specialTags.Id)
            {
                return NotFound();
            }
           if(ModelState.IsValid)
            {
                //delete from db
                _db.Remove(specialTags);
                //savechanges
                await _db.SaveChangesAsync(); //savechangesasync
                //redirect to action
                return RedirectToAction(actionName: nameof(Index)); 
            }
            return View(specialTags);

        }

    }
}