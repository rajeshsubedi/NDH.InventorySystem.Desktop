using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InventoryAppServicesLayer.ServiceInterfaces;
using Microsoft.Extensions.Configuration;
using PostmarkDotNet;

namespace InventoryAppServicesLayer.ServiceImplementations
{
    public class EmailService : IEmailService
    {
        private readonly PostmarkClient _client;
        private readonly string _fromAddress;

        public EmailService(IConfiguration configuration)
        {
            // Fetch API key from app settings
            var apiKey = configuration["Postmark:ApiKey"];
            _fromAddress = configuration["Postmark:FromAddress"];

            // Initialize Postmark client
            _client = new PostmarkClient(apiKey);
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            if (string.IsNullOrWhiteSpace(toEmail))
                throw new ArgumentException("To email address is required.");

            if (string.IsNullOrWhiteSpace(subject))
                throw new ArgumentException("Subject is required.");

            if (string.IsNullOrWhiteSpace(body))
                throw new ArgumentException("Body content is required.");

            try
            {
                var message = new PostmarkMessage
                {
                    To = toEmail,
                    From = _fromAddress, // The 'From' address must be verified in Postmark
                    Subject = subject,
                    TextBody = body,
                    HtmlBody = $"<p>{body}</p>"
                };

                var response = await _client.SendMessageAsync(message);

                if (response.Status == PostmarkStatus.Success)
                {
                    Console.WriteLine($"Email sent successfully to {toEmail}. MessageId: {response.MessageID}");
                }
                else
                {
                    Console.WriteLine($"Error sending email to {toEmail}: {response.Message}");
                    throw new Exception($"Postmark failed to send email. Status: {response.Status}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception occurred: {ex.Message}");
                throw;
            }
        }
    }
}
