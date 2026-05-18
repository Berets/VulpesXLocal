using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System.Threading.Tasks;

namespace VulpesX.Shared.Utilities;

public class StorageHelper
{
    private const string ACCOUNT = "gxfs";
    private const string ACCESS_KEY = "edOH7lLXt0Ecu182fNadAQh7xQMxA0dh1tGMZwvo6m9R+zIXvgDEPGUL/IThatdu9v7sTioGMhoNzMcuqUqXPw==";
    private static BlobServiceClient client = new BlobServiceClient("DefaultEndpointsProtocol=https;AccountName=gxfs;AccountKey=edOH7lLXt0Ecu182fNadAQh7xQMxA0dh1tGMZwvo6m9R+zIXvgDEPGUL/IThatdu9v7sTioGMhoNzMcuqUqXPw==;EndpointSuffix=core.windows.net");

    public const string VULPESX_DATA_CONTAINER = "vulpesxdata";

    public const string ACTIVITIES_ATTACHMENTS_FOLDER = "Activities attachments/";
    public const string OFFERS_ATTACHMENTS_FOLDER = "Offers attachments/";
    public const string ORDERS_ATTACHMENTS_FOLDER = "Orders attachments/";
    public const string INVOICE_ATTACHMENTS_FOLDER = "Invoice attachments/";
    public const string INVOICE_RECEIVED_FOLDER = "Invoice received/";
    public const string INVOICE_EXTERNAL_SENT_FOLDER = "Invoice external sent/";
    public const string DDT_ATTACHMENTS_FOLDER = "DDT attachments/";
    public const string ASSETS_ATTACHMENTS_FOLDER = "Assets attachments/";
    public const string BUY_ATTACHMENTS_FOLDER = "Purchase orders attachments/";
    public const string CUSTOM_FOLDER = "Custom/";

    public static string? Upload(string Container, string BlobName, byte[] Data)
    {
        try
        {
            var blobContainer = client.GetBlobContainerClient(Container);
            blobContainer.CreateIfNotExistsAsync().Wait();
            blobContainer.UploadBlobAsync(BlobName, new MemoryStream(Data)).Wait();
            return null;
        }
        catch (Exception exc)
        {
            return $"{exc.Message} {exc.InnerException?.Message}";
        }
    }

    public static byte[]? Download(string Container, string BlobName)
    {
        try
        {
            if (BlobName == null)
                return (null);
            var blobContainer = client.GetBlobContainerClient(Container);
            blobContainer.CreateIfNotExistsAsync().Wait();
            var blobClient = blobContainer.GetBlobClient(BlobName);
            using (var memoryStream = new MemoryStream())
            {
                blobClient.DownloadToAsync(memoryStream).Wait();
                return (memoryStream.ToArray());
            }
        }
        catch
        { return null; }
    }

    public async static Task<byte[]?> DownloadAsync(string Container, string BlobName)
    {
        if (BlobName == null)
            return (null);
        var blobContainer = client.GetBlobContainerClient(Container);
        await blobContainer.CreateIfNotExistsAsync();
        var blobClient = blobContainer.GetBlobClient(BlobName);
        using (var memoryStream = new MemoryStream())
        {
            await blobClient.DownloadToAsync(memoryStream);
            return (memoryStream.ToArray());
        }
    }

    public static byte[]? DownloadThrough(string Container, string BlobName)
    {
        if (BlobName == null)
            return (null);
        var blobContainer = client.GetBlobContainerClient(Container);
        blobContainer.CreateIfNotExistsAsync().Wait();
        var blobClient = blobContainer.GetBlobClient(BlobName);
        MemoryStream ms = new MemoryStream();
        blobClient.DownloadToAsync(ms).Wait();
        return ms.ToArray();
    }

    public static void Delete(string Container, string BlobName)
    {
        var blobContainer = client.GetBlobContainerClient(Container);
        blobContainer.CreateIfNotExistsAsync().Wait();
        var blobClient = blobContainer.GetBlobClient(BlobName);
        blobClient.DeleteIfExistsAsync();
    }

    public static async Task<AzureContainerInfo> GetContainerSizeAsync(string Container)
    {
        BlobServiceClient bcli = new BlobServiceClient("DefaultEndpointsProtocol=https;AccountName=gxfs;AccountKey=edOH7lLXt0Ecu182fNadAQh7xQMxA0dh1tGMZwvo6m9R+zIXvgDEPGUL/IThatdu9v7sTioGMhoNzMcuqUqXPw==;EndpointSuffix=core.windows.net");
        var blobContainer = bcli.GetBlobContainerClient(Container);
        long totalSize = 0;
        long blobCount = 0;
        if (blobContainer.Exists())
        {
            var resultSegment = blobContainer.GetBlobsAsync().AsPages(default, 100);
            await foreach (Azure.Page<BlobItem> blobPage in resultSegment)
            {
                foreach (BlobItem blobItem in blobPage.Values)
                {
                    if (blobItem.Properties.ContentLength.HasValue)
                    {
                        totalSize += blobItem.Properties.ContentLength.Value;
                        blobCount++;
                    }
                }
            }
        }
        return new AzureContainerInfo(totalSize) { BlobCount = blobCount };
    }
}
 
public class AzureContainerInfo
{
    public long Size { get; set; }
    public long BlobCount { get; set; }
    public AzureContainerInfo(long LongSize) => Size = LongSize;

    public override string ToString()
    {
        return FileHelper.FilesizeGetText(Size);
    }
}
