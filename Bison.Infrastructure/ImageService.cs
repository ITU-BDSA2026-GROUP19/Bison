using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Linq;

public class ImageService : IImageService
{
    private readonly IWebHostEnvironment _environment;

    public ImageService(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    public Task<bool> ValidateImageAsync(IFormFile imageFile)
    {
        if (imageFile == null || imageFile.Length == 0)
        {
            return Task.FromResult(false);
        }

        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
        var extension = Path.GetExtension(imageFile.FileName).ToLowerInvariant();

        if (!allowedExtensions.Contains(extension))
        {
            return Task.FromResult(false);
        }

        if (imageFile.Length > 5 * 1024 * 1024) // 5 MB limit
        {
            return Task.FromResult(false);
        }

        return Task.FromResult(true);
    }


    public async Task<string> SaveImageAsync(IFormFile imageFile)
    {
        if (!await ValidateImageAsync(imageFile))
        {
            throw new ArgumentException("Invalid image file.");
        }

        var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "observationPictures");
        if (!Directory.Exists(uploadsFolder))
        {
            Directory.CreateDirectory(uploadsFolder);
        }

        var extension = Path.GetExtension(imageFile.FileName).ToLowerInvariant();
        var uniqueFileName = Guid.NewGuid().ToString() + extension;
        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

        using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
            await imageFile.CopyToAsync(fileStream);
        }

        return "/uploads/observationPictures/" + uniqueFileName;
    }
}