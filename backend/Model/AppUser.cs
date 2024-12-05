using System.ComponentModel.DataAnnotations;

namespace backend.Models
{
    public class AppUser
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Username { get; set; }
        [Required]
       public string Email { get; set; } 
        [Required]
        public string PasswordHash { get; set; } // Store hashed passwords
         public ICollection<UserTask> Tasks { get; set; }
         public string? PasswordResetToken {get;set;}
         public DateTime? PasswordResetTokenExpiration {get;set;}
    }
}
