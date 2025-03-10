using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AquariumForum.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;

namespace AquariumForum.Models
{
    public class Discussion
    {
        [Key] // Primary Key
        public int DiscussionId { get; set; }

        [Required]
        [StringLength(200)] // Limit title length
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }

        public string? ImageFilename { get; set; } // Nullable if no image

        [Required]
        public DateTime CreateDate { get; set; } = DateTime.Now; // Autoset date

        // 🔹 Add UserId to link discussion to its creator
        [Required]
        [ForeignKey("UserId")]
        public string? UserId { get; set; } // Make it nullable


        // 🔹 Navigation property to Identity User
        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        // Navigation property: One discussion has many comments
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    }
}
