using Azure.Storage.Blobs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VulpesX.Shared.Utilities
{
    public static class StorageHelperCustom
    {
        public static class Ufp
        {
            private const string ACCOUNT = "ufp";
            private const string ACCESS_KEY = "i/BYLiLFqMNJGHFz234KZ7kEFuI2gmN5V5ocrZHRNoemkZvu0MP30jJjmGT3txwCdRO15FRVh8+SZ/hBBKoADg==";
            private static BlobServiceClient client = new BlobServiceClient($"DefaultEndpointsProtocol=https;AccountName={ACCOUNT};AccountKey={ACCESS_KEY};EndpointSuffix=core.windows.net");

            public const string CLIENTI_FOLDER = "clienti";
            public const string DISEGNI_FOLDER = "disegni";

            public async static Task<byte[]?> DownloadAsync(string Container, string BlobName)
            {
                if (BlobName == null)
                    return (null);

                var blobContainer = client.GetBlobContainerClient(Container);

                await blobContainer.CreateIfNotExistsAsync();

                var blobClient = blobContainer.GetBlobClient(BlobName);

                using (var memoryStream = new MemoryStream())
                {
                    try
                    {
                        await blobClient.DownloadToAsync(memoryStream);
                        return (memoryStream.ToArray());
                    }
                    catch (Exception)
                    {
                        return null;
                    }
                }
            }

        }
    }
}
