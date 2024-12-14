using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Project1.Models
{
    public class Comment
    {
        [Key]
        public int CommentID { get; set; }

        [Required]
        [StringLength(1000, ErrorMessage = "Comment cannot exceed 1000 characters.")]
        public string Content { get; set; } = string.Empty;

        public DateTime DatePosted { get; set; } = DateTime.Now;

        [Required]
        public int BlogID { get; set; } 

        [ForeignKey("BlogID")]
        public Blog? Blog { get; set; }

        public string? UserID { get; set; }

        [ForeignKey("UserID")]
        public IdentityUser? User { get; set; }

        public string? UserEmail { get; set; }

        public int? ParentCommentID { get; set; } 
        [ForeignKey("ParentCommentID")]
        public virtual Comment? ParentComment { get; set; }

        public virtual ICollection<Comment> Replies { get; set; } = new List<Comment>();
    }
}
