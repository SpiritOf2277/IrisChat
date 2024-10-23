using Microsoft.AspNetCore.Mvc;

namespace IrisChat.Controllers
{
    public class ModerationController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
