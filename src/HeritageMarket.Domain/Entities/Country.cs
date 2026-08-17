namespace HeritageMarket.Domain.Entities;

public class Country
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string? FlagImageUrl { get; set; }

    /// <summary>Photo of the country's most iconic landmark (e.g. the Eiffel Tower for France), used in the visual country picker.</summary>
    public string? LandmarkImageUrl { get; set; }
    public string? Description { get; set; }

    /// <summary>Heritage/geographic grouping used to organize the country filter (e.g. "Arab World", "Europe").</summary>
    public string Region { get; set; } = string.Empty;

    public ICollection<Product> Products { get; set; } = new List<Product>();
}
