using IrisChat.Data.Entities;

namespace IrisChat.Models
{
    public class HomeViewModel
    {
        public IEnumerable<Category> Categories { get; set; }
        public IEnumerable<ForumThread> ForumThreads { get; set; }
    }
}
