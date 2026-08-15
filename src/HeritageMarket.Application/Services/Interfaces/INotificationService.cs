using HeritageMarket.Application.DTOs;

namespace HeritageMarket.Application.Services.Interfaces;

public interface INotificationService
{
    Task<IReadOnlyList<NotificationDto>> GetRecentAsync(int count);
    Task MarkAsReadAsync(int id);
}
