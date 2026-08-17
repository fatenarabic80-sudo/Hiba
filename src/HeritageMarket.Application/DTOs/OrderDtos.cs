using HeritageMarket.Domain.Enums;

namespace HeritageMarket.Application.DTOs;

public class OrderListItemDto
{
    public int Id { get; set; }
    public DateTime OrderDate { get; set; }
    public OrderStatus Status { get; set; }
    public decimal TotalAmount { get; set; }
    public int ItemCount { get; set; }
}

public class OrderDetailDto : OrderListItemDto
{
    public string ApplicationUserId { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string ShippingAddress { get; set; } = string.Empty;
    public string ShippingCity { get; set; } = string.Empty;
    public List<OrderItemDto> Items { get; set; } = new();
}

public class OrderItemDto
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string? ProductImageUrl { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public string? SelectedSize { get; set; }
    public decimal LineTotal => UnitPrice * Quantity;
}

public class PlaceOrderRequest
{
    public string ApplicationUserId { get; set; } = string.Empty;
    public string ShippingAddress { get; set; } = string.Empty;
    public string ShippingCity { get; set; } = string.Empty;
}
