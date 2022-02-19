namespace MilvaTemplate.API.DTOs.AccountDTOs;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
public class RefreshLoginDTO
{
    public string RefreshToken { get; set; }
    public string OldToken { get; set; }
}
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
