#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
namespace MilvaTemplate.API.Helpers.Models;

public class Configurations : IJsonModel
{
    public List<MailConfiguration> Mails { get; set; }
    public List<TokenManagement> Tokens { get; set; }

}
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
