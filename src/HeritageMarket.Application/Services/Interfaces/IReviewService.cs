using HeritageMarket.Application.DTOs;

namespace HeritageMarket.Application.Services.Interfaces;

public interface IReviewService
{
    Task AddReviewAsync(CreateReviewRequest request);
    Task<IReadOnlyList<ReviewDto>> GetForProductAsync(int productId);
}
