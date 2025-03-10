using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using AquariumForum.Data;
using AquariumForum.Models;
using AquariumForum.Areas.Identity.Data;

namespace AquariumForum.Controllers
{
    [Authorize] // Require authentication for all actions
    public class CommentsController : Controller
    {
        private readonly AquariumContext _context;
        private readonly UserManager<User> _userManager;

        public CommentsController(AquariumContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
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
            if (ModelState.IsValid)
            {
                comment.CreateDate = DateTime.Now;
                comment.ApplicationUserId = _userManager.GetUserId(User); // Assign logged-in user ID

                _context.Add(comment);
                await _context.SaveChangesAsync();

                return RedirectToAction("GetDiscussion", "Home", new { id = comment.DiscussionId });
            }

            return View(comment);
        }

        // GET: Comments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var comment = await _context.Comments
                .FirstOrDefaultAsync(c => c.CommentId == id);

            if (comment == null || comment.ApplicationUserId != _userManager.GetUserId(User))
            {
                return Forbid(); // Prevent unauthorized deletion
            }

            return View(comment);
        }

        // POST: Comments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var comment = await _context.Comments.FindAsync(id);

            if (comment == null || comment.ApplicationUserId != _userManager.GetUserId(User))
            {
                return Forbid();
            }

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();
            return RedirectToAction("GetDiscussion", "Home", new { id = comment.DiscussionId });
        }
    }
}
