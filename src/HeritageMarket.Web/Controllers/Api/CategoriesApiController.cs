using HeritageMarket.Application.DTOs;
using HeritageMarket.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HeritageMarket.Web.Controllers.Api;

[ApiController]
[Route("api/categories")]
[Produces("application/json")]
public class CategoriesApiController : ControllerBase
{
    private readonly ICategoryService _categoryService;

    public CategoriesApiController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    /// <summary>Lists all product categories.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<CategoryDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCategories() => Ok(await _categoryService.GetAllAsync());
}
