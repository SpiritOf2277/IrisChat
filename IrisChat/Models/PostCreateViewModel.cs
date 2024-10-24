using System.ComponentModel.DataAnnotations;

namespace IrisChat.Models
{
    public class PostCreateViewModel
    {
        [Required]
        public string Content { get; set; }

        [Required]
        public string ForumThreadId { get; set; }
    }
}
