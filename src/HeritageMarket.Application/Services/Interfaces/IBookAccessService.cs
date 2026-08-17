using HeritageMarket.Application.DTOs;
using HeritageMarket.Domain.Enums;

namespace HeritageMarket.Application.Services.Interfaces;

public interface IBookAccessService
{
    /// <summary>The customer's most recent request, or null if they've never asked.</summary>
    Task<BookAccessRequestDto?> GetLatestForUserAsync(string applicationUserId);

    Task<bool> IsApprovedAsync(string applicationUserId);

    Task SubmitRequestAsync(SubmitBookAccessRequest request);

    Task<IReadOnlyList<BookAccessRequestDto>> GetPendingAsync();

    Task<IReadOnlyList<BookAccessRequestDto>> GetAllAsync();

    Task ReviewAsync(int requestId, BookAccessStatus decision, string? adminNote);
}
