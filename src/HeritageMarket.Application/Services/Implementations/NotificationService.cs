using HeritageMarket.Application.Common;
using HeritageMarket.Application.DTOs;
using HeritageMarket.Application.Services.Interfaces;
using HeritageMarket.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HeritageMarket.Application.Services.Implementations;

public class NotificationService : INotificationService
{
    private readonly IUnitOfWork _unitOfWork;

    public NotificationService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IReadOnlyList<NotificationDto>> GetRecentAsync(int count)
    {
        return await _unitOfWork.Notifications.Query().AsNoTracking()
            .OrderByDescending(n => n.CreatedAt)
            .Take(count)
            .Select(n => new NotificationDto
            {
                Id = n.Id,
                Message = n.Message,
                CreatedAt = n.CreatedAt,
                IsRead = n.IsRead,
                ProductId = n.ProductId
            })
            .ToListAsync();
    }

    public async Task MarkAsReadAsync(int id)
    {
        var notification = await _unitOfWork.Notifications.GetByIdAsync(id)
            ?? throw new NotFoundException($"Notification {id} not found.");

        notification.IsRead = true;
        _unitOfWork.Notifications.Update(notification);
        await _unitOfWork.SaveChangesAsync();
    }
}
