using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace AquariumForum.Models
{
    public class Discussion
    {
        [Key] //primary Key
        public int DiscussionId { get; set; }

        [Required]
        [StringLength(200)] //limit title length
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }

        public string? ImageFilename { get; set; } //nullable if no image

        [Required]
        public DateTime CreateDate { get; set; } = DateTime.Now; //autoset date

        //navigation property one discussion has many comments
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    }
}
