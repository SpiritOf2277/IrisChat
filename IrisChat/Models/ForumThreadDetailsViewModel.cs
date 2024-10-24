using System;
using System.Collections.Generic;

namespace IrisChat.Models
{
    public class ForumThreadDetailsViewModel
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public string AuthorName { get; set; }

        public List<PostViewModel> Posts { get; set; } = new List<PostViewModel>();
    }

    public class PostViewModel
    {
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public string AuthorName { get; set; }
    }
}
