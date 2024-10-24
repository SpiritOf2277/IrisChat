using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using IrisChat.Data.Entities;
using System.Threading.Tasks;
using IrisChat.Models;
using Microsoft.EntityFrameworkCore;

public class AccountController : Controller
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;

    public AccountController(UserManager<User> userManager, SignInManager<User> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    [HttpGet]
    public IActionResult Login() => View();

    [HttpPost]
    public async Task<IActionResult> Login(string email, string password, bool rememberMe)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user != null) {
            if (user.IsDeleted) {
                ModelState.AddModelError(string.Empty, "Your account has been deleted.");
                return View();
            }

            if (user.IsBanned && (!user.BanEndDate.HasValue || user.BanEndDate > DateTime.UtcNow)) {
                ModelState.AddModelError(string.Empty, "Your account is banned.");
                return View();
            }

            var result = await _signInManager.PasswordSignInAsync(email, password, rememberMe, false);

            if (result.Succeeded) {
                return RedirectToAction("Index", "Home");
            } else {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return View();
            }
        }

        ModelState.AddModelError(string.Empty, "User not found.");
        return View();
    }

    [HttpGet]
    public IActionResult Register() => View(new RegisterViewModel());

    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (ModelState.IsValid) {
            // Проверка: существует ли email
            var userWithSameEmail = await _userManager.Users.IgnoreQueryFilters()
            .FirstOrDefaultAsync(u => u.Email == model.Email);
            if (userWithSameEmail != null) {
                ModelState.AddModelError("Email", "This email is already registered.");
            }

            // Проверка: существует ли UserName
            var userWithSameUserName = await _userManager.Users.IgnoreQueryFilters()
            .FirstOrDefaultAsync(u => u.UserName == model.Email);
            if (userWithSameUserName != null) {
                ModelState.AddModelError("UserName", "This username (email) is already taken.");
            }

            if (ModelState.IsValid) {
                var user = new User
                {
                    UserName = model.Email,
                    Email = model.Email,
                    DisplayName = model.DisplayName,
                    DateRegistered = DateTime.UtcNow
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded) {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }

                foreach (var error in result.Errors) {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
        }

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }
}
