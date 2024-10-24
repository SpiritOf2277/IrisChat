using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using IrisChat.Data.Repositorys;
using IrisChat.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace IrisChat.Controllers
{
    [Authorize(Roles = "Admin, Moderator")]
    public class ModerationController : Controller
    {
        private readonly UserRepository _userRepository;

        public ModerationController(UserRepository userRepository)
        {
            _userRepository = userRepository;
        }


        public IActionResult Index()
        {
            return View();
        }


        public async Task<IActionResult> ManageUsers()
        {
            var users = await _userRepository.GetAllUsersWithDeletedAsync();
            return View(users);
        }

        [HttpPost]
        public async Task<IActionResult> BanUser(string id, int days)
        {
            await _userRepository.BanUserAsync(id, days);
            return RedirectToAction("ManageUsers");
        }

        [HttpPost]
        public async Task<IActionResult> SoftDeleteUser(string id)
        {
            await _userRepository.SoftDeleteAsync(id);
            return RedirectToAction("ManageUsers");
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> HardDeleteUser(string id)
        {
            await _userRepository.HardDeleteAsync(id);
            return RedirectToAction("ManageUsers");
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ChangeUserRole(string id, string newRole)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user != null) {
                await _userRepository.ChangeUserRoleAsync(user, "User", newRole);
            }
            return RedirectToAction("ManageUsers");
        }

        [HttpPost]
        public async Task<IActionResult> UnbanUser(string id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user != null && user.IsBanned) {
                user.IsBanned = false;
                user.BanEndDate = null;
                await _userRepository.UpdateAsync(user);
            }

            return RedirectToAction("ManageUsers");
        }

        [HttpPost]
        public async Task<IActionResult> RestoreUser(string id)
        {
            await _userRepository.RestoreUserAsync(id);
            return RedirectToAction("ManageUsers");
        }
    }
}
