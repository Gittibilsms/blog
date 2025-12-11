using System.ComponentModel.DataAnnotations;

namespace Blog.Models
{
    public class AdminUser
    {
        [Key]
        public int AdminId { get; set; }

        [Required, MaxLength(50)]
        public string Username { get; set; } = "";

        [Required, MaxLength(255)]
        public string PasswordHash { get; set; } = "";

        public bool IsActive { get; set; } = true;
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    }
}
