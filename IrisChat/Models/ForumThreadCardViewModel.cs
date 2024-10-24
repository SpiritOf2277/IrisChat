namespace IrisChat.Models
{
    public class ForumThreadCardViewModel
        {
        public string Id { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }

        public string CategoryName { get; set; }

        public DateTime CreatedAt { get; set; }

        public string ShortTitle => Title.Length > 20 ? Title.Substring(0, 20) + "..." : Title;

        public string ShortContent => Content.Length > 300 ? Content.Substring(0, 300) + "..." : Content;

        public string ShortCategoryName => CategoryName.Length > 10 ? CategoryName.Substring(0, 10) + "..." : CategoryName;
        
    }
}
