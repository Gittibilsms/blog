using System.Globalization;
using Blog.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Blog.Controllers
{
    public class frontController : Controller
    {
        private readonly AppDbContext _db;
        private const int LOW_PACKAGE_SMS = 100_000;
        private const int MEDIUM_PACKAGE_SMS = 500_000;
        private const int HIGH_PACKAGE_SMS = 1_000_000;
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

        [HttpGet]
       // [ResponseCache(Duration = 300, Location = ResponseCacheLocation.Any)]
        public async Task<IActionResult> Getpricing()
        {

            var pricing = await _db.Pricings
                .AsNoTracking()
                .Where(p => p.IsActive)
                .OrderByDescending(p => p.CreatedAt)
                .Select(p => new { p.Low, p.Middle, p.High })
                .FirstOrDefaultAsync();

            if (pricing == null)
            {
                return NotFound(new { message = "Global pricing not configured." });
            }


            var fmt = CultureInfo.InvariantCulture;

            var tiers = new[]
            {
                new PricingTierDto
                {
                    name      = "100K SMS",
                    price     = (LOW_PACKAGE_SMS    * pricing.Low   ).ToString("N0", fmt) + "₺",
                    unitPrice = pricing.Low   .ToString("0.00", fmt) + " Krş",
                    popular   = false
                },
                new PricingTierDto
                {
                    name      = "500K SMS",
                    price     = (MEDIUM_PACKAGE_SMS * pricing.Middle).ToString("N0", fmt) + "₺",
                    unitPrice = pricing.Middle.ToString("0.00", fmt) + " Krş",
                    popular   = true
                },
                new PricingTierDto
                {
                    name      = "1M SMS",
                    price     = (HIGH_PACKAGE_SMS   * pricing.High  ).ToString("N0", fmt) + "₺",
                    unitPrice = pricing.High  .ToString("0.00", fmt) + " Krş",
                    popular   = false
                }
            };

            return Ok(tiers);
        }
        public class PricingTierDto
        {
            public string name { get; set; }
            public string price { get; set; }
            public string unitPrice { get; set; }
            public bool popular { get; set; }
        }
    }
}
