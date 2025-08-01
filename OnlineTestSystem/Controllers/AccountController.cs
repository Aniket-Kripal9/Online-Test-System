using Microsoft.AspNetCore.Mvc;
using OnlineTestSystem.DataAccess;
using OnlineTestSystem.Models;

namespace OnlineTestSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly DatabaseHelper _dbHelper;

        public AccountController(IConfiguration configuration)
        {
            _dbHelper = new DatabaseHelper(configuration);
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _dbHelper.GetUserByCredentials(model.Username, model.Password);
                if (user != null)
                {
                    HttpContext.Session.SetString("UserId", user.UserId.ToString());
                    HttpContext.Session.SetString("Role", user.Role);
                    if (user.Role == "Admin")
                        return RedirectToAction("Index", "Admin");
                    else
                        return RedirectToAction("Index", "Student");
                }
                ModelState.AddModelError("", "Invalid username or password");
            }
            return View(model);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}