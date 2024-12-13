using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project1.Models
{
    public class Blog
    {
        [Key]
        public int BlogID { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Content { get; set; } = string.Empty;

        public string? UserID { get; set; }

        public string? UserEmail { get; set; }

        public DateTime DatePosted { get; set; } = DateTime.Now;

        [ForeignKey("UserID")]
        public IdentityUser? User { get; set; }

        public ICollection<Comment> Comments { get; set; } = new List<Comment>();

        // Multimedia support
        public string? ImageUrl { get; set; } // For images
        public string? VideoUrl { get; set; } // For video links
        public string? LinkUrl { get; set; }  // For external links
    }
}
