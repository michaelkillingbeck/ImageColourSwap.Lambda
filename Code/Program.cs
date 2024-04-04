using Amazon.Lambda.Core;
using ImageHelpers.Services.ImageColourSwap;

namespace ImageColourSwap.Lambda
{
    public class Program
    {
#pragma warning disable S1186 // Methods should not be empty
        public static void Main()
#pragma warning restore S1186 // Methods should not be empty
        {
        }

        [LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]
#pragma warning disable CA1822 // Mark members as static
        public ProcessingResultsModel Handler(FileInputModel input, ILambdaContext context)
#pragma warning restore CA1822 // Mark members as static
        {
            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (input is null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            context.Logger.LogInformation($"Pallette Image : {input.PalletteImage}");
            context.Logger.LogInformation($"Source Image : {input.SourceImage}");

            SettingsModel settings = new()
            {
                BucketName = Environment.GetEnvironmentVariable("BucketName") ?? string.Empty,
            };

            AWSS3ImageLoader imageLoader = new(settings);
            S3ImageSaver imageSaver = new(settings);

            ImageColourSwapImageHelper imageHelper = new(
                imageLoader,
                imageSaver);

            imageHelper.LoadImages(
                input.SourceImage,
                input.PalletteImage);

            imageHelper.Resize();
            context.Logger.LogInformation("Finished resizing image(s)");

            bool result = imageHelper.SaveImagesAsync().GetAwaiter().GetResult();

            if (!result)
            {
                imageSaver.Dispose();

                return new ProcessingResultsModel
                {
                    Success = false,
                };
            }

            context.Logger.LogInformation("Images Saved");

            imageHelper.CreateSortedImages().GetAwaiter().GetResult();
            string outputFileName = imageHelper.CreateOutputImage().GetAwaiter().GetResult();

            Tuple<string, string> originalFilenames = imageHelper.GetSourceAndPalletteImageFilenames();

            ProcessingResultsModel resultsModel = new()
            {
                OutputImage = outputFileName,
                SourceImage = originalFilenames.Item1,
                PalletteImage = originalFilenames.Item2,
            };

            imageSaver.Dispose();
            context.Logger.LogInformation("Returning Model");
            return resultsModel;
        }
    }
}