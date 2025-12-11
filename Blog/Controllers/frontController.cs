using Blog.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Blog.Controllers
{
    public class frontController : Controller
    {
        private readonly AppDbContext _db;         
        public frontController(AppDbContext db)
        {
            _db = db;           
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> GetBlogs()
        {


            var query = _db.BlogPosts
                .AsNoTracking()
                .OrderByDescending(b => b.CreatedDate)
                .AsQueryable();

            var items = await query

                .Select(b => new
                {
                    blogId = b.BlogId,
                    title = b.Title,
                    slug = b.Slug,
                    excerpt = b.Excerpt,
                    content = b.Content,
                    featureImageUrl = b.FeatureImageUrl,
                    category = b.Category,
                    tags = b.Tags,
                    isActive = b.IsActive,
                    isPublished = b.IsPublished,
                    createdBy = "Admin",
                    createdDate = b.CreatedDate,
                    updatedBy = b.UpdatedBy,
                    updatedDate = b.UpdatedDate,
                    publishedDate = b.PublishedDate
                })
                .ToListAsync();

            return Ok(items);   // ← return list directly
        }
    }
}
