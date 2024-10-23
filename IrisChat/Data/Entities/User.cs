using Microsoft.AspNetCore.Identity;

namespace IrisChat.Data.Entities
{
    public class User : IdentityUser
    {
        public string DisplayName { get; set; } = string.Empty;
        public DateTime DateRegistered { get; set; }

        public ICollection<ForumThread> ForumThreads { get; set; } = new List<ForumThread>();
        public ICollection<Post> Posts { get; set; } = new List<Post>();

        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
        public bool IsBanned { get; set; }
        public DateTime? BanEndDate { get; set; }


    }
}
