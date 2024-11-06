namespace Project1.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project1.Data;
using Project1.Models;
using System.Threading.Tasks;


public class BlogController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;

    public BlogController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [Authorize]
    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create(Blog model)
    {
        if (ModelState.IsValid)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                TempData["ErrorMessage"] = "Unable to create blog post. Please make sure you are logged in.";
                return RedirectToAction("Login", "Account");
            }

           
            model.UserID = user.Id;
            _context.Blogs.Add(model);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Blog post created successfully!";
            return RedirectToAction("Create", "Blog");
        }

        return View(model);
    }

    [Authorize]
    public async Task<IActionResult> MyBlogs()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            TempData["ErrorMessage"] = "Please log in to view your blogs.";
            return RedirectToAction("Login", "Account");
        }

        var userBlogs = await _context.Blogs
                                      .Where(b => b.UserID == user.Id)
                                      .OrderByDescending(b => b.DatePosted)
                                      .ToListAsync();

        return View(userBlogs);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            TempData["ErrorMessage"] = "Please log in to delete a blog post.";
            return RedirectToAction("Login", "Account");
        }

        var blog = await _context.Blogs.FindAsync(id);

     
        if (blog == null || blog.UserID != user.Id)
        {
            TempData["ErrorMessage"] = "Blog post not found or you are not authorized to delete it.";
            return RedirectToAction("MyBlogs");
        }

 
        _context.Blogs.Remove(blog);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Blog post deleted successfully!";
        return RedirectToAction("MyBlogs");
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> InlineEdit(int id, string title, string content)
    {
        if (ModelState.IsValid)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                TempData["ErrorMessage"] = "Please log in to edit a blog post.";
                return RedirectToAction("Login", "Account");
            }

            var blog = await _context.Blogs.FindAsync(id);

            
            if (blog == null || blog.UserID != user.Id)
            {
                TempData["ErrorMessage"] = "Blog post not found or you are not authorized to edit it.";
                return RedirectToAction("MyBlogs");
            }

           
            blog.Title = title;
            blog.Content = content;
            blog.DatePosted = DateTime.Now;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Blog post updated successfully!";
            return RedirectToAction("MyBlogs");
        }

        return RedirectToAction("MyBlogs");
    }










}
