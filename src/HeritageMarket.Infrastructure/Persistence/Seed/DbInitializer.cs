using HeritageMarket.Domain.Entities;
using HeritageMarket.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HeritageMarket.Infrastructure.Persistence.Seed;

public static class DbInitializer
{
    public static async Task SeedAsync(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        await context.Database.MigrateAsync();

        foreach (var roleName in new[] { IdentityRoles.Admin, IdentityRoles.Customer })
        {
            if (!await roleManager.RoleExistsAsync(roleName))
                await roleManager.CreateAsync(new IdentityRole(roleName));
        }

        const string adminEmail = "admin@heritagemarket.local";
        if (await userManager.FindByEmailAsync(adminEmail) is null)
        {
            var admin = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                FullName = "Platform Administrator",
                EmailConfirmed = true,
                CreatedAt = DateTime.UtcNow
            };

            var result = await userManager.CreateAsync(admin, "Admin@12345");
            if (result.Succeeded)
                await userManager.AddToRoleAsync(admin, IdentityRoles.Admin);
        }

        if (!await context.Countries.AnyAsync())
        {
            context.Countries.AddRange(
                new Country { Name = "Lebanon", Code = "LB", Description = "Levantine heritage: cedar motifs, mosaic art, and mountain craftsmanship.", FlagImageUrl = "https://picsum.photos/seed/country-lebanon/400/500" },
                new Country { Name = "Morocco", Code = "MA", Description = "North African heritage: Berber patterns, zellige tilework, and desert textiles.", FlagImageUrl = "https://picsum.photos/seed/country-morocco/400/500" },
                new Country { Name = "Japan", Code = "JP", Description = "East Asian heritage: washi paper, indigo dye, and minimalist ceramics.", FlagImageUrl = "https://picsum.photos/seed/country-japan/400/500" },
                new Country { Name = "Mexico", Code = "MX", Description = "Mesoamerican heritage: Talavera pottery, vibrant textiles, and folk art.", FlagImageUrl = "https://picsum.photos/seed/country-mexico/400/500" },
                new Country { Name = "India", Code = "IN", Description = "South Asian heritage: block printing, brassware, and handloom weaving.", FlagImageUrl = "https://picsum.photos/seed/country-india/400/500" }
            );
            await context.SaveChangesAsync();
        }

        if (!await context.Categories.AnyAsync())
        {
            context.Categories.AddRange(
                new Category { Name = "Home & Decoration", Description = "Heritage-inspired decor for the home.", IconUrl = "https://picsum.photos/seed/cat-home-decor/600/760" },
                new Category { Name = "Accessories", Description = "Traditional accessories with a modern edge.", IconUrl = "https://picsum.photos/seed/cat-accessories/600/760" },
                new Category { Name = "Phone Covers", Description = "Heritage-pattern phone covers.", IconUrl = "https://picsum.photos/seed/cat-phone-covers/600/760" },
                new Category { Name = "Wear & Traditional Clothing", Description = "Garments inspired by traditional dress.", IconUrl = "https://picsum.photos/seed/cat-wear/600/760" },
                new Category { Name = "Heritage Books", Description = "Books on history, culture, and traditions.", IconUrl = "https://picsum.photos/seed/cat-books/600/760" }
            );
            await context.SaveChangesAsync();
        }

        if (!await context.Products.AnyAsync())
        {
            var countries = await context.Countries.ToDictionaryAsync(c => c.Name, c => c.Id);
            var categories = await context.Categories.ToDictionaryAsync(c => c.Name, c => c.Id);

            context.Products.AddRange(
                new Product
                {
                    Name = "Cedar-Carved Wooden Box",
                    Description = "Hand-carved Lebanese cedar wood box with traditional inlay patterns.",
                    Price = 45.00m,
                    StockQuantity = 20,
                    SKU = "LB-HOME-001",
                    ImageUrl = "https://picsum.photos/seed/prod-cedar-box/600/600",
                    CategoryId = categories["Home & Decoration"],
                    CountryId = countries["Lebanon"]
                },
                new Product
                {
                    Name = "Zellige Mosaic Coasters (Set of 4)",
                    Description = "Handmade Moroccan zellige-inspired ceramic coasters.",
                    Price = 28.00m,
                    StockQuantity = 35,
                    SKU = "MA-HOME-002",
                    ImageUrl = "https://picsum.photos/seed/prod-zellige-coasters/600/600",
                    CategoryId = categories["Home & Decoration"],
                    CountryId = countries["Morocco"]
                },
                new Product
                {
                    Name = "Indigo Washi Phone Cover",
                    Description = "Phone cover featuring traditional Japanese indigo-dyed washi paper design.",
                    Price = 22.00m,
                    StockQuantity = 50,
                    SKU = "JP-PHONE-001",
                    ImageUrl = "https://picsum.photos/seed/prod-washi-phone/600/600",
                    CategoryId = categories["Phone Covers"],
                    CountryId = countries["Japan"]
                },
                new Product
                {
                    Name = "Talavera Pattern Phone Cover",
                    Description = "Vibrant Mexican Talavera-pattern phone cover.",
                    Price = 20.00m,
                    StockQuantity = 40,
                    SKU = "MX-PHONE-002",
                    ImageUrl = "https://picsum.photos/seed/prod-talavera-phone/600/600",
                    CategoryId = categories["Phone Covers"],
                    CountryId = countries["Mexico"]
                },
                new Product
                {
                    Name = "Block-Print Cotton Scarf",
                    Description = "Hand block-printed cotton scarf from Rajasthan, India.",
                    Price = 32.00m,
                    StockQuantity = 25,
                    SKU = "IN-ACC-001",
                    ImageUrl = "https://picsum.photos/seed/prod-block-print-scarf/600/600",
                    CategoryId = categories["Accessories"],
                    CountryId = countries["India"]
                },
                new Product
                {
                    Name = "Brass Filigree Earrings",
                    Description = "Traditional Indian brass filigree earrings.",
                    Price = 18.00m,
                    StockQuantity = 60,
                    SKU = "IN-ACC-002",
                    ImageUrl = "https://picsum.photos/seed/prod-brass-earrings/600/600",
                    CategoryId = categories["Accessories"],
                    CountryId = countries["India"]
                },
                new Product
                {
                    Name = "Embroidered Thobe",
                    Description = "Traditional Levantine embroidered thobe with tatreez patterns.",
                    Price = 120.00m,
                    StockQuantity = 12,
                    SKU = "LB-WEAR-001",
                    ImageUrl = "https://picsum.photos/seed/prod-thobe/600/600",
                    CategoryId = categories["Wear & Traditional Clothing"],
                    CountryId = countries["Lebanon"]
                },
                new Product
                {
                    Name = "Moroccan Kaftan",
                    Description = "Hand-embroidered Moroccan kaftan.",
                    Price = 95.00m,
                    StockQuantity = 15,
                    SKU = "MA-WEAR-002",
                    ImageUrl = "https://picsum.photos/seed/prod-kaftan/600/600",
                    CategoryId = categories["Wear & Traditional Clothing"],
                    CountryId = countries["Morocco"]
                },
                new Product
                {
                    Name = "The Art of Zellige: Moroccan Heritage",
                    Description = "An illustrated history of Moroccan zellige tilework and its cultural significance.",
                    Price = 34.00m,
                    StockQuantity = 30,
                    SKU = "MA-BOOK-001",
                    ImageUrl = "https://picsum.photos/seed/prod-zellige-book/600/600",
                    CategoryId = categories["Heritage Books"],
                    CountryId = countries["Morocco"]
                },
                new Product
                {
                    Name = "Cedars and Mosaics: A History of Lebanon",
                    Description = "A cultural history of Lebanon's heritage crafts and traditions.",
                    Price = 29.00m,
                    StockQuantity = 22,
                    SKU = "LB-BOOK-002",
                    ImageUrl = "https://picsum.photos/seed/prod-cedars-book/600/600",
                    CategoryId = categories["Heritage Books"],
                    CountryId = countries["Lebanon"]
                }
            );

            await context.SaveChangesAsync();
        }
    }
}
