using Microsoft.AspNetCore.Mvc;
using OrderHighLand.Models;
using OrderHighLand.Service;
using System.Reflection;

namespace OrderHighLand.Controllers.User
{
    public class AccountController : Controller
    {
        private readonly UserService _userService;
        public AccountController(UserService userService)
        {
            _userService = userService;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Detail()
        {
            return View();
        }

        public async Task<IActionResult> Register(Register model)
        {
            if (ModelState.IsValid)
            {
                var existingUser = await _userService.GetUserByEmailAsync(model.Email);
                if (existingUser != null)
                {
                    ModelState.AddModelError("Email", "Email is already registered.");
                }
                if (model.Password != model.ConfirmPassword)
                {
                    ModelState.AddModelError("ConfirmPassword", "Passwords do not match.");
                }
                if (!ModelState.IsValid)
                {
                    return View("Index", model);
                }

                await _userService.RegisterAsync(model);

                TempData["SuccessMessage"] = "Đăng ký thành công!";
                return RedirectToAction("Index", "Account");
            }

            return View("Index", model);
        }


        public async Task<IActionResult> Login(string email, string password)
        {

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                TempData["ErrorMessage"] = "Email and Password are required.";
                return View("Index"); 
            }


            var user = await _userService.LoginAsync(email, password);
            if (user != null)
            {
                HttpContext.Session.SetString("UserId", user.A_ID.ToString());
                HttpContext.Session.SetString("UserName", user.A_NAME);
                HttpContext.Session.SetString("UserEmail", user.A_EMAIL);
                HttpContext.Session.SetString("RoleId", user.ROLE_ID.ToString());
                if (user.ROLE_ID == 1) 
                {
                    return RedirectToAction("Index", "AdminDashboard");
                }
                else if (user.ROLE_ID == 2)
                {
                    return RedirectToAction("Index", "Home");
                }
                return RedirectToAction("Index", "Home");
            }

            TempData["ErrorMessage"] = "Email or password is incorrect.";
            return View("Index");
        }

        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}
