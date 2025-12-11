using System.ComponentModel.DataAnnotations;

namespace Blog.Models
{
    public class BlogPost
    {
        [Key]
        public int BlogId { get; set; }

      //  [Required, MaxLength(255)]
        public string Title { get; set; } = "";

      //  [Required, MaxLength(255)]
        public string Slug { get; set; } = "";

       // [MaxLength(500)]
        public string Excerpt { get; set; }

      //  [Required]                 // CKEditor HTML
        public string Content { get; set; } = "";

       // [MaxLength(500)]
        public string FeatureImageUrl { get; set; }

       // [MaxLength(100)]
        public string Category { get; set; }

        // store tags as CSV or JSON if you like
       // [MaxLength(250)]
        public string Tags { get; set; }

        public bool IsActive { get; set; } = true;
        public bool IsPublished { get; set; } = false;

       // [Required, MaxLength(100)]
        public string CreatedBy { get; set; } = "";

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        //[MaxLength(100)]
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public DateTime? PublishedDate { get; set; }         
    }
}
