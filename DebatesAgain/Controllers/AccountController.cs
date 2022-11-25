using DebatesAgain.Models;
using DebatesAgain.ViewModels;
using DebatesApp.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Claims;

namespace DebatesAgain.Controllers
{
    public class AccountController : Controller
    {
        private DebatesDataContext _db;

        public AccountController(DebatesDataContext context)
        {
            _db = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                User user = (await _db.Users
                    .Include(u => u.Role)
                    .FirstOrDefaultAsync(u => u.Email == model.Email && u.Password == model.Password))!;

                if (user != null)
                {
                    await Authenticate(user); // аутентификация

                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError("", "Некорректные логин и(или) пароль");
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                User user = (await _db.Users.FirstOrDefaultAsync(u => u.Email == model.Email))!;
                if (user == null)
                {
                    // добавляем пользователя в бд
                    user = new User { Email = model.Email, Password = model.Password };
                    Role userRole = (await _db.Roles.FirstOrDefaultAsync(r => r.Name == "viewer"))!;
                    if (userRole != null)
                        user.Role = userRole;

                    _db.Users.Add(user);
                    await _db.SaveChangesAsync();

                    await Authenticate(user); // аутентификация

                    return RedirectToAction("Index", "Home");
                }
                else
                    ModelState.AddModelError("", "Некорректные логин и(или) пароль");
            }
            return View(model);
        }

        // Аутентификация пользователя
        private async Task Authenticate(User user)
        {
            // создаем один claim
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.Email!),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, user.Role?.Name!)
            };
            // создаем объект ClaimsIdentity
            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType,
                ClaimsIdentity.DefaultRoleClaimType);
            // установка аутентификационных куки
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }

        
        [Authorize]
        [HttpGet]
        public IActionResult Properties()
        {
            return View();
        }

        
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Properties(string message)
        {
            if (message == "ChangeRoleToViewer" || message == "ChangeRoleToParticipant" || message == "ChangeRoleToModerator")
                await ChangeRoleAsync(message);

            return View();
        }


        private async Task ChangeRoleAsync(string changedRole)
        {
            string name = User.FindFirst(x => x.Type == ClaimsIdentity.DefaultNameClaimType)!.Value;
            User changeUser = _db.Users.FirstOrDefault(x => x.Email == name)!;

            Role userRole;
            switch(changedRole)
            {
                case "ChangeRoleToViewer":
                    userRole = (await _db.Roles.FirstOrDefaultAsync(r => r.Name == "viewer"))!;
                    break;
                case "ChangeRoleToParticipant":
                    userRole = (await _db.Roles.FirstOrDefaultAsync(r => r.Name == "participant"))!;
                    break;
                case "ChangeRoleToModerator":
                    userRole = (await _db.Roles.FirstOrDefaultAsync(r => r.Name == "moderator"))!;
                    break;
                default:
                    userRole = (await _db.Roles.FirstOrDefaultAsync(r => r.Name == "viewer"))!;
                    break;
            }

            changeUser.Role = userRole;
            _db.Users.Update(changeUser);
            await _db.SaveChangesAsync();
        }
    }
}
