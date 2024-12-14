using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project1.Data;
using Project1.Models;
using System.IO;
using System.Threading.Tasks;

namespace Project1.Controllers
{
    public class BlogController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public BlogController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [Authorize(Roles = "Blogger")]
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [Authorize(Roles = "Reader, Blogger")]
        [Authorize]
        public async Task<IActionResult> MyComments()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                TempData["ErrorMessage"] = "Please log in to view your blogs.";
                return RedirectToAction("Login", "Account");
            }

            var userComments = await _context.Comments
                                      .Where(c => c.UserID == user.Id && c.IsApproved)
                                      .Select(c => new Comment
                                      {
                                          CommentID = c.CommentID,
                                          Blog = c.Blog,
                                          Content = c.Content,
                                          UserID = c.UserID,
                                          DatePosted = c.DatePosted,
                                          UserEmail = c.UserEmail
                                      })
                                      .OrderByDescending(c => c.DatePosted)
                                      .ToListAsync();


            return View(userComments);
        }

        [Authorize(Roles = "Reader, Blogger")]
        [HttpPost]
        public async Task<IActionResult> MyComments(int? editingCommentId)
        {
            if (editingCommentId.HasValue)
            {
                TempData["EditingCommentId"] = editingCommentId;
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                TempData["ErrorMessage"] = "Please log in to view your comments.";
                return RedirectToAction("Login", "Account");
            }

            var userComments = await _context.Comments
                                          .Include(c => c.Blog)
                                          .Where(c => c.UserID == user.Id && c.IsApproved)
                                          .OrderByDescending(c => c.DatePosted)
                                          .ToListAsync();
            return View(userComments);
        }

        [Authorize(Roles = "Reader, Blogger")]
        [HttpPost]
        public async Task<IActionResult> InlineEditC(int id, string content)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    TempData["ErrorMessage"] = "Please log in to edit a comment.";
                    return RedirectToAction("Login", "Account");
                }

                var comment = await _context.Comments.FindAsync(id);


                if (comment == null || comment.UserID != user.Id)
                {
                    TempData["ErrorMessage"] = "Comment not found or you are not authorized to edit it.";
                    return RedirectToAction("MyComments");
                }
                comment.Content = content;
                comment.DatePosted = DateTime.Now;

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Comment updated successfully!";

                return RedirectToAction("MyComments");
            }

            return RedirectToAction("MyComments");
        }

        [Authorize(Roles = "Reader, Blogger")]
        [HttpPost]
        public async Task<IActionResult> DeleteC(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                TempData["ErrorMessage"] = "Please log in to delete a comment.";
                return RedirectToAction("Login", "Account");
            }

            var comment = await _context.Comments.FindAsync(id);


            if (comment == null || comment.UserID != user.Id)
            {
                TempData["ErrorMessage"] = "Comment not found or you are not authorized to delete it.";
                return RedirectToAction("MyComments");
            }


            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Comment deleted successfully!";
            return RedirectToAction("MyComments");
        }


        [Authorize(Roles = "Blogger")]
        [HttpPost]
        public async Task<IActionResult> Create(Blog model, IFormFile? Image, bool saveAsDraft)
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
                model.UserEmail = user.Email;
                model.IsDraft = saveAsDraft;

                if (Image != null && Image.Length > 0)
                {
                    var fileName = Path.GetFileName(Image.FileName);
                    var filePath = Path.Combine("wwwroot/uploads", fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await Image.CopyToAsync(stream);
                    }

                    model.ImageUrl = $"/uploads/{fileName}";
                }

                _context.Blogs.Add(model);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = saveAsDraft ? "Blog post saved as draft!" : "Blog post created successfully!";
                return RedirectToAction("MyBlogs", "Blog");
            }

            return View(model);
        }


        [Authorize(Roles = "Blogger")]
        public async Task<IActionResult> MyBlogs()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                TempData["ErrorMessage"] = "Please log in to view your blogs.";
                return RedirectToAction("Login", "Account");
            }

            var userBlogs = await _context.Blogs
                                          .Where(b => b.UserID == user.Id && (b.IsApproved || b.IsDraft))
                                          .OrderByDescending(b => b.DatePosted)
                                          .ToListAsync();

            return View(userBlogs);
        }

        [Authorize(Roles = "Blogger")]
        [HttpPost]
        public async Task<IActionResult> MyBlogs(int? editingBlogId)
        {
            if (editingBlogId.HasValue)
            {
                TempData["EditingBlogId"] = editingBlogId;
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                TempData["ErrorMessage"] = "Please log in to view your blogs.";
                return RedirectToAction("Login", "Account");
            }

            var userBlogs = await _context.Blogs
                                          .Where(b => b.UserID == user.Id && (b.IsApproved || b.IsDraft))
                                          .OrderByDescending(b => b.DatePosted)
                                          .ToListAsync();
            return View(userBlogs);
        }
        [Authorize(Roles = "Blogger")]
        [HttpPost]
        public async Task<IActionResult> InlineEdit(int id, string title, string content, IFormFile? Image)
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

           
                if (Image != null && Image.Length > 0)
                {
                    
                    if (!string.IsNullOrEmpty(blog.ImageUrl))
                    {
                        var oldImagePath = Path.Combine("wwwroot", blog.ImageUrl.TrimStart('/'));
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    var fileName = Path.GetFileName(Image.FileName);
                    var filePath = Path.Combine("wwwroot/uploads", fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await Image.CopyToAsync(stream);
                    }

                    blog.ImageUrl = $"/uploads/{fileName}";
                }

                blog.DatePosted = DateTime.Now;

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Blog post updated successfully!";

                return RedirectToAction("MyBlogs");
            }

            return RedirectToAction("MyBlogs");
        }


        [Authorize(Roles = "Blogger")]
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


        [Authorize(Roles = "Blogger")]
        [HttpPost]
        public async Task<IActionResult> Publish(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                TempData["ErrorMessage"] = "Please log in to publish the blog.";
                return RedirectToAction("Login", "Account");
            }

            var blog = await _context.Blogs.FindAsync(id);
            if (blog == null || blog.UserID != user.Id || !blog.IsDraft)
            {
                TempData["ErrorMessage"] = "Blog not found, not authorized, or already published.";
                return RedirectToAction("MyBlogs");
            }

            blog.IsDraft = false;
            blog.DatePosted = DateTime.Now;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Blog post published successfully!";
            return RedirectToAction("MyBlogs");
        }

    }
}
