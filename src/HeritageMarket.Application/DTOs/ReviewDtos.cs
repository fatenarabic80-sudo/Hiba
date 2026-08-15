namespace HeritageMarket.Application.DTOs;

public class ReviewDto
{
    public int Id { get; set; }
    public string ApplicationUserId { get; set; } = string.Empty;
    public string ReviewerName { get; set; } = string.Empty;
    public int Rating { get; set; }
    public string? Comment { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateReviewRequest
{
    public int ProductId { get; set; }
    public string ApplicationUserId { get; set; } = string.Empty;
    public int Rating { get; set; }
    public string? Comment { get; set; }
}
