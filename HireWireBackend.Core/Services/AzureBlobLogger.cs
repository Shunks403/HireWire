using System.Text;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Specialized;
using HireWireBackend.Core.Interfaces.ILoggers;

namespace HireWireBackend.Core.Services;

public class AzureBlobLogger : IBlobLogger
{
    private readonly BlobContainerClient _blobContainerClient;

    public AzureBlobLogger(string connectionString, string containerName)
    {
        var blobServiceClient = new BlobServiceClient(connectionString);
        _blobContainerClient = blobServiceClient.GetBlobContainerClient(containerName);
        _blobContainerClient.CreateIfNotExists();
    }

    public async Task LogAsync(string message, string level = "INFO")
    {
        try
        {
            var blobName = $"logs/log-{DateTime.UtcNow:yyyy-MM-dd}.txt";
            var blobClient = _blobContainerClient.GetAppendBlobClient(blobName);

            try
            {
                // Проверяем, существует ли Append Blob
                if (!await blobClient.ExistsAsync())
                {
                    // Если блоб не существует, создаем новый
                    await blobClient.CreateAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error checking or creating blob '{blobName}': {ex.Message}");
                throw;
            }

            try
            {
                // Форматируем сообщение с уровнем логирования
                var formattedMessage = $"{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} [{level.ToUpper()}] {message}";

                // Записываем сообщение в Append Blob
                using var stream = new MemoryStream(Encoding.UTF8.GetBytes(formattedMessage + "\n"));
                await blobClient.AppendBlockAsync(stream);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error appending message to blob '{blobName}': {ex.Message}");
                throw;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unexpected error in LogAsync: {ex.Message}");
            throw;
        }
    }

   
}