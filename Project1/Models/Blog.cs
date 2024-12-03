﻿namespace Project1.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Linq;

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
}
