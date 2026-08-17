namespace HeritageMarket.Domain.Entities;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public string? ImageUrl { get; set; }
    public string SKU { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>Who the item is cut for — "Women", "Men", "Kids", or null when not applicable (mainly used by Wear & Traditional Clothing).</summary>
    public string? Gender { get; set; }

    /// <summary>Comma-separated available sizes, e.g. "S,M,L,XL" or "One Size". Mainly used by Wear & Traditional Clothing.</summary>
    public string? Sizes { get; set; }

    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;

    public int CountryId { get; set; }
    public Country Country { get; set; } = null!;

    public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
}
