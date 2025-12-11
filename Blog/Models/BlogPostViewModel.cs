namespace Blog.Models;

public class BlogPostViewModel
{
    public int BlogId { get; set; } 
    public string Title { get; set; } = ""; 
    public string Slug { get; set; } = ""; 
    public string? Excerpt { get; set; }
    public string Content { get; set; } = ""; 
    public string? FeatureImageUrl { get; set; } 
    public string? Category { get; set; } 
    public string[]? Tags { get; set; } 
    public bool IsActive { get; set; } 
    public bool IsPublished { get; set; } 
    public string CreatedBy { get; set; } = ""; 
    public DateTime CreatedDate { get; set; } 
    public string? UpdatedBy { get; set; } 
    public DateTime? UpdatedDate { get; set; } 
    public DateTime? PublishedDate { get; set; } 
    public int ReadMinutes { get; set; } 
}
