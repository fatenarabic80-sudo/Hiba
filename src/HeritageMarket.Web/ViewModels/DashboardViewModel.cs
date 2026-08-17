using HeritageMarket.Application.DTOs;

namespace HeritageMarket.Web.ViewModels;

/// <summary>
/// The customer's personal dashboard — a snapshot of their own account: recent orders, spend,
/// wishlist, and cart, as distinct from the Admin Dashboard which surfaces store-wide data.
/// </summary>
public class DashboardViewModel
{
    public string FullName { get; set; } = string.Empty;
    public string? ProfileImageUrl { get; set; }
    public DateTime MemberSince { get; set; }

    public int TotalOrders { get; set; }
    public decimal TotalSpent { get; set; }
    public IReadOnlyList<OrderListItemDto> RecentOrders { get; set; } = new List<OrderListItemDto>();

    public int WishlistCount { get; set; }
    public IReadOnlyList<ProductListItemDto> WishlistPreview { get; set; } = new List<ProductListItemDto>();

    public int CartItemCount { get; set; }
}
