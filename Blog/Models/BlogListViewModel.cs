namespace Blog.Models;

public class BlogListItem
{
    public int BlogId { get; set; }

    public string Title { get; set; } = "";

    public string Slug { get; set; } = "";

    public string? Excerpt { get; set; }

    public string? FeatureImageUrl { get; set; }

    public string? Category { get; set; }

    public DateTime PublishedDate { get; set; }

    public int ReadMinutes { get; set; }
}

public class BlogListViewModel
{
    public BlogPost Featured { get; set; } = new BlogPost();
    public List<BlogListItem> Trending { get; set; } = new List<BlogListItem>();
}
