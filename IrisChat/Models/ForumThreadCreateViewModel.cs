using System.ComponentModel.DataAnnotations;

namespace IrisChat.Models
{
    public class ForumThreadCreateViewModel
    {
        [Required]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }

        [Required]
        public string CategoryName { get; set; }
    }
}
