using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;
using AquariumForum.Models;

namespace AquariumForum.Areas.Identity.Data
{
    public class User : IdentityUser
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(100)]
        public string Location { get; set; }

        public string? ImageFilename { get; set; } // Stores the profile image filename

        [NotMapped] // Prevents EF from adding this to the database
        public IFormFile? ImageFile { get; set; }

        public ICollection<Discussion> Discussions { get; set; } = new List<Discussion>();
    }
}