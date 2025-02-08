using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AquariumForum.Models
{
    public class Comment
    {
        [Key] //primary Key
        public int CommentId { get; set; }

        [Required]
        public string Content { get; set; }

        [Required]
        public DateTime CreateDate { get; set; } = DateTime.Now; //auto-set date

        //foreign Key to Discussion
        [Required]
        public int DiscussionId { get; set; }

        //navigation property many comments belong to one discussion
        [ForeignKey("DiscussionId")]
        public Discussion? Discussion { get; set; }
    }
}
