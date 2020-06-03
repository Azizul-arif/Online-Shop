using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OnlineShop.Data;
using OnlineShop.Models;

namespace OnlineShop.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class UserController : Controller
    {
        UserManager<IdentityUser> _userManager; //UserManager is Identity class
        ApplicationDbContext _db;
        public UserController(UserManager<IdentityUser> userManager, ApplicationDbContext db) //creating constructor
        {
            _userManager = userManager;
            _db = db;
        }
        public IActionResult Index()
        {
            return View(_db.ApplicationUsers.ToList());
        }
        public async Task<IActionResult> Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(ApplicationUser user)
        {
            if (ModelState.IsValid)
            {
                var result = await _userManager.CreateAsync(user, user.PasswordHash); //Hashing the password
                if (result.Succeeded)
                {
                    var isSaveRole = await _userManager.AddToRoleAsync(user, role: "User");
                    TempData["save"] = "User Create Successfully"; //sucesss message
                    return RedirectToAction(nameof(Index));//redirect to index page
                }
                foreach (var error in result.Errors) //for validation
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View();
        }
        public async Task<IActionResult> Edit(string id)
        {
            var user = _db.ApplicationUsers.FirstOrDefault(c => c.Id == id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(ApplicationUser user)
        {
            var userInfo = _db.ApplicationUsers.FirstOrDefault(c => c.Id == user.Id);
            if (userInfo == null)
            {
                return NotFound();
            }
            userInfo.FirstName = user.FirstName;
            userInfo.LastName = user.LastName;
            var result = await _userManager.UpdateAsync(userInfo);
            if (result.Succeeded)
            {
                {
                    TempData["save"] = "User Updated Successfully"; //sucesss message
                    return RedirectToAction(nameof(Index));//redirect to index page
                }
            }
            return View();
        }
        public async Task<IActionResult> Details(string id)
        {
            var user = _db.ApplicationUsers.FirstOrDefault(c => c.Id == id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }
        public async Task<IActionResult> Lockout(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var user = _db.ApplicationUsers.FirstOrDefault(c => c.Id == id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);

        }
        [HttpPost]
        public async Task<IActionResult>Lockout(ApplicationUser user)
        {
            var userinfo = _db.ApplicationUsers.FirstOrDefault(c => c.Id == user.Id);
            if(userinfo==null)
            {
                return NotFound();
            }
            userinfo.LockoutEnd = DateTime.Now.AddYears(10);
            int rowAffected = _db.SaveChanges();
            if(rowAffected>0)
            {
                TempData["save"] = "User lockedout Successfully"; //sucesss message
                return RedirectToAction(nameof(Index));//redirect to index page
            }
            return View(userinfo);
        }
        public async Task<IActionResult>Active(string id)
        {
            var user = _db.ApplicationUsers.FirstOrDefault(c => c.Id == id);
            if(user==null)
            {
                return NotFound();
            }
            return View(user);
        }
        [HttpPost]
        public async Task<IActionResult> Active(ApplicationUser user)
        {
            var userinfo = _db.ApplicationUsers.FirstOrDefault(c => c.Id == user.Id);
            if (userinfo == null)
            {
                return NotFound();
            }
            userinfo.LockoutEnd = DateTime.Now.AddDays(-1); //Activate lockout user "null can be used insted of DateTime.Now.AddDays(-1)"
            int rowAffected = _db.SaveChanges();
            if (rowAffected > 0)
            {
                TempData["save"] = "User Active Successfully"; //sucesss message
                return RedirectToAction(nameof(Index));//redirect to index page
            }
            return View(userinfo);
        }
        public async Task<IActionResult> Delete(string id)
        {
            var user = _db.ApplicationUsers.FirstOrDefault(c => c.Id == id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }
        [HttpPost]
        public async Task<IActionResult> Delete(ApplicationUser user)
        {
            var userinfo = _db.ApplicationUsers.FirstOrDefault(c => c.Id == user.Id);
            if (userinfo == null)
            {
                return NotFound();
            }
            _db.ApplicationUsers.Remove(userinfo);
            int rowAffected = _db.SaveChanges();
            if (rowAffected > 0)
            {
                TempData["save"] = "User Delete Successfully"; //sucesss message
                return RedirectToAction(nameof(Index));//redirect to index page
            }
            return View(userinfo);
        }

    }
}