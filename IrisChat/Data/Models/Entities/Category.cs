namespace IrisChat.Data.Models.Entities
{
    public class Category : SoftDeletableEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public ICollection<ForumThread> ForumThreads { get; set; }
    }
}
