using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project1.Data;
using Project1.Models;
using System.Diagnostics;

namespace Project1.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public HomeController(ApplicationDbContext context, ILogger<HomeController> logger, UserManager<IdentityUser> userManager)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task<IActionResult> Index(string searchString, DateTime? startDate, DateTime? endDate)
        {
            var lowerSearchString = searchString?.ToLower();
            var query = _context.Blogs
                                .Include(b => b.User)
                                .Include(b => b.Comments)
                                .ThenInclude(c => c.User)
                                .AsQueryable();

        
            if (!string.IsNullOrWhiteSpace(searchString))
            {
                query = query.Where(b => b.Title.ToLower().Contains(lowerSearchString) || b.Content.ToLower().Contains(lowerSearchString));
            }

         
            if (startDate.HasValue)
            {
                query = query.Where(b => b.DatePosted >= startDate.Value);
            }

            if (endDate.HasValue)
            {
               
                var adjustedEndDate = endDate.Value.AddDays(1).AddTicks(-1);
                query = query.Where(b => b.DatePosted <= adjustedEndDate);
            }

            var allBlogs = await query.OrderByDescending(b => b.DatePosted).ToListAsync();

            ViewData["SearchString"] = searchString;
            ViewData["StartDate"] = startDate?.ToString("yyyy-MM-dd");
            ViewData["EndDate"] = endDate?.ToString("yyyy-MM-dd");

            return View(allBlogs);
        }



        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddComment(int blogId, string content)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                TempData["ErrorMessage"] = "You need to log in to post a comment.";
                return RedirectToAction("Login", "Account");
            }

            if (string.IsNullOrWhiteSpace(content))
            {
                TempData["ErrorMessage"] = "Comment cannot be empty.";
                return RedirectToAction("Details", new { id = blogId });
            }

            var comment = new Comment
            {
                Content = content,
                BlogID = blogId,
                UserID = user.Id,
                UserEmail = user.Email
            };

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Comment added successfully.";
            return RedirectToAction("Index");  
        }

        public async Task<IActionResult> Details(int id)
        {
            var blog = await _context.Blogs
                                     .Include(b => b.User)
                                     .Include(b => b.Comments)
                                     .ThenInclude(c => c.User)
                                     .FirstOrDefaultAsync(b => b.BlogID == id);

            if (blog == null)
            {
                return NotFound();
            }

            return View(blog);
        }
    }
}
