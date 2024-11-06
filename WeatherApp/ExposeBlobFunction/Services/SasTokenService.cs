using Azure.Storage.Blobs;
using Azure.Storage.Sas;

namespace ExposeBlobFunction.Services;
public class SasTokenService
{
    public Uri GenerateSasToken(BlobClient blobClient, TimeSpan expiryTime)
    {
        // Ensure the blob client is authenticated with the account key to generate SAS
        if (blobClient.CanGenerateSasUri)
        {
            // Define SAS token permissions and expiry
            var sasBuilder = new BlobSasBuilder
            {
                BlobContainerName = blobClient.BlobContainerName,
                BlobName = blobClient.Name,
                Resource = "b",
                ExpiresOn = DateTimeOffset.UtcNow.Add(expiryTime)
            };
            sasBuilder.SetPermissions(BlobSasPermissions.Read);

            // Generate the SAS URI
            return blobClient.GenerateSasUri(sasBuilder);
        }

        return null;
    }
}