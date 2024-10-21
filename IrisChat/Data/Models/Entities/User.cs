using Microsoft.AspNetCore.Identity;

namespace IrisChat.Data.Models.Entities
{
    public class User : IdentityUser
    {
        public string DisplayName { get; set; }
        public DateTime DateRegistered { get; set; }
        public ICollection<Thread> Threads { get; set; }
        public ICollection<Post> Posts { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsBanned { get; set; }
        public DateTime? BanEndDate { get; set; }
    }
}
