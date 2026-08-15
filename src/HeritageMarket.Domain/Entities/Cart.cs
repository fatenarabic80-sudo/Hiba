namespace HeritageMarket.Domain.Entities;

public class Cart
{
    public int Id { get; set; }
    public string ApplicationUserId { get; set; } = string.Empty;

    public ICollection<CartItem> Items { get; set; } = new List<CartItem>();
}
