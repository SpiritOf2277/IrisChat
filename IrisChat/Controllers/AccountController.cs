using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using IrisChat.Data.Entities;
using System.Threading.Tasks;
using IrisChat.Models;

public class AccountController : Controller
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;

    public AccountController(UserManager<User> userManager, SignInManager<User> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    // Страница входа
    [HttpGet]
    public IActionResult Login() => View();

    // Процесс входа пользователя
    [HttpPost]
    public async Task<IActionResult> Login(string email, string password, bool rememberMe)
    {
        // Попробуем найти пользователя по email
        var user = await _userManager.FindByEmailAsync(email);

        if (user != null) {
            // Проверка на мягкое удаление
            if (user.IsDeleted) {
                ModelState.AddModelError(string.Empty, "Your account has been deleted.");
                return View();
            }

            // Проверка, забанен ли пользователь, и истек ли бан
            if (user.IsBanned && (!user.BanEndDate.HasValue || user.BanEndDate > DateTime.UtcNow)) {
                ModelState.AddModelError(string.Empty, "Your account is banned.");
                return View();
            }

            // Если пользователь не удален и не забанен, продолжаем процесс входа
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

    // Страница регистрации
    [HttpGet]
    public IActionResult Register() => View(new RegisterViewModel());

    // Процесс регистрации
    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (ModelState.IsValid) {
            // Создаем нового пользователя с дополнительными полями
            var user = new User
            {
                UserName = model.Email,
                Email = model.Email,
                DisplayName = model.DisplayName,
                DateRegistered = DateTime.UtcNow
            };

            // Создаем пользователя и задаем ему пароль
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded) {
                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Index", "Home");
            }

            // Если произошли ошибки при создании пользователя, выводим их
            foreach (var error in result.Errors) {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        return View(model);
    }

    // Процесс выхода пользователя
    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }
}
