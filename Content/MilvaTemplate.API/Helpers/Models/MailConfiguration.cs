#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
namespace MilvaTemplate.API.Helpers.Models;

public class MailConfiguration
{
    public string Key { get; set; }
    public string DisplayName { get; set; }
    public string Sender { get; set; }
    public string SenderPass { get; set; }
    public int SmtpPort { get; set; }
    public string SmtpHost { get; set; }
}
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
