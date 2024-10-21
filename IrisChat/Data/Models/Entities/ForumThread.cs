namespace IrisChat.Data.Models.Entities
{
    public class ForumThread : SoftDeletableEntity
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsLocked { get; set; }
        public bool IsBlocked { get; set; }
        public string AuthorId { get; set; }
        public User Author { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        public ICollection<Post> Posts { get; set; }
    }
}
