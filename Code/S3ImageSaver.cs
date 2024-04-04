using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using ImageHelpers.Interfaces;
using System.Net;

namespace ImageColourSwap.Lambda;

public class S3ImageSaver : IImageSaver, IDisposable
{
    private readonly AmazonS3Client _client;
    private bool _isDisposed;
    private readonly SettingsModel _settings;

    public S3ImageSaver(SettingsModel settings)
    {
        _client = new AmazonS3Client(RegionEndpoint.EUWest2);
        _isDisposed = false;
        _settings = settings;
    }

    public async Task<bool> SaveAsync(string filename, Stream imageStream)
    {
        try
        {
            PutObjectRequest putRequest = new()
            {
                BucketName = _settings.BucketName,
                ContentType = "text/plain",
                InputStream = imageStream,
                Key = filename,
            };

            PutObjectResponse putResponse = await _client.PutObjectAsync(putRequest).ConfigureAwait(false);

            return putResponse.HttpStatusCode == HttpStatusCode.OK;
        }
        catch (AmazonS3Exception ex)
        {
            Console.Error.WriteLine($"Error saving {filename}");
            Console.Error.WriteLine(ex.Message);

            return false;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_isDisposed)
        {
            return;
        }

        if (disposing)
        {
            _client.Dispose();
        }

        _isDisposed = true;
    }
}