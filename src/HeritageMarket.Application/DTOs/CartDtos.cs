namespace HeritageMarket.Application.DTOs;

public class CartDto
{
    public int Id { get; set; }
    public List<CartItemDto> Items { get; set; } = new();
    public decimal Total => Items.Sum(i => i.LineTotal);
}

public class CartItemDto
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string? ProductImageUrl { get; set; }
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public int AvailableStock { get; set; }
    public string? SelectedSize { get; set; }
    public decimal LineTotal => UnitPrice * Quantity;
}
