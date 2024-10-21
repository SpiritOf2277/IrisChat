namespace IrisChat.Data.Models.Entities
{
    public class Post : SoftDeletableEntity 
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsBlocked { get; set; }
        public string AuthorId { get; set; } // Foreign key
        public User Author { get; set; }
        public int ThreadId { get; set; } // Foreign key
        public ForumThread ForumThread { get; set; }
    }
}
