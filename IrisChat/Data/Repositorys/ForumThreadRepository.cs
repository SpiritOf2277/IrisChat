using IrisChat.Data.Interfaces;
using IrisChat.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace IrisChat.Data.Repositorys
{
    public class ForumThreadRepository : IRepository<ForumThread>
    {
        private readonly AppDbContext _context;

        public ForumThreadRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(ForumThread thread)
        {
            await _context.ForumThreads.AddAsync(thread);
            await _context.SaveChangesAsync();
        }

        public async Task<ForumThread> GetByIdAsync(string id)
        {
            return await _context.ForumThreads.FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task UpdateAsync(ForumThread thread)
        {
            _context.ForumThreads.Update(thread);
            await _context.SaveChangesAsync();
        }

        public async Task SoftDeleteAsync(string id)
        {
            var thread = await _context.ForumThreads.FindAsync(id);
            if (thread != null) {
                thread.IsDeleted = true;
                thread.DeletedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

        public async Task HardDeleteAsync(string id)
        {
            var thread = await _context.ForumThreads.FindAsync(id);
            if (thread != null) {
                _context.ForumThreads.Remove(thread);
                await _context.SaveChangesAsync();
            }
        }
    }
}
