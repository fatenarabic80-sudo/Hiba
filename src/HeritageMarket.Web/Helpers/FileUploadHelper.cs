namespace HeritageMarket.Web.Helpers;

public static class FileUploadHelper
{
    private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".jpg", ".jpeg", ".png", ".webp"
    };

    private const long MaxFileSizeBytes = 5 * 1024 * 1024;

    public static async Task<string?> SaveImageAsync(IFormFile? file, IWebHostEnvironment environment, string subFolder)
    {
        if (file is null || file.Length == 0) return null;

        var extension = Path.GetExtension(file.FileName);
        if (!AllowedExtensions.Contains(extension))
            throw new InvalidOperationException("Only .jpg, .jpeg, .png, and .webp images are allowed.");

        if (file.Length > MaxFileSizeBytes)
            throw new InvalidOperationException("Image files must be 5 MB or smaller.");

        var uploadsRoot = Path.Combine(environment.WebRootPath, "uploads", subFolder);
        Directory.CreateDirectory(uploadsRoot);

        var fileName = $"{Guid.NewGuid()}{extension}";
        var filePath = Path.Combine(uploadsRoot, fileName);

        await using var stream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(stream);

        return $"/uploads/{subFolder}/{fileName}";
    }
}
