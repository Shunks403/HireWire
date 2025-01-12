using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using HireWireBackend.Core.Interfaces.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;


namespace HireWireBackend.Core.Services;

public class BlobStorageService : IBlobStorageService
{
    private readonly string _connectionString;
    private readonly string _containerName;

    public BlobStorageService(IConfiguration configuration)
    {
        _connectionString = configuration["AzureBlobStorage:ConnectionString"];
        _containerName = configuration["AzureBlobStorage:ContainerName"];
    }

    public async Task<string> UploadFileAsync(IFormFile file)
    {
        var blobServiceClient = new BlobServiceClient(_connectionString);
        var blobContainerClient = blobServiceClient.GetBlobContainerClient(_containerName);

        // Проверяем, что контейнер существует
        await blobContainerClient.CreateIfNotExistsAsync(PublicAccessType.None);

        var blobClient = blobContainerClient.GetBlobClient(Guid.NewGuid() + Path.GetExtension(file.FileName));

        using (var stream = file.OpenReadStream())
        {
            await blobClient.UploadAsync(stream, new BlobHttpHeaders { ContentType = file.ContentType });
        }

        return blobClient.Uri.ToString();
    }
    
    public async Task DeleteFileAsync(string fileUrl)
    {
        // Получаем имя файла из URL
        var blobName = GetBlobNameFromUrl(fileUrl);

        // Получаем ссылку на контейнер и удаляем файл
        var blobServiceClient = new BlobServiceClient(_connectionString);
        var containerClient = blobServiceClient.GetBlobContainerClient(_containerName);
        var blobClient = containerClient.GetBlobClient(blobName);

        await blobClient.DeleteIfExistsAsync();
    }

    private string GetBlobNameFromUrl(string fileUrl)
    {
        // Извлекаем имя файла из URL
        var uri = new Uri(fileUrl);
        return Path.GetFileName(uri.LocalPath);
    }
    
}