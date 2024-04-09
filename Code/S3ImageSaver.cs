using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using ImageHelpers.Interfaces;
using System.Net;

namespace ImageColourSwap.Lambda;

public class S3ImageSaver(SettingsModel settingsModel) : IImageSaver, IDisposable
{
    private readonly AmazonS3Client _client = new(RegionEndpoint.EUWest2);
    private bool _isDisposed;
    private readonly SettingsModel _settings = settingsModel;

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
            await Console.Error.WriteLineAsync($"Error saving {filename}").ConfigureAwait(false);
            await Console.Error.WriteLineAsync(ex.Message).ConfigureAwait(false);

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