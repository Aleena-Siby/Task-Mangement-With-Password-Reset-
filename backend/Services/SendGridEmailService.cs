using SendGrid;
using SendGrid.Helpers.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

public class SendGridEmailService
{
    private readonly string _apiKey;

    public SendGridEmailService(IConfiguration configuration)
    {
        _apiKey = configuration["SendGrid:ApiKey"];
    }

    public async Task SendEmailAsync(string toEmail, string subject, string body)
    {
        var client = new SendGridClient(_apiKey);
        var from = new EmailAddress("selimasiby@gmail.com", "Support Team");
        var to = new EmailAddress(toEmail);
        var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent: body, htmlContent: body);

        var response = await client.SendEmailAsync(msg);

        if ((int)response.StatusCode >= 400)
        {
            throw new Exception($"Failed to send email: {response.StatusCode}");
        }
    }
}
