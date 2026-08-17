namespace HeritageMarket.Domain.Entities;

public class Country
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string? FlagImageUrl { get; set; }
    public string? Description { get; set; }

    /// <summary>Heritage/geographic grouping used to organize the country filter (e.g. "Arab World", "Europe").</summary>
    public string Region { get; set; } = string.Empty;

    public ICollection<Product> Products { get; set; } = new List<Product>();
}
