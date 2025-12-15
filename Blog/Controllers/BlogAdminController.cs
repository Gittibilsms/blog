using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Blog.Data;
using Blog.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
namespace Blog.Controllers
{
    public class BlogAdminController : Controller
    {
        private readonly AppDbContext _db;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public BlogAdminController(AppDbContext db, IWebHostEnvironment env)
        {
            _db = db;
            _webHostEnvironment = env;
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();   // returns Views/Admin/Login.cshtml
        }
        private bool IsAuthed() => !string.IsNullOrEmpty(HttpContext.Session.GetString("AdminUser"));
        private IActionResult RequireAuth() => RedirectToAction("Login", "Admin");

        public IActionResult Index() => IsAuthed() ? View() : RequireAuth();

        [HttpGet]
        public async Task<IActionResult> GetBlogs(int page = 1, int pageSize = 10, string? q = null)
        {
            if (!IsAuthed()) return Unauthorized();

            var query = _db.BlogPosts.AsNoTracking().OrderByDescending(b => b.CreatedDate).AsQueryable();
            if (!string.IsNullOrWhiteSpace(q))
                query = query.Where(b => b.Title.Contains(q) || b.Slug.Contains(q));

            var total = await query.CountAsync();
            var data = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(b => new {
                    b.BlogId,
                    b.Title,
                    b.Slug,
                    b.IsPublished,
                    b.IsActive,
                    b.CreatedBy,
                    b.CreatedDate
                })
                .ToListAsync();

            return Json(new { total, data });
        }

        [HttpGet]
        public IActionResult Create() => IsAuthed() ? View(new BlogPost()) : RequireAuth();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BlogPost model)
        {
            if (!IsAuthed()) return RequireAuth();
            if (!ModelState.IsValid) return View(model);

            model.CreatedBy = HttpContext.Session.GetString("AdminUser")!;
            model.CreatedDate = DateTime.UtcNow;           
            model.PublishedDate = DateTime.UtcNow;
            model.UpdatedBy = HttpContext.Session.GetString("AdminUser")!;
            model.UpdatedDate = DateTime.UtcNow;

            _db.BlogPosts.Add(model);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        public async Task<IActionResult> UploadImage(IFormFile upload)
        {
            if (upload == null || upload.Length == 0)
                return BadRequest(new { error = "No file uploaded" });

            var allowedExt = new[] { ".jpg", ".jpeg", ".png", ".webp", ".gif" };
            var ext = Path.GetExtension(upload.FileName).ToLowerInvariant();
            if (!allowedExt.Contains(ext))
                return BadRequest(new { error = "Only image files are allowed" });

            if (upload.Length > 5 * 1024 * 1024)
                return BadRequest(new { error = "Max 5MB" });

            var cfg = HttpContext.RequestServices.GetRequiredService<IConfiguration>();

            // Get connection string from Key Vault (via configuration)
            var connStr = cfg["AzureStorageConnectionString"]; // This now comes from Key Vault!
            var containerName = cfg["AzureBlob:ContainerName"];
            var publicBaseUrl = cfg["AzureBlob:PublicBaseUrl"]; // optional

            var blobServiceClient = new BlobServiceClient(connStr);
            var containerClient = blobServiceClient.GetBlobContainerClient(containerName);

            // Create container if it doesn't exist (safe)
            await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);

            var fileName = $"{Guid.NewGuid()}{ext}";
            var blobClient = containerClient.GetBlobClient(fileName);

            // Upload
            using (var stream = upload.OpenReadStream())
            {
                await blobClient.UploadAsync(stream, new BlobHttpHeaders
                {
                    ContentType = upload.ContentType
                });
            }

            var url = !string.IsNullOrWhiteSpace(publicBaseUrl)
                ? $"{publicBaseUrl}/{fileName}"
                : blobClient.Uri.ToString();

            return Json(new { url });
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (!IsAuthed()) return RequireAuth();
            var post = await _db.BlogPosts.FindAsync(id);
            return post == null ? NotFound() : View(post);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, BlogPost model)
        {
            if (!IsAuthed()) return RequireAuth();
            if (id != model.BlogId) return BadRequest();
            if (!ModelState.IsValid) return View(model);

            var existing = await _db.BlogPosts.FindAsync(id);
            if (existing == null) return NotFound();

            existing.Title = model.Title;
            existing.Slug = model.Slug;
            existing.Excerpt = model.Excerpt;
            existing.Content = model.Content;
            existing.FeatureImageUrl = model.FeatureImageUrl;
            existing.Category = model.Category;
            existing.Tags = model.Tags;
            existing.IsActive = model.IsActive;
            existing.IsPublished = model.IsPublished;
            existing.UpdatedBy = HttpContext.Session.GetString("AdminUser");
            existing.UpdatedDate = DateTime.UtcNow;
            if (existing.IsPublished && existing.PublishedDate == null)
                existing.PublishedDate = DateTime.UtcNow;

            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        
    }
}
