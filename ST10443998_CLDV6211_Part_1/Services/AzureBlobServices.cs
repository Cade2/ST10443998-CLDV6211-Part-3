using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;

namespace ST10443998_CLDV6211_POE.Services
{
    public class AzureBlobService
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly string _containerName = "st10443998-eventease-images"; // or "eventimages" if you're handling events

        public AzureBlobService(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("AzureBlobStorage");
            _blobServiceClient = new BlobServiceClient(connectionString);
        }

        public async Task<string> UploadImageAsync(IFormFile file)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            await containerClient.CreateIfNotExistsAsync();

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var blobClient = containerClient.GetBlobClient(fileName);

            using (var stream = file.OpenReadStream())
            {
                await blobClient.UploadAsync(stream, overwrite: true);
            }

            return blobClient.Uri.ToString(); // this URL will be saved in your Venue.ImageUrl
        }
    }
}
