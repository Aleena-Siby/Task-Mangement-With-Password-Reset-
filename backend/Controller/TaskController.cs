using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using backend.Data;
using System.Security.Claims;
using backend.Models;
using Microsoft.AspNetCore.Http.HttpResults;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class TasksController : ControllerBase
{
    private readonly AppDbContext _context;

    public TasksController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/tasks
    // GET: api/tasks
[HttpGet]
public async Task<ActionResult<IEnumerable<UserTask>>> GetTasks()
{   Console.WriteLine("Helloo");
    var userIdClaim = User.FindFirst("id")?.Value;
     Console.WriteLine($"Retrieved User ID claim (as string): {userIdClaim}");      // Retrieve user ID as a string

    // Check if the claim is not null and attempt to parse it
    if (int.TryParse(userIdClaim, out int userId)) // Assuming UserId is int
    {   
        var tasks = await _context.Tasks.Where(t => t.UserId == userId).ToListAsync(); // Query tasks for the logged-in user

        // Check if tasks were found
        if (tasks == null || !tasks.Any())
        {   
            return NotFound(); // Return 404 if no tasks found
        }

        return Ok(tasks); // Return 200 with tasks
    }
    else
    {   Console.WriteLine("User ID claim is null or invalid.");
        return BadRequest("Invalid user ID."); // Return error if parsing fails
    }
}

    // POST: api/tasks
    [HttpPost]
public async Task<ActionResult<UserTask>> CreateTask(UserTask task)
{
    // Attempt to retrieve the user ID as a string from the claims
    var userIdString = User.FindFirst("id")?.Value;

    // Convert the user ID to an integer
    if (!int.TryParse(userIdString, out var userId))
    {
        return BadRequest("Invalid user ID."); // Handle the case where the ID is not valid
    }

    task.UserId = userId; // Assign the logged-in user's ID to the task

    _context.Tasks.Add(task);
    await _context.SaveChangesAsync();
    Console.WriteLine("Tasks added..");
    return CreatedAtAction(nameof(GetTasks), new { id = task.TaskId }, task);
}


  [HttpDelete("{taskId}")]
public async Task<IActionResult> DeleteTask(int taskId)
{
    var task = await _context.Tasks.FindAsync(taskId);
    if (task == null)
    {
        return NotFound();
    }

    _context.Tasks.Remove(task);
    await _context.SaveChangesAsync();

    return NoContent();
}


[HttpPut("{taskId}")]
public async Task<IActionResult> UpdateTask(int taskId, [FromBody] UserTask updatedTask)
{
    var task = await _context.Tasks.FindAsync(taskId);
    if (task == null)
    {
        return NotFound();  // If task doesn't exist, return 404
    }

    // Update task fields (assuming you want to update the Title and Description)
    task.Title = updatedTask.Title;
    task.Description = updatedTask.Description;
    task.DueDate = updatedTask.DueDate;
    task.IsCompleted = updatedTask.IsCompleted;

    // Save the changes
    _context.Tasks.Update(task);
    await _context.SaveChangesAsync();

    return NoContent();  // Return status 204 if update is successful
}


}
