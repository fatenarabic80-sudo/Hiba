using HeritageMarket.Application.Common;
using HeritageMarket.Application.DTOs;
using HeritageMarket.Application.Services.Interfaces;
using HeritageMarket.Infrastructure.Identity;
using HeritageMarket.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HeritageMarket.Web.Controllers;

public class ProductsController : Controller
{
    private readonly IProductService _productService;
    private readonly ICategoryService _categoryService;
    private readonly ICountryService _countryService;
    private readonly IReviewService _reviewService;
    private readonly IWishlistService _wishlistService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<ProductsController> _logger;

    public ProductsController(
        IProductService productService,
        ICategoryService categoryService,
        ICountryService countryService,
        IReviewService reviewService,
        IWishlistService wishlistService,
        UserManager<ApplicationUser> userManager,
        ILogger<ProductsController> logger)
    {
        _productService = productService;
        _categoryService = categoryService;
        _countryService = countryService;
        _reviewService = reviewService;
        _wishlistService = wishlistService;
        _userManager = userManager;
        _logger = logger;
    }

    public async Task<IActionResult> Index(string? searchTerm, int? categoryId, int? countryId, int pageNumber = 1)
    {
        var filter = new ProductFilter
        {
            SearchTerm = searchTerm,
            CategoryId = categoryId,
            CountryId = countryId,
            PageNumber = pageNumber,
            PageSize = 9
        };

        var model = new ProductCatalogViewModel
        {
            Products = await _productService.GetCatalogAsync(filter),
            Categories = await _categoryService.GetAllAsync(),
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

        var model = new ProductDetailsViewModel
        {
            Product = product,
            ReviewForm = new ReviewFormViewModel { ProductId = id },
            CanReview = User.Identity?.IsAuthenticated ?? false
        };

        await SetWishlistedIdsAsync();
        return View(model);
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
