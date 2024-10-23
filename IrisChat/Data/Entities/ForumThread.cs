namespace IrisChat.Data.Entities
{
    public class ForumThread : SoftDeletableEntity
    {
        public string Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public bool IsLocked { get; set; }
        public bool IsBlocked { get; set; }

        public string AuthorId { get; set; }
        public User Author { get; set; }

        public string CategoryId { get; set; }
        public Category Category { get; set; }

        public ICollection<Post> Posts { get; set; } = new List<Post>();
    }
}
