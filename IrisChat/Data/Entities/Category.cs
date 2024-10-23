namespace IrisChat.Data.Entities
{
    public class Category : SoftDeletableEntity
    {
        public string Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public ICollection<ForumThread> ForumThreads { get; set; } = new List<ForumThread>();
    }
}
