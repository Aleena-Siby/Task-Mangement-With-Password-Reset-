using System.ComponentModel.DataAnnotations;


namespace backend.Models
{
 public class UserTask
{    [Key]
    public int TaskId { get; set; } // This uniquely identifies the task
    public string Title { get; set; }
    public string Description { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime DueDate { get; set; }

    // Foreign key to link the task with the user
    public int  UserId { get; set; }  // Links to the AppUser
    public AppUser?User { get; set; }   // Navigation property
}

}