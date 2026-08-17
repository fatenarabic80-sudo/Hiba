using HeritageMarket.Application.Common;
using HeritageMarket.Application.DTOs;
using HeritageMarket.Application.Services.Interfaces;
using HeritageMarket.Domain.Entities;
using HeritageMarket.Domain.Enums;
using HeritageMarket.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HeritageMarket.Application.Services.Implementations;

public class BookAccessService : IBookAccessService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserDirectoryService _userDirectory;

    public BookAccessService(IUnitOfWork unitOfWork, IUserDirectoryService userDirectory)
    {
        _unitOfWork = unitOfWork;
        _userDirectory = userDirectory;
    }

    private async Task<BookAccessRequestDto> ToDtoAsync(BookAccessRequest r) => new()
    {
        Id = r.Id,
        ApplicationUserId = r.ApplicationUserId,
        RequesterName = await _userDirectory.GetDisplayNameAsync(r.ApplicationUserId),
        Reason = r.Reason,
        PreferredCountry = r.PreferredCountry,
        Status = r.Status,
        RequestedAt = r.RequestedAt,
        ReviewedAt = r.ReviewedAt,
        AdminNote = r.AdminNote
    };

    public async Task<BookAccessRequestDto?> GetLatestForUserAsync(string applicationUserId)
    {
        var request = await _unitOfWork.BookAccessRequests.Query().AsNoTracking()
            .Where(r => r.ApplicationUserId == applicationUserId)
            .OrderByDescending(r => r.RequestedAt)
            .FirstOrDefaultAsync();

        return request is null ? null : await ToDtoAsync(request);
    }

    public async Task<bool> IsApprovedAsync(string applicationUserId)
    {
        return await _unitOfWork.BookAccessRequests.Query().AsNoTracking()
            .AnyAsync(r => r.ApplicationUserId == applicationUserId && r.Status == BookAccessStatus.Approved);
    }

    public async Task SubmitRequestAsync(SubmitBookAccessRequest request)
    {
        // Auto-approved on submission: a manual admin wait here loses the customer's momentum
        // right when they're most interested. The request is still logged for Admin visibility
        // (and can be revoked from Admin -> Book Requests), it just no longer blocks access.
        var now = DateTime.UtcNow;
        await _unitOfWork.BookAccessRequests.AddAsync(new BookAccessRequest
        {
            ApplicationUserId = request.ApplicationUserId,
            Reason = request.Reason,
            PreferredCountry = request.PreferredCountry,
            Status = BookAccessStatus.Approved,
            RequestedAt = now,
            ReviewedAt = now,
            AdminNote = "Auto-approved"
        });
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<IReadOnlyList<BookAccessRequestDto>> GetPendingAsync()
    {
        var requests = await _unitOfWork.BookAccessRequests.Query().AsNoTracking()
            .Where(r => r.Status == BookAccessStatus.Pending)
            .OrderBy(r => r.RequestedAt)
            .ToListAsync();

        var result = new List<BookAccessRequestDto>();
        foreach (var r in requests) result.Add(await ToDtoAsync(r));
        return result;
    }

    public async Task<IReadOnlyList<BookAccessRequestDto>> GetAllAsync()
    {
        var requests = await _unitOfWork.BookAccessRequests.Query().AsNoTracking()
            .OrderByDescending(r => r.RequestedAt)
            .ToListAsync();

        var result = new List<BookAccessRequestDto>();
        foreach (var r in requests) result.Add(await ToDtoAsync(r));
        return result;
    }

    public async Task ReviewAsync(int requestId, BookAccessStatus decision, string? adminNote)
    {
        var request = await _unitOfWork.BookAccessRequests.GetByIdAsync(requestId)
            ?? throw new NotFoundException($"Book access request {requestId} not found.");

        request.Status = decision;
        request.AdminNote = adminNote;
        request.ReviewedAt = DateTime.UtcNow;

        _unitOfWork.BookAccessRequests.Update(request);
        await _unitOfWork.SaveChangesAsync();
    }
}
