namespace IrisChat.Data.Entities
{
    public class Post : SoftDeletableEntity
    {
        public string Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public bool IsBlocked { get; set; }

        public string AuthorId { get; set; }
        public User Author { get; set; }

        public string ThreadId { get; set; }
        public ForumThread ForumThread { get; set; }
    }
}
