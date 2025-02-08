using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AquariumForum.Data;
using AquariumForum.Models;

namespace AquariumForum.Controllers
{
    public class CommentsController : Controller
    {
        private readonly AquariumContext _context;

        public CommentsController(AquariumContext context)
        {
            _context = context;
        }

        // GET: Comments/Create
        public IActionResult Create(int discussionId)
        {
            var comment = new Comment
            {
                DiscussionId = discussionId
            };

            return View(comment);
        }

        // POST: Comments/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Content,DiscussionId")] Comment comment)
        {
            Console.WriteLine($"Content: {comment.Content}");
            Console.WriteLine($"DiscussionId: {comment.DiscussionId}");

            if (ModelState.IsValid)
            {
                comment.CreateDate = DateTime.Now;

                _context.Add(comment);
                await _context.SaveChangesAsync();

                return RedirectToAction("Details", "Discussions", new { id = comment.DiscussionId });
            }

            foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
            {
                Console.WriteLine($"Validation Error: {error.ErrorMessage}");
            }

            return View(comment);
        }
    }
}