using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AquariumForum.Areas.Identity.Data;

namespace AquariumForum.Models
{
    public class Comment
    {
        [Key] // Primary Key
        public int CommentId { get; set; }

        [Required]
        public string Content { get; set; }

        [Required]
        public DateTime CreateDate { get; set; } = DateTime.Now; // Auto-set date

        // Foreign Key to Discussion 
        [Required]
        public int DiscussionId { get; set; }

        [ForeignKey("DiscussionId")]
        public virtual Discussion? Discussion { get; set; }

        //  Foreign Key to User 
        public string? ApplicationUserId { get; set; }

        [ForeignKey("ApplicationUserId")]
        public virtual User? User { get; set; } // Navigation property
    }
}