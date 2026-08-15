using HeritageMarket.Application.Common;
using HeritageMarket.Application.DTOs;
using HeritageMarket.Domain.Enums;

namespace HeritageMarket.Application.Services.Interfaces;

public interface IOrderService
{
    Task<OrderDetailDto> PlaceOrderAsync(PlaceOrderRequest request);
    Task<IReadOnlyList<OrderListItemDto>> GetOrdersForUserAsync(string applicationUserId);
    Task<OrderDetailDto?> GetOrderDetailAsync(int id, string? applicationUserId = null);
    Task<PagedResult<OrderListItemDto>> GetAllOrdersAsync(int pageNumber, int pageSize, OrderStatus? status);
    Task UpdateStatusAsync(int orderId, OrderStatus status);
}
