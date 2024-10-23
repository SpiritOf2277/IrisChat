using IrisChat.Data.Entities;
using IrisChat.Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace IrisChat.Data.Repositorys
{
    public class PostRepository : IRepository<Post>
    {
        private readonly AppDbContext _context;

        public PostRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Post post)
        {
            await _context.Posts.AddAsync(post);
            await _context.SaveChangesAsync();
        }

        public async Task<Post> GetByIdAsync(string id)
        {
            return await _context.Posts.FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task UpdateAsync(Post post)
        {
            _context.Posts.Update(post);
            await _context.SaveChangesAsync();
        }

        public async Task SoftDeleteAsync(string id)
        {
            var post = await _context.Posts.FindAsync(id);
            if (post != null) {
                post.IsDeleted = true;
                post.DeletedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

        public async Task HardDeleteAsync(string id)
        {
            var post = await _context.Posts.FindAsync(id);
            if (post != null) {
                _context.Posts.Remove(post);
                await _context.SaveChangesAsync();
            }
        }
    }
}
