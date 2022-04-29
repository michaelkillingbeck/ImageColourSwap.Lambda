using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Image_Colour_Swap.Interfaces;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace ImageColourSwap.Lambda;

public class AWSS3ImageLoader : IImageLoader
{
    public RgbPixelData[] CreatePixelData(IImageData imageData)
    {
        using(var image = Image.LoadPixelData<Rgba32>(imageData.Bytes, imageData.Size.Width, imageData.Size.Height))
        {
            Rgba32[] pixels = new Rgba32[0];
            pixels = new Rgba32[image.Width * image.Height];
            image.CopyPixelDataTo(pixels);

            return pixels.Select(pixel => new RgbPixelData{ R = pixel.R, G = pixel.G, B = pixel.B }).ToArray<RgbPixelData>();
        }
    }

    public Stream GenerateStream(IImageData sourceImage)
    {
        using(var image = Image.LoadPixelData<Rgba32>(sourceImage.Bytes, sourceImage.Size.Width, sourceImage.Size.Height))
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
            .ToArray<Rgba32>();

        using(var image = Image.LoadPixelData<Rgba32>(imageSharpPixels, imageData.Size.Width, imageData.Size.Height))
        {
            MemoryStream stream = new MemoryStream();
            image.Save(stream, new JpegEncoder());

            return stream;
        }
    }

    public IImageData LoadImage(string filepath)
    {
        var client = new AmazonS3Client(Amazon.RegionEndpoint.EUWest2);

        var getRequest = new GetObjectRequest
        {
            BucketName = "imagecolourswap",
            Key = filepath
        };

        var response = client.GetObjectAsync(getRequest).GetAwaiter().GetResult();

        using(var bytes = response.ResponseStream)
        {
            var image = SixLabors.ImageSharp.Image.Load<Rgba32>(bytes, new JpegDecoder());

            var byteArray = new byte[image.Width * image.Height * 4];
            image.CopyPixelDataTo(byteArray);

            return new InMemoryImageData($"{Guid.NewGuid().ToString()}.jpg", image.Width, image.Height, byteArray);
        }
    }

    public IImageData Resize(IImageData sourceImage, IImageData targetImage)
    {
        using(var image = 
            Image.LoadPixelData<Rgba32>(sourceImage.Bytes, sourceImage.Size.Width, sourceImage.Size.Height))
            {
                image.Mutate(img => img.Resize(targetImage.Size.Width, targetImage.Size.Height));
                var byteArray = new byte[image.Width * image.Height * 4];
                image.CopyPixelDataTo(byteArray);
                
                return new InMemoryImageData(sourceImage.Filename, image.Width, image.Height, byteArray);
            }
    }
}