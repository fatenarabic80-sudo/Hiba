using HeritageMarket.Domain.Entities;
using HeritageMarket.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace HeritageMarket.Infrastructure.BackgroundServices;

public class LowStockNotificationService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<LowStockNotificationService> _logger;
    private readonly int _thresholdQuantity;
    private readonly TimeSpan _interval;

    public LowStockNotificationService(
        IServiceScopeFactory scopeFactory,
        ILogger<LowStockNotificationService> logger,
        IConfiguration configuration)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
        _thresholdQuantity = configuration.GetValue("LowStock:ThresholdQuantity", 5);
        _interval = TimeSpan.FromMinutes(configuration.GetValue("LowStock:CheckIntervalMinutes", 60));
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await CheckLowStockAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Low stock notification sweep failed.");
            }

            await Task.Delay(_interval, stoppingToken);
        }
    }

    private async Task CheckLowStockAsync(CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        var lowStockProducts = await unitOfWork.Products.Query()
            .Where(p => p.IsActive && p.StockQuantity <= _thresholdQuantity)
            .ToListAsync(cancellationToken);

        if (lowStockProducts.Count == 0)
        {
            _logger.LogInformation("Low stock sweep complete: no products at or below threshold {Threshold}.", _thresholdQuantity);
            return;
        }

        var recentlyNotifiedProductIds = (await unitOfWork.Notifications.Query()
            .Where(n => n.CreatedAt >= DateTime.UtcNow.AddHours(-24) && n.ProductId != null)
            .Select(n => n.ProductId!.Value)
            .ToListAsync(cancellationToken))
            .ToHashSet();

        var newNotifications = 0;
        foreach (var product in lowStockProducts)
        {
            if (recentlyNotifiedProductIds.Contains(product.Id))
                continue;

            await unitOfWork.Notifications.AddAsync(new Notification
            {
                Message = $"Low stock alert: '{product.Name}' has only {product.StockQuantity} unit(s) left.",
                ProductId = product.Id,
                CreatedAt = DateTime.UtcNow
            });
            newNotifications++;
        }

        if (newNotifications > 0)
        {
            await unitOfWork.SaveChangesAsync();
            _logger.LogWarning("Created {Count} low stock notification(s).", newNotifications);
        }
    }
}
