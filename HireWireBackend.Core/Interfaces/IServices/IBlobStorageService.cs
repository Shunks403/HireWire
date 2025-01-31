using Microsoft.AspNetCore.Http;

namespace HireWireBackend.Core.Interfaces.IServices;

public interface IBlobStorageService
{
    public Task<string> UploadFileAsync(IFormFile file);
    public Task DeleteFileAsync(string fileUrl);
}

