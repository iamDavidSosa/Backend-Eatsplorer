using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

public class ImageUploadService
{
    private readonly string _connectionString;

    public ImageUploadService(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("AzureBlobStorage");
    }

    public async Task<string> UploadImageAsync(IFormFile file, string containerName)
    {
        var blobServiceClient = new BlobServiceClient(_connectionString);
        var containerClient = blobServiceClient.GetBlobContainerClient(containerName);

        var blobClient = containerClient.GetBlobClient(file.FileName);
        await blobClient.UploadAsync(file.OpenReadStream());

        return blobClient.Uri.ToString();
    }
}

