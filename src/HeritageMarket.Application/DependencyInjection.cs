using HeritageMarket.Application.Services.Implementations;
using HeritageMarket.Application.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace HeritageMarket.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<ICountryService, CountryService>();
        services.AddScoped<ICartService, CartService>();
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<IReviewService, ReviewService>();
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<IReportService, ReportService>();
        services.AddScoped<IWishlistService, WishlistService>();
        services.AddScoped<IBookAccessService, BookAccessService>();

        return services;
    }
}
