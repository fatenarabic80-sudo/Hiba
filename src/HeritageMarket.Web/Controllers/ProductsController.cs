using HeritageMarket.Application.Common;
using HeritageMarket.Application.DTOs;
using HeritageMarket.Application.Services.Interfaces;
using HeritageMarket.Domain.Enums;
using HeritageMarket.Infrastructure.Identity;
using HeritageMarket.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HeritageMarket.Web.Controllers;

public class ProductsController : Controller
{
    public const string HeritageBooksCategoryName = "Heritage Books";

    private readonly IProductService _productService;
    private readonly ICategoryService _categoryService;
    private readonly ICountryService _countryService;
    private readonly IReviewService _reviewService;
    private readonly IWishlistService _wishlistService;
    private readonly IBookAccessService _bookAccessService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<ProductsController> _logger;

    public ProductsController(
        IProductService productService,
        ICategoryService categoryService,
        ICountryService countryService,
        IReviewService reviewService,
        IWishlistService wishlistService,
        IBookAccessService bookAccessService,
        UserManager<ApplicationUser> userManager,
        ILogger<ProductsController> logger)
    {
        _productService = productService;
        _categoryService = categoryService;
        _countryService = countryService;
        _reviewService = reviewService;
        _wishlistService = wishlistService;
        _bookAccessService = bookAccessService;
        _userManager = userManager;
        _logger = logger;
    }

    private string? CurrentUserId => _userManager.GetUserId(User);

    public async Task<IActionResult> Index(string? searchTerm, int? categoryId, int? countryId, int pageNumber = 1)
    {
        var categories = await _categoryService.GetAllAsync();
        var booksCategoryId = categories.FirstOrDefault(c => c.Name == HeritageBooksCategoryName)?.Id;

        if (categoryId.HasValue && categoryId == booksCategoryId)
        {
            var approved = CurrentUserId is not null && await _bookAccessService.IsApprovedAsync(CurrentUserId);
            if (!approved)
                return RedirectToAction(nameof(Books));
        }

        var filter = new ProductFilter
        {
            SearchTerm = searchTerm,
            CategoryId = categoryId,
            CountryId = countryId,
            ExcludeCategoryId = booksCategoryId,
            PageNumber = pageNumber,
            PageSize = 9
        };

        var model = new ProductCatalogViewModel
        {
            Products = await _productService.GetCatalogAsync(filter),
            Categories = categories,
            Countries = await _countryService.GetAllAsync(),
            Filter = filter
        };

        await SetWishlistedIdsAsync();
        return View(model);
    }

    public async Task<IActionResult> Details(int id)
    {
        var product = await _productService.GetDetailAsync(id);
        if (product is null) return NotFound();

        if (product.CategoryName == HeritageBooksCategoryName)
        {
            var approved = CurrentUserId is not null && await _bookAccessService.IsApprovedAsync(CurrentUserId);
            if (!approved)
                return RedirectToAction(nameof(Books));
        }

        var model = new ProductDetailsViewModel
        {
            Product = product,
            ReviewForm = new ReviewFormViewModel { ProductId = id },
            CanReview = User.Identity?.IsAuthenticated ?? false
        };

        await SetWishlistedIdsAsync();
        return View(model);
    }

    // The "magical" Heritage Books intercept: a Heritage-Guide-styled intake instead of a normal
    // product grid, gating the category until an Admin approves the customer's request.
    public async Task<IActionResult> Books()
    {
        var isAuthenticated = User.Identity?.IsAuthenticated ?? false;
        var model = new BooksGateViewModel
        {
            IsAuthenticated = isAuthenticated,
            Countries = await _countryService.GetAllAsync()
        };

        if (isAuthenticated && CurrentUserId is not null)
        {
            var latest = await _bookAccessService.GetLatestForUserAsync(CurrentUserId);
            model.Status = latest?.Status;
            model.AdminNote = latest?.AdminNote;

            if (latest?.Status == BookAccessStatus.Approved)
            {
                var booksCategory = (await _categoryService.GetAllAsync()).FirstOrDefault(c => c.Name == HeritageBooksCategoryName);
                if (booksCategory is not null)
                    return RedirectToAction(nameof(Index), new { categoryId = booksCategory.Id });
            }
        }

        return View(model);
    }

    [HttpPost, Authorize, ValidateAntiForgeryToken]
    public async Task<IActionResult> SubmitBookAccessRequest(BookAccessRequestFormViewModel form)
    {
        if (!ModelState.IsValid)
        {
            var model = new BooksGateViewModel
            {
                IsAuthenticated = true,
                Countries = await _countryService.GetAllAsync(),
                Form = form
            };
            return View(nameof(Books), model);
        }

        await _bookAccessService.SubmitRequestAsync(new SubmitBookAccessRequest
        {
            ApplicationUserId = CurrentUserId!,
            Reason = form.Reason,
            PreferredCountry = form.PreferredCountry
        });

        TempData["StatusMessage"] = "Thanks! Your Heritage Guide request has been sent to our team.";
        return RedirectToAction(nameof(Books));
    }

    // Used by the AI Guide chat widget on the Books page — same effect as SubmitBookAccessRequest,
    // but responds with JSON so the conversation can continue in place instead of redirecting.
    [HttpPost, Authorize, ValidateAntiForgeryToken]
    public async Task<IActionResult> SubmitBookAccessRequestAjax(string preferredCountry, string reason)
    {
        if (string.IsNullOrWhiteSpace(preferredCountry) || string.IsNullOrWhiteSpace(reason))
            return BadRequest(new { message = "A country and a reason are both required." });

        await _bookAccessService.SubmitRequestAsync(new SubmitBookAccessRequest
        {
            ApplicationUserId = CurrentUserId!,
            Reason = reason.Length > 500 ? reason[..500] : reason,
            PreferredCountry = preferredCountry
        });

        return Json(new { success = true });
    }

    private async Task SetWishlistedIdsAsync()
    {
        if (!(User.Identity?.IsAuthenticated ?? false)) return;

        var userId = _userManager.GetUserId(User);
        if (userId is not null)
            ViewData["WishlistedIds"] = await _wishlistService.GetWishlistedProductIdsAsync(userId);
    }

    [HttpPost, Authorize, ValidateAntiForgeryToken]
    public async Task<IActionResult> AddReview(ReviewFormViewModel form)
    {
        if (!ModelState.IsValid)
        {
            TempData["ErrorMessage"] = "Please provide a valid rating (1-5).";
            return RedirectToAction(nameof(Details), new { id = form.ProductId });
        }

        var userId = _userManager.GetUserId(User)!;

        try
        {
            await _reviewService.AddReviewAsync(new CreateReviewRequest
            {
                ProductId = form.ProductId,
                ApplicationUserId = userId,
                Rating = form.Rating,
                Comment = form.Comment
            });
            TempData["StatusMessage"] = "Thank you for your review!";
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to add review for product {ProductId}", form.ProductId);
            TempData["ErrorMessage"] = "We couldn't save your review. Please try again.";
        }

        return RedirectToAction(nameof(Details), new { id = form.ProductId });
    }
}
