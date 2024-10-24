using IrisChat.Data;
using IrisChat.Data.Entities;
using IrisChat.Data.Interfaces;
using IrisChat.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace IrisChat.Controllers
{
    [Authorize]
    public class ForumThreadController : Controller
    {
        private readonly IRepository<ForumThread> _forumThreadRepository;
        private readonly IRepository<Category> _categoryRepository;
        private readonly IRepository<Post> _postRepository;
        private readonly AppDbContext _context;

        public ForumThreadController(
            IRepository<ForumThread> forumThreadRepository,
            IRepository<Category> categoryRepository,
            IRepository<Post> postRepository,
            AppDbContext context)
        {
            _forumThreadRepository = forumThreadRepository;
            _categoryRepository = categoryRepository;
            _postRepository = postRepository;
            _context = context;
        }


        [HttpGet]
        public IActionResult Create()
        {
            return View(new ForumThreadCreateViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Create(ForumThreadCreateViewModel model)
        {
            if (ModelState.IsValid) {
                using (var transaction = await _context.Database.BeginTransactionAsync()) {
                    try {
                        var existingCategory = await _context.Categories
                            .FirstOrDefaultAsync(c => c.Name.ToLower() == model.CategoryName.ToLower());

                        if (existingCategory == null) {
                            existingCategory = new Category
                            {
                                Id = Guid.NewGuid().ToString(),
                                Name = model.CategoryName
                            };
                            await _categoryRepository.AddAsync(existingCategory);
                        }

                        var newForumThread = new ForumThread
                        {
                            Id = Guid.NewGuid().ToString(),
                            Title = model.Title,
                            Content = model.Content,
                            CategoryId = existingCategory.Id,
                            AuthorId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value,
                            CreatedAt = DateTime.UtcNow,
                            IsLocked = false,
                            IsBlocked = false
                        };

                        await _forumThreadRepository.AddAsync(newForumThread);

                        await transaction.CommitAsync();

                        return RedirectToAction("Index", "Home");
                    } catch {
                        await transaction.RollbackAsync();
                        ModelState.AddModelError("", "An error occurred while creating the thread.");
                    }
                }
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Details(string id)
        {
            var thread = await _context.ForumThreads
                .Include(t => t.Author)
                .Include(t => t.Posts)
                    .ThenInclude(p => p.Author)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (thread == null) {
                return NotFound();
            }

            var model = new ForumThreadDetailsViewModel
            {
                Id = thread.Id,
                Title = thread.Title,
                Content = thread.Content,
                CreatedAt = thread.CreatedAt,
                AuthorName = thread.Author?.UserName ?? "Unknown",
                Posts = thread.Posts.Select(p => new PostViewModel
                {
                    Content = p.Content,
                    CreatedAt = p.CreatedAt,
                    AuthorName = p.Author?.UserName ?? "Unknown"
                }).ToList()
            };

            return View(model);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddPost(PostCreateViewModel model)
        {
            if (ModelState.IsValid) {
                var newPost = new Post
                {
                    Id = Guid.NewGuid().ToString(),
                    Content = model.Content,
                    ThreadId = model.ForumThreadId,
                    AuthorId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value,
                    CreatedAt = DateTime.UtcNow
                };

                await _postRepository.AddAsync(newPost);

                return RedirectToAction("Details", new { id = model.ForumThreadId });
            }

            return RedirectToAction("Details", new { id = model.ForumThreadId });
        }
    }
}