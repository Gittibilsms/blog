using Blog.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using  Blog.Models;

public class BlogController : Controller
{
    private readonly AppDbContext _db;

    public BlogController(AppDbContext db) => _db = db;

    // List all public blog posts (admin can access too)
    public async Task<IActionResult> Index()
    {
        var featuredPost = await _db.BlogPosts
        .Where(b => b.IsActive && b.IsPublished)
        .OrderByDescending(b => b.PublishedDate ?? b.CreatedDate)
        .FirstOrDefaultAsync();

        // Fetch the trending blog posts and project them to BlogListItem
        var trendingPosts = await _db.BlogPosts
            .Where(b => b.IsActive && b.IsPublished)
            .OrderByDescending(b => b.PublishedDate ?? b.CreatedDate)
            .Take(5) // Adjust the number for trending posts
            .Select(b => new BlogListItem
            {
                BlogId = b.BlogId,
                Title = b.Title,
                Slug = b.Slug,
                Excerpt = b.Excerpt,
                FeatureImageUrl = b.FeatureImageUrl,
                Category = b.Category,
                PublishedDate = b.PublishedDate ?? b.CreatedDate,
                ReadMinutes = 10
            })
            .ToListAsync();

        // Prepare the view model
        var viewModel = new BlogListViewModel
        {
            Featured = featuredPost,
            Trending = trendingPosts  // Now Trending is a List<BlogListItem>
        };

        return View(viewModel);
    }

    // Show specific blog post (public)
    public async Task<IActionResult> Details(string slug)
    {
        var post = await _db.BlogPosts
             .Where(b => b.Slug == slug && b.IsActive && b.IsPublished)
             .FirstOrDefaultAsync();

        if (post == null)
        {
            return NotFound();  
        }
        int adminId = 0;
        _ = int.TryParse(post.CreatedBy, out adminId); // CreatedBy is your stored ID string

        var createdByUsername = await _db.AdminUsers
            .Where(a => a.AdminId == adminId)
            .Select(a => a.Username)
            .FirstOrDefaultAsync() ?? "(unknown)";
        var viewModel = new BlogPostViewModel
        {
            Title = post.Title,
            Slug = post.Slug,
            Excerpt = post.Excerpt,
            Content = post.Content,
            FeatureImageUrl = post.FeatureImageUrl,
            Category = post.Category,
            Tags = post.Tags?.Split(',') ?? new string[] { },  // Assuming Tags is a CSV of tags
            IsActive = post.IsActive,
            IsPublished = post.IsPublished,
            CreatedBy = createdByUsername,
            CreatedDate = post.CreatedDate,
            UpdatedBy = post.UpdatedBy,
            UpdatedDate = post.UpdatedDate,
            PublishedDate = post.PublishedDate,
            ReadMinutes = post.Content.Length / 250 // Approximate reading time, adjust as needed
        };

        return View(viewModel);  // Pass the ViewModel to the Details view
    } 
}
