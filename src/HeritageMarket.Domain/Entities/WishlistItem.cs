namespace HeritageMarket.Domain.Entities;

public class WishlistItem
{
    public int Id { get; set; }
    public string ApplicationUserId { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;
}
