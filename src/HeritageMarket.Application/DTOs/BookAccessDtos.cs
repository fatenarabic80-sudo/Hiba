using HeritageMarket.Domain.Enums;

namespace HeritageMarket.Application.DTOs;

public class BookAccessRequestDto
{
    public int Id { get; set; }
    public string ApplicationUserId { get; set; } = string.Empty;
    public string RequesterName { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public string PreferredCountry { get; set; } = string.Empty;
    public BookAccessStatus Status { get; set; }
    public DateTime RequestedAt { get; set; }
    public DateTime? ReviewedAt { get; set; }
    public string? AdminNote { get; set; }
}

public class SubmitBookAccessRequest
{
    public string ApplicationUserId { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public string PreferredCountry { get; set; } = string.Empty;
}
