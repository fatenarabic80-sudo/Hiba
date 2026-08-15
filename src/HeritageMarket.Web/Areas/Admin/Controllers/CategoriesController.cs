using HeritageMarket.Application.DTOs;
using HeritageMarket.Application.Services.Interfaces;
using HeritageMarket.Infrastructure.Identity;
using HeritageMarket.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HeritageMarket.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = IdentityRoles.Admin)]
public class CategoriesController : Controller
{
    private readonly ICategoryService _categoryService;
    private readonly ILogger<CategoriesController> _logger;

    public CategoriesController(ICategoryService categoryService, ILogger<CategoriesController> logger)
    {
        _categoryService = categoryService;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        var categories = await _categoryService.GetAllAsync();
        return View(categories);
    }

    public IActionResult Create() => View(new CategoryFormViewModel());

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CategoryFormViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        await _categoryService.CreateAsync(new CategoryDto { Name = model.Name, Description = model.Description, IconUrl = model.IconUrl });
        TempData["StatusMessage"] = "Category created.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var category = await _categoryService.GetByIdAsync(id);
        if (category is null) return NotFound();

        return View(new CategoryFormViewModel { Id = category.Id, Name = category.Name, Description = category.Description, IconUrl = category.IconUrl });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, CategoryFormViewModel model)
    {
        if (id != model.Id) return BadRequest();
        if (!ModelState.IsValid) return View(model);

        try
        {
            await _categoryService.UpdateAsync(new CategoryDto { Id = model.Id, Name = model.Name, Description = model.Description, IconUrl = model.IconUrl });
            TempData["StatusMessage"] = "Category updated.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to update category {Id}", id);
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(model);
        }
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _categoryService.DeleteAsync(id);
            TempData["StatusMessage"] = "Category deleted.";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = "Could not delete category: " + ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }
}
