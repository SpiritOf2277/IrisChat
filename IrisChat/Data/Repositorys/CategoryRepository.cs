using IrisChat.Data.Entities;
using IrisChat.Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace IrisChat.Data.Repositorys
{
    public class CategoryRepository : IRepository<Category>
    {
        private readonly AppDbContext _context;

        public CategoryRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Category category)
        {
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();
        }

        public async Task<Category> GetByIdAsync(string id)
        {
            return await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task UpdateAsync(Category category)
        {
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();
        }

        public async Task SoftDeleteAsync(string id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category != null) {
                category.IsDeleted = true;
                category.DeletedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

        public async Task HardDeleteAsync(string id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category != null) {
                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Category>> GetAllAsync()
        {
            return await _context.Categories.ToListAsync();
        }
    }
}
