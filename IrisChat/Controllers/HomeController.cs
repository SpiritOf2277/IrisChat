using IrisChat.Data.Entities;
using IrisChat.Data.Interfaces;
using IrisChat.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace IrisChat.Controllers
{
    public class HomeController : Controller
    {
        private readonly IRepository<ForumThread> _forumThreadRepository;

        public HomeController(IRepository<ForumThread> forumThreadRepository)
        {
            _forumThreadRepository = forumThreadRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var forumThreads = await _forumThreadRepository.GetAllAsync();

            var model = forumThreads.Select(thread => new ForumThreadCardViewModel
            {
                Id = thread?.Id ?? "No ID",
                Title = thread?.Title ?? "No Title",
                Content = thread?.Content ?? "No Content",
                CategoryName = thread.Category?.Name ?? "No Category",
                CreatedAt = thread?.CreatedAt ?? DateTime.Now
            }).ToList();


            return View(model);
        }
    }
}

