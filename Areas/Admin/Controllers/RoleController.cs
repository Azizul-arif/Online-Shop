using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using OnlineShop.Areas.Admin.Models;
using OnlineShop.Data;

namespace OnlineShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class RoleController : Controller
    {
        RoleManager<IdentityRole> _roleManager;//RoleManager is built-in class in identity framework
        UserManager<IdentityUser> _userManager; //UserManager is Identity class (bring usermanager object to assign role to the user)
        ApplicationDbContext _db;
        public RoleController(RoleManager<IdentityRole> roleManager, ApplicationDbContext db, UserManager<IdentityUser> userManager) //crete a constructor
        {
            _roleManager = roleManager;
            _db = db;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            var roles = _roleManager.Roles.ToList();
            ViewBag.Roles = roles;
            return View();
        }
        public async Task<IActionResult> Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(string name) //name parameter came from views name identifier
        {
            IdentityRole role = new IdentityRole();
            role.Name = name;
            var isExist = await _roleManager.RoleExistsAsync(role.Name); //check for existing role
            if (isExist)
            {
                ViewBag.name = name; //name came from html input type name="name"
                ViewBag.message = " Can Not Update " + ViewBag.name + " Role Already Exist";
                return View();
            }
            var result = await _roleManager.CreateAsync(role);
            if (result.Succeeded)
            {
                TempData["save"] = "Role Save Successfully"; //sucesss message
                return RedirectToAction(nameof(Index));//redirect to index page
            }
            return View();
        }
        public async Task<IActionResult> Edit(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
            {
                return NotFound();
            }
            ViewBag.id = role.Id; //get id of role
            ViewBag.name = role.Name; //get name of role
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Edit(string id, string name) //name parameter came from views name identifier
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
            {
                return NotFound();
            }
            role.Name = name;
            var isExist = await _roleManager.RoleExistsAsync(role.Name); //check for existing role
            if (isExist)
            {
                ViewBag.name = name; //name came from html input type name="name"
                ViewBag.message = " Can Not Update: " + ViewBag.name + " :Role Already Exist";
                //ViewBag.name = name; //name came from html input type name="name"
                return View();
            }
            var result = await _roleManager.UpdateAsync(role);
            if (result.Succeeded)
            {
                TempData["edit"] = "Role Updated Successfully"; //sucesss message
                return RedirectToAction(nameof(Index));//redirect to index page
            }
            return View();
        }
        //delete get method
        public async Task<IActionResult> Delete(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
            {
                return NotFound();
            }
            ViewBag.id = role.Id; //get id of role
            ViewBag.name = role.Name; //get name of role
            return View();
        }
        [HttpPost]
        [ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirm(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
            {
                return NotFound();
            }
            var result = await _roleManager.DeleteAsync(role);
            if (result.Succeeded)
            {
                TempData["delete"] = "Role Deleted Successfully"; //sucesss message
                return RedirectToAction(nameof(Index));//redirect to index page
            }
            return View();
        }
        //role assign to user
        public async Task<IActionResult> Assign()
        {
            ViewData[("UserId")] = new SelectList(_db.ApplicationUsers.Where(c => c.LockoutEnd < DateTime.Now || c.LockoutEnd == null).ToList(), "Id", "UserName"); //dropdown data bind and check for disable lockout user
            //ViewData[("RoleId")] = new SelectList(_roleManager.Roles.ToList(),"Id","Name"); //dropdown data bind
            ViewData[("RoleId")] = new SelectList(_roleManager.Roles.ToList(), "Name", "Name"); //dropdown data bind
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Assign(RoleUserVm roleUser)
        {
            var user = _db.ApplicationUsers.FirstOrDefault(c => c.Id == roleUser.UserId);
            var IsCheckRoleAssign = await _userManager.IsInRoleAsync(user, roleUser.RoleId); //check if the role assign previously or not
            if (IsCheckRoleAssign)
            {
                ViewBag.message = "Already Assigned";
                ViewData[("UserId")] = new SelectList(_db.ApplicationUsers.Where(c => c.LockoutEnd < DateTime.Now || c.LockoutEnd == null).ToList(), "Id", "UserName"); //dropdown data bind and check for disable lockout user
                //ViewData[("RoleId")] = new SelectList(_roleManager.Roles.ToList(),"Id","Name"); //dropdown data bind
                ViewData[("RoleId")] = new SelectList(_roleManager.Roles.ToList(), "Name", "Name"); //dropdown data bind
                return View();
            }
            var role = await _userManager.AddToRoleAsync(user, roleUser.RoleId);
            if (role.Succeeded)
            {
                TempData["save"] = "Role Assign Successfully"; //sucesss message
                return RedirectToAction(nameof(Index));//redirect to index page
            }
            return View();
        }
        //method for user role linq reporting
        public ActionResult AssignUserRole()
        {
            var result = from ur in _db.UserRoles
                         join r in _db.Roles on ur.RoleId equals r.Id
                         join a in _db.ApplicationUsers on ur.UserId equals a.Id
                         select new UserRoleMaping()
                         {
                             UserId = ur.UserId,
                             RoleId = ur.RoleId,
                             UserName = a.UserName,
                             RoleName = r.Name
                         };
            ViewBag.UserRoles = result;
            return View();
        }

    }
}