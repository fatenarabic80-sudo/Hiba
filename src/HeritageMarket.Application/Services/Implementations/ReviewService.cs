using HeritageMarket.Application.Common;
using HeritageMarket.Application.DTOs;
using HeritageMarket.Application.Services.Interfaces;
using HeritageMarket.Domain.Entities;
using HeritageMarket.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HeritageMarket.Application.Services.Implementations;

public class ReviewService : IReviewService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserDirectoryService _userDirectory;

    public ReviewService(IUnitOfWork unitOfWork, IUserDirectoryService userDirectory)
    {
        _unitOfWork = unitOfWork;
        _userDirectory = userDirectory;
    }

    public async Task AddReviewAsync(CreateReviewRequest request)
    {
        if (request.Rating is < 1 or > 5)
            throw new ArgumentOutOfRangeException(nameof(request.Rating), "Rating must be between 1 and 5.");

        var productExists = await _unitOfWork.Products.ExistsAsync(p => p.Id == request.ProductId);
        if (!productExists)
            throw new NotFoundException($"Product {request.ProductId} not found.");

        var review = new Review
        {
            ProductId = request.ProductId,
            ApplicationUserId = request.ApplicationUserId,
            Rating = request.Rating,
            Comment = request.Comment,
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Reviews.AddAsync(review);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<IReadOnlyList<ReviewDto>> GetForProductAsync(int productId)
    {
        var reviews = await _unitOfWork.Reviews.Query().AsNoTracking()
            .Where(r => r.ProductId == productId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();

        var result = new List<ReviewDto>();
        foreach (var r in reviews)
        {
            result.Add(new ReviewDto
            {
                Id = r.Id,
                ApplicationUserId = r.ApplicationUserId,
                ReviewerName = await _userDirectory.GetDisplayNameAsync(r.ApplicationUserId),
                Rating = r.Rating,
                Comment = r.Comment,
                CreatedAt = r.CreatedAt
            });
        }

        return result;
    }
}
