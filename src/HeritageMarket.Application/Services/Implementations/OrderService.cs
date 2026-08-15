using HeritageMarket.Application.Common;
using HeritageMarket.Application.DTOs;
using HeritageMarket.Application.Services.Interfaces;
using HeritageMarket.Domain.Entities;
using HeritageMarket.Domain.Enums;
using HeritageMarket.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HeritageMarket.Application.Services.Implementations;

public class OrderService : IOrderService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserDirectoryService _userDirectory;

    public OrderService(IUnitOfWork unitOfWork, IUserDirectoryService userDirectory)
    {
        _unitOfWork = unitOfWork;
        _userDirectory = userDirectory;
    }

    private static OrderDetailDto ToDetailDto(Order order, string customerName) => new()
    {
        Id = order.Id,
        ApplicationUserId = order.ApplicationUserId,
        CustomerName = customerName,
        OrderDate = order.OrderDate,
        Status = order.Status,
        TotalAmount = order.TotalAmount,
        ShippingAddress = order.ShippingAddress,
        ShippingCity = order.ShippingCity,
        ItemCount = order.Items.Sum(i => i.Quantity),
        Items = order.Items.Select(i => new OrderItemDto
        {
            ProductId = i.ProductId,
            ProductName = i.Product.Name,
            ProductImageUrl = i.Product.ImageUrl,
            Quantity = i.Quantity,
            UnitPrice = i.UnitPrice
        }).ToList()
    };

    public async Task<OrderDetailDto> PlaceOrderAsync(PlaceOrderRequest request)
    {
        var cart = await _unitOfWork.Carts.GetByUserIdAsync(request.ApplicationUserId);
        if (cart is null || cart.Items.Count == 0)
            throw new InvalidOperationException("Cannot place an order with an empty cart.");

        var order = new Order
        {
            ApplicationUserId = request.ApplicationUserId,
            OrderDate = DateTime.UtcNow,
            Status = OrderStatus.Pending,
            ShippingAddress = request.ShippingAddress,
            ShippingCity = request.ShippingCity
        };

        decimal total = 0;
        foreach (var cartItem in cart.Items.ToList())
        {
            var product = await _unitOfWork.Products.GetByIdAsync(cartItem.ProductId)
                ?? throw new NotFoundException($"Product {cartItem.ProductId} not found.");

            if (cartItem.Quantity > product.StockQuantity)
                throw new InsufficientStockException($"Only {product.StockQuantity} unit(s) of '{product.Name}' are available.");

            product.StockQuantity -= cartItem.Quantity;
            _unitOfWork.Products.Update(product);

            order.Items.Add(new OrderItem
            {
                ProductId = product.Id,
                Quantity = cartItem.Quantity,
                UnitPrice = product.Price
            });

            total += product.Price * cartItem.Quantity;
        }

        order.TotalAmount = total;

        await _unitOfWork.Orders.AddAsync(order);
        cart.Items.Clear();
        await _unitOfWork.SaveChangesAsync();

        var reloaded = await _unitOfWork.Orders.GetWithItemsAsync(order.Id)
            ?? throw new NotFoundException("Order could not be reloaded after creation.");

        var customerName = await _userDirectory.GetDisplayNameAsync(order.ApplicationUserId);
        return ToDetailDto(reloaded, customerName);
    }

    public async Task<IReadOnlyList<OrderListItemDto>> GetOrdersForUserAsync(string applicationUserId)
    {
        var orders = await _unitOfWork.Orders.GetByUserIdAsync(applicationUserId);
        return orders
            .OrderByDescending(o => o.OrderDate)
            .Select(o => new OrderListItemDto
            {
                Id = o.Id,
                OrderDate = o.OrderDate,
                Status = o.Status,
                TotalAmount = o.TotalAmount,
                ItemCount = o.Items.Sum(i => i.Quantity)
            })
            .ToList();
    }

    public async Task<OrderDetailDto?> GetOrderDetailAsync(int id, string? applicationUserId = null)
    {
        var order = await _unitOfWork.Orders.GetWithItemsAsync(id);
        if (order is null) return null;
        if (applicationUserId is not null && order.ApplicationUserId != applicationUserId) return null;

        var customerName = await _userDirectory.GetDisplayNameAsync(order.ApplicationUserId);
        return ToDetailDto(order, customerName);
    }

    public async Task<PagedResult<OrderListItemDto>> GetAllOrdersAsync(int pageNumber, int pageSize, OrderStatus? status)
    {
        var query = _unitOfWork.Orders.Query().AsNoTracking();
        if (status.HasValue)
            query = query.Where(o => o.Status == status);

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(o => o.OrderDate)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(o => new OrderListItemDto
            {
                Id = o.Id,
                OrderDate = o.OrderDate,
                Status = o.Status,
                TotalAmount = o.TotalAmount,
                ItemCount = o.Items.Sum(i => i.Quantity)
            })
            .ToListAsync();

        return new PagedResult<OrderListItemDto>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    public async Task UpdateStatusAsync(int orderId, OrderStatus status)
    {
        var order = await _unitOfWork.Orders.GetByIdAsync(orderId)
            ?? throw new NotFoundException($"Order {orderId} not found.");

        order.Status = status;
        _unitOfWork.Orders.Update(order);
        await _unitOfWork.SaveChangesAsync();
    }
}
