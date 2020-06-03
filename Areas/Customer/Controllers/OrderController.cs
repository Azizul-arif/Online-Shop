using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineShop.Data;
using OnlineShop.Models;
using OnlineShop.Utility;

namespace OnlineShop.Areas.Customer.Controllers

{
    [Area("Customer")]
    public class OrderController : Controller
    {
        private ApplicationDbContext _db;

        public OrderController(ApplicationDbContext db)
        {
            _db = db;
        }
        //Get Checkout Method
        public IActionResult Checkout()
        {
            return View();
        }
        //Post Checkout Method
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult>CheckOut(Order anOrder)
        {
            List<Products> products = HttpContext.Session.Get<List<Products>>("products"); //collection of data retrieve from session
            if(products!=null)
            {
                foreach(var product in products)
                {
                    OrderDetails orderDetails = new OrderDetails();
                    orderDetails.PorductId = product.Id;
                    anOrder.OrderDetails.Add(orderDetails);
                }
                anOrder.OrderNo = GetOrderNo();
                _db.Orders.Add(anOrder);
                await _db.SaveChangesAsync();
                HttpContext.Session.Set("products", new List<Products>());
                return View();
            }
            return View();
        }
        public string GetOrderNo()
        {
            int rowCount = _db.Orders.ToList().Count() + 1;//to generate/count  the order no
            return rowCount.ToString("000");
        }
    }
}