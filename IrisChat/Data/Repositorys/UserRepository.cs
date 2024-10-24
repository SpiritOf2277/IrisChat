using IrisChat.Data.Entities;
using IrisChat.Data.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace IrisChat.Data.Repositorys
{
    public class UserRepository : IRepository<User>
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public async Task<User> GetByIdAsync(string id)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task UpdateAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task SoftDeleteAsync(string id)
        {
            var user = await _context.Users
                .Include(u => u.ForumThreads)
                .Include(u => u.Posts)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user != null) {
                user.IsDeleted = true;
                user.DeletedAt = DateTime.UtcNow;

                foreach (var thread in user.ForumThreads) {
                    thread.IsDeleted = true;
                    thread.DeletedAt = DateTime.UtcNow;
                }

                foreach (var post in user.Posts) {
                    post.IsDeleted = true;
                    post.DeletedAt = DateTime.UtcNow;
                }

                await _context.SaveChangesAsync();
            }
        }


        public async Task HardDeleteAsync(string id)
        {
            var user = await _context.Users
                .Include(u => u.ForumThreads)
                .Include(u => u.Posts)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user != null) {
                _context.Posts.RemoveRange(user.Posts);

                _context.ForumThreads.RemoveRange(user.ForumThreads);

                _context.Users.Remove(user);

                await _context.SaveChangesAsync();
            }
        }


        public async Task BanUserAsync(string id, int days)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null) {
                user.IsBanned = true;
                user.BanEndDate = DateTime.UtcNow.AddDays(days);
                await _context.SaveChangesAsync();
            }
        }

        public async Task ChangeUserRoleAsync(User user, string currentRole, string newRole)
        {
            var userManager = _context.GetService<UserManager<User>>();

            if (await userManager.IsInRoleAsync(user, currentRole)) {
                await userManager.RemoveFromRoleAsync(user, currentRole);
            }
            await userManager.AddToRoleAsync(user, newRole);
        }

        public async Task<IEnumerable<User>> GetAllUsersWithDeletedAsync()
        {
            return await _context.Users.IgnoreQueryFilters().ToListAsync();
        }

        public async Task RestoreUserAsync(string id)
        {
            var user = await _context.Users
                .IgnoreQueryFilters()
                .Include(u => u.ForumThreads)
                .Include(u => u.Posts)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user != null) {
                user.IsDeleted = false;
                user.DeletedAt = null;

                _context.Entry(user).State = EntityState.Modified;

                foreach (var thread in user.ForumThreads) {
                    thread.IsDeleted = false;
                    thread.DeletedAt = null;
                    _context.Entry(thread).State = EntityState.Modified;
                }

                foreach (var post in user.Posts) {
                    post.IsDeleted = false;
                    post.DeletedAt = null;
                    _context.Entry(post).State = EntityState.Modified;
                }

                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _context.Users.ToListAsync();
        }
    }
}
