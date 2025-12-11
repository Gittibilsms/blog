using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Blog.Data;
using System.Text;
using System.Xml.Linq;

[Route("rss")]
public class RssController : Controller
{
    private readonly AppDbContext _db;
    public RssController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var posts = await _db.BlogPosts.Where(b => b.IsPublished && b.IsActive)
            .OrderByDescending(b => b.PublishedDate ?? b.CreatedDate)
            .Take(20)
            .ToListAsync();

        var baseUrl = $"{Request.Scheme}://{Request.Host}";
        XNamespace ns = "http://www.w3.org/2005/Atom";
        var feed = new XElement("rss",
            new XAttribute("version", "2.0"),
            new XElement("channel",
                new XElement("title", "GittiBil SMS — Blog"),
                new XElement("link", $"{baseUrl}{Url.Action("Index", "Blog")}"),
                new XElement("description", "Latest posts from GittiBil SMS"),
                posts.Select(p => new XElement("item",
                    new XElement("title", p.Title),
                    new XElement("link", $"{baseUrl}{Url.Action("Details","Blog", new { slug = p.Slug })}"),
                    new XElement("guid", $"{baseUrl}{Url.Action("Details","Blog", new { slug = p.Slug })}"),
                    new XElement("pubDate", ((p.PublishedDate ?? p.CreatedDate).ToUniversalTime()).ToString("r")),
                    new XElement("description", System.Security.SecurityElement.Escape(p.Excerpt ?? ""))
                ))
            )
        );

        var xml = new XDocument(new XDeclaration("1.0", "utf-8", "yes"), feed);
        var bytes = Encoding.UTF8.GetBytes(xml.ToString(SaveOptions.DisableFormatting));
        return File(bytes, "application/rss+xml; charset=utf-8");
    }
}
