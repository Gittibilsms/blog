using Blog.Data;
using Microsoft.AspNetCore.Mvc;
using System;

namespace Blog.Controllers
{    
    public class AdminController : Controller
    {
        private readonly AppDbContext _db;
        public AdminController(AppDbContext db) {
            _db = db;
        }
        // GET: /Admin/Login
        [HttpGet]
        public IActionResult Login()
        {
            return View();   // returns Views/Admin/Login.cshtml
        }
        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            var admin = _db.AdminUsers.FirstOrDefault(a => a.Username == username && a.PasswordHash == password && a.IsActive);
            if (admin != null)
            {
                HttpContext.Session.SetString("AdminUser", admin.AdminId.ToString());
                return RedirectToAction("Index", "BlogAdmin");
            }
            ViewBag.Error = "Invalid credentials";
            return View();
        }
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            // return RedirectToAction("Login");
            return RedirectToAction("Index", "BlogAdmin");
        }
    }
}
