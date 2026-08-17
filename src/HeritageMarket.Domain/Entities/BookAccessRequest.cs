using HeritageMarket.Domain.Enums;

namespace HeritageMarket.Domain.Entities;

/// <summary>
/// A customer's request to unlock the Heritage Books category. Created after the customer answers
/// the Heritage Guide's intake questions; the Books category stays hidden from them until an Admin
/// approves it.
/// </summary>
public class BookAccessRequest
{
    public int Id { get; set; }
    public string ApplicationUserId { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public string PreferredCountry { get; set; } = string.Empty;
    public BookAccessStatus Status { get; set; } = BookAccessStatus.Pending;
    public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ReviewedAt { get; set; }
    public string? AdminNote { get; set; }
}
