namespace IrisChat.Data.Interfaces
{
    public interface IRepository<T> where T : class
    {
        Task AddAsync(T entity);
        Task<T> GetByIdAsync(string id);
        Task UpdateAsync(T entity);
        Task SoftDeleteAsync(string id);
        Task HardDeleteAsync(string id);
        Task<IEnumerable<T>> GetAllAsync();
    }
}
