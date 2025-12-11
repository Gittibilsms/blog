using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Blog.Data;
using System.Text;
using System.Xml.Linq;

[Route("sitemap.xml")]
public class SitemapController : Controller
{
    private readonly AppDbContext _db;
    public SitemapController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var posts = await _db.BlogPosts.Where(b => b.IsPublished && b.IsActive).ToListAsync();
        var baseUrl = $"{Request.Scheme}://{Request.Host}";

        XNamespace ns = "http://www.sitemaps.org/schemas/sitemap/0.9";
        var urlset = new XElement(ns + "urlset",
            new XElement(ns + "url",
                new XElement(ns + "loc", $"{baseUrl}{Url.Action("Index","Blog")}"),
                new XElement(ns + "changefreq", "daily"),
                new XElement(ns + "priority", "0.8")
            ),
            posts.Select(p => new XElement(ns + "url",
                new XElement(ns + "loc", $"{baseUrl}{Url.Action("Details","Blog", new { slug = p.Slug })}"),
                new XElement(ns + "lastmod", ((p.UpdatedDate ?? p.PublishedDate ?? p.CreatedDate).ToUniversalTime()).ToString("yyyy-MM-ddTHH:mm:ssZ")),
                new XElement(ns + "changefreq", "weekly"),
                new XElement(ns + "priority", "0.6")
            ))
        );

        var xml = new XDocument(new XDeclaration("1.0", "utf-8", "yes"), urlset);
        var bytes = Encoding.UTF8.GetBytes(xml.ToString(SaveOptions.DisableFormatting));
        return File(bytes, "application/xml; charset=utf-8");
    }
}
