using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project1.Data;
using Project1.Models;
using System.Threading.Tasks;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;

    public AdminController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<IActionResult> ManageUsers()
    {
        var users = await _userManager.Users.ToListAsync();
        return View(users);
    }

    [HttpPost]
    public async Task<IActionResult> DeleteUser(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user != null)
        {
            await _userManager.DeleteAsync(user);
            TempData["SuccessMessage"] = "User deleted successfully.";
        }
        else
        {
            TempData["ErrorMessage"] = "User not found.";
        }
        return RedirectToAction("ManageUsers");
    }

    public async Task<IActionResult> ManageBlogs()
    {
        var blogs = await _context.Blogs.Include(b => b.User).ToListAsync();
        return View(blogs);
    }

    [HttpPost]
    public async Task<IActionResult> DeleteBlog(int id)
    {
        var blog = await _context.Blogs.FindAsync(id);
        if (blog != null)
        {
            _context.Blogs.Remove(blog);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Blog deleted successfully.";
        }
        else
        {
            TempData["ErrorMessage"] = "Blog not found.";
        }
        return RedirectToAction("ManageBlogs");
    }

    public async Task<IActionResult> ManageComments()
    {
        var comments = await _context.Comments.Include(c => c.Blog).ToListAsync();
        return View(comments);
    }

    [HttpPost]
    public async Task<IActionResult> DeleteComment(int id)
    {
        var comment = await _context.Comments.FindAsync(id);
        if (comment != null)
        {
            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Comment deleted successfully.";
        }
        else
        {
            TempData["ErrorMessage"] = "Comment not found.";
        }
        return RedirectToAction("ManageComments");
    }
}
