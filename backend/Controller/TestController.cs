using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using backend.Data;
using System.Security.Claims;
using backend.Models;
using Microsoft.AspNetCore.Http.HttpResults;
namespace backend.Controllers{
[ApiController]
[Route("api/[controller]")]
public class TestController : ControllerBase
{
    private readonly SendGridEmailService _emailService;

    public TestController(SendGridEmailService emailService)
    {
        _emailService = emailService;
    }

    [HttpGet("test-email-service")]
    public async Task<IActionResult> TestSendGridService()
    {
        try
        {
            await _emailService.SendEmailAsync("selimasiby@gmail.com", "Test Email from Service", "This is a test email body.");
            return Ok("Email sent successfully!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return StatusCode(500, ex.Message);
        }
    }
}
}