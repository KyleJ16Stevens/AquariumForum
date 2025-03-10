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
    [Authorize] // Restrict access to authenticated users only
    public class DiscussionsController : Controller
    {
        private readonly AquariumContext _context;
        private readonly UserManager<User> _userManager;

        public DiscussionsController(AquariumContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Discussions (My Threads)
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            var userThreads = await _context.Discussions
                .Where(d => d.ApplicationUserId == userId)
                .OrderByDescending(d => d.CreateDate)
                .ToListAsync();

            return View(userThreads);
        }

        // GET: Discussions/Details/5
        [AllowAnonymous] // Allow anyone to view details
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var discussion = await _context.Discussions
                .Include(d => d.Comments)
                .Include(d => d.User)
                .FirstOrDefaultAsync(m => m.DiscussionId == id);

            if (discussion == null)
            {
                return NotFound();
            }

            return View(discussion);
        }

        // GET: Discussions/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Discussions/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Discussion discussion, IFormFile? imageFile)
        {
            if (ModelState.IsValid)
            {
                discussion.ApplicationUserId = _userManager.GetUserId(User);
                discussion.CreateDate = DateTime.Now;

                if (imageFile != null && imageFile.Length > 0)
                {
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    var uniqueFilename = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                    var filePath = Path.Combine(uploadsFolder, uniqueFilename);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(fileStream);
                    }

                    discussion.ImageFilename = uniqueFilename;
                }

                _context.Add(discussion);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(discussion);
        }

        // GET: Discussions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var discussion = await _context.Discussions
                .Include(d => d.User) 
                .FirstOrDefaultAsync(d => d.DiscussionId == id);

            if (discussion == null)
            {
                return NotFound();
            }

            // ensure only the owner can edit
            var loggedInUserId = _userManager.GetUserId(User);
            if (discussion.ApplicationUserId != loggedInUserId)
            {
                return Forbid(); // Prevent unauthorized access
            }

            return View(discussion);
        }


        // POST: Discussions/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Discussion discussion, IFormFile? imageFile)
        {
            if (id != discussion.DiscussionId)
            {
                return NotFound();
            }

            //Fetch the original discussion from the database
            var existingDiscussion = await _context.Discussions.FindAsync(id);

            if (existingDiscussion == null)
            {
                return NotFound();
            }

            // Ensure only the owner can edit
            var loggedInUserId = _userManager.GetUserId(User);
            if (existingDiscussion.ApplicationUserId != loggedInUserId)
            {
                return Forbid(); // Prevent unauthorized editing
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Keep the original ApplicationUserId (don't override it)
                    discussion.ApplicationUserId = existingDiscussion.ApplicationUserId;
                    discussion.CreateDate = existingDiscussion.CreateDate; // Preserve original date

                    if (imageFile != null && imageFile.Length > 0)
                    {
                        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
                        if (!Directory.Exists(uploadsFolder))
                        {
                            Directory.CreateDirectory(uploadsFolder);
                        }

                        var uniqueFilename = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                        var filePath = Path.Combine(uploadsFolder, uniqueFilename);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await imageFile.CopyToAsync(fileStream);
                        }

                        // Delete old image if it exists
                        if (!string.IsNullOrEmpty(existingDiscussion.ImageFilename))
                        {
                            var oldImagePath = Path.Combine(uploadsFolder, existingDiscussion.ImageFilename);
                            if (System.IO.File.Exists(oldImagePath))
                            {
                                System.IO.File.Delete(oldImagePath);
                            }
                        }

                        discussion.ImageFilename = uniqueFilename;
                    }
                    else
                    {
                        //Keep the original image if no new one was uploaded
                        discussion.ImageFilename = existingDiscussion.ImageFilename;
                    }

                    _context.Entry(existingDiscussion).CurrentValues.SetValues(discussion);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Discussions.Any(e => e.DiscussionId == discussion.DiscussionId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(discussion);
        }

        // GET: Discussions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var discussion = await _context.Discussions
                .FirstOrDefaultAsync(m => m.DiscussionId == id);

            if (discussion == null || discussion.ApplicationUserId != _userManager.GetUserId(User))
            {
                return Forbid();
            }

            return View(discussion);
        }

        // POST: Discussions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var discussion = await _context.Discussions.FindAsync(id);

            if (discussion == null || discussion.ApplicationUserId != _userManager.GetUserId(User))
            {
                return Forbid();
            }

            _context.Discussions.Remove(discussion);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
