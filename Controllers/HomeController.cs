using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AquariumForum.Data;
using AquariumForum.Models;
using System.Linq;
using System.Threading.Tasks;

namespace AquariumForum.Controllers
{
    public class HomeController : Controller
    {
        private readonly AquariumContext _context;

        public HomeController(AquariumContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var discussions = await _context.Discussions
                .Include(d => d.User) //  Include discussion owner
                .Include(d => d.Comments) //  Load comments for count
                .OrderByDescending(d => d.CreateDate) //  Sort by newest first
                .ToListAsync();

            return View(discussions);
        }

        public async Task<IActionResult> GetDiscussion(int id)
        {
            var discussion = await _context.Discussions
                .Include(d => d.User) // Include discussion owner
                .Include(d => d.Comments)
                    .ThenInclude(c => c.User) // Include comment owners
                .FirstOrDefaultAsync(d => d.DiscussionId == id);

            if (discussion == null)
            {
                return NotFound();
            }

            //sort comments in descending order (newest first)
            discussion.Comments = discussion.Comments.OrderByDescending(c => c.CreateDate).ToList();

            return View(discussion);
        }
        public async Task<IActionResult> Profile(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var user = await _context.Users
                .Include(u => u.Discussions) 
                .ThenInclude(d => d.Comments) 
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }
    }
}
