using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using ImageHelpers.Interfaces;
using System.Net;

namespace ImageColourSwap.Lambda;

public class S3ImageSaver : IImageSaver
{
    private readonly AmazonS3Client _client;
    private readonly SettingsModel _settings;

    public S3ImageSaver(SettingsModel settings)
    {
        _client = new AmazonS3Client(RegionEndpoint.EUWest2);
        _settings = settings;
    }

    public async Task<bool> SaveAsync(string filename, Stream imageStream)
    {
        try
        {
            PutObjectRequest putRequest = new PutObjectRequest
            {
                BucketName = _settings.BucketName,
                ContentType = "text/plain",
                InputStream = imageStream,
                Key = filename
            };

            PutObjectResponse putResponse = await _client.PutObjectAsync(putRequest);

            return putResponse.HttpStatusCode == HttpStatusCode.OK;
        }
        catch(Exception ex)
        {
            Console.Error.WriteLine($"Error saving {filename}");
            Console.Error.WriteLine(ex.Message);

            return false;            
        }
    }
}