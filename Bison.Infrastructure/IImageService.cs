using Microsoft.AspNetCore.Http;

public interface IImageService
{
    Task<bool> ValidateImageAsync(IFormFile imageFile);
    Task<string> SaveImageAsync(IFormFile imageFile);
}