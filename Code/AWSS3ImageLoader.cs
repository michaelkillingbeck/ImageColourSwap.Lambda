using Amazon.S3;
using Amazon.S3.Model;
using ImageHelpers.Interfaces;
using ImageHelpers.Models;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace ImageColourSwap.Lambda;

public class AWSS3ImageLoader : IImageHandler
{
    private readonly SettingsModel _settings;

    public AWSS3ImageLoader(SettingsModel settings)
    {
        _settings = settings;
    }

    public RgbPixelData[] CreatePixelData(IImageData imageData)
    {
        using(Image<Rgba32> image = 
            Image.LoadPixelData<Rgba32>(
                imageData.Bytes, imageData.Size.Width, imageData.Size.Height))
        {
            Rgba32[] pixels = Array.Empty<Rgba32>();
            pixels = new Rgba32[image.Width * image.Height];
            image.CopyPixelDataTo(pixels);

            return pixels.Select(pixel => 
                new RgbPixelData{ R = pixel.R, G = pixel.G, B = pixel.B }).ToArray();
        }
    }

    public Stream GenerateStream(IImageData sourceImage)
    {
        using(Image<Rgba32> image = 
            Image.LoadPixelData<Rgba32>(
                sourceImage.Bytes, sourceImage.Size.Width, sourceImage.Size.Height))
        {
            MemoryStream stream = new MemoryStream();
            image.Save(stream, new JpegEncoder());

            return stream;
        }
    }

    public Stream GenerateStream(RgbPixelData[] pixels, IImageData imageData)
    {
        Rgba32[] imageSharpPixels =
            pixels.Select(pixel => new Rgba32 { R = pixel.R, G = pixel.G, B = pixel.B, A = 255 })
            .ToArray();

        using(Image<Rgba32> image = 
            Image.LoadPixelData(imageSharpPixels, imageData.Size.Width, imageData.Size.Height))
        {
            MemoryStream stream = new MemoryStream();
            image.Save(stream, new JpegEncoder());

            return stream;
        }
    }

    public Stream GenerateStream(string base64EncodedString)
    {
        throw new NotImplementedException();
    }

    public IImageData LoadImage(string filepath)
    {
        AmazonS3Client client = new AmazonS3Client(Amazon.RegionEndpoint.EUWest2);

        GetObjectRequest getRequest = new GetObjectRequest
        {
            BucketName = _settings.BucketName,
            Key = filepath
        };

        GetObjectResponse response = client.GetObjectAsync(getRequest).GetAwaiter().GetResult();

        using(Stream bytes = response.ResponseStream)
        {
            Image<Rgba32> image = Image.Load<Rgba32>(bytes, new JpegDecoder());

            byte[] byteArray = new byte[image.Width * image.Height * 4];
            image.CopyPixelDataTo(byteArray);

            return new InMemoryImageData($"{Guid.NewGuid()}.jpg", image.Width, image.Height, byteArray);
        }
    }

    public IImageData LoadImageFromBase64EncodedString(string base64EncodedString)
    {
        throw new NotImplementedException();
    }
    
    public IImageData Resize(IImageData sourceImage, IImageData targetImage)
    {
        using(Image<Rgba32> image = 
            Image.LoadPixelData<Rgba32>(sourceImage.Bytes, sourceImage.Size.Width, sourceImage.Size.Height))
        {
            image.Mutate(img => img.Resize(targetImage.Size.Width, targetImage.Size.Height));
            byte[] byteArray = new byte[image.Width * image.Height * 4];
            image.CopyPixelDataTo(byteArray);
            
            return new InMemoryImageData(sourceImage.Filename, image.Width, image.Height, byteArray);
        }
    }
}