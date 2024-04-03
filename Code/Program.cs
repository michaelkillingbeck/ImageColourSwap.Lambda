using Amazon.Lambda.Core;
using ImageHelpers.Services.ImageColourSwap;

namespace ImageColourSwap.Lambda
{
    public class Program
    {
        public static void Main()
        {
            
        }

        [LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]
        public ProcessingResultsModel Handler(FileInputModel input, ILambdaContext context)
        {
            context.Logger.LogInformation($"Pallette Image : {input.PalletteImage}");
            context.Logger.LogInformation($"Source Image : {input.SourceImage}");

            SettingsModel settings = new SettingsModel
            {
                BucketName = Environment.GetEnvironmentVariable("BucketName") ?? string.Empty
            };

            ImageColourSwapImageHelper imageHelper = new ImageColourSwapImageHelper(
                new AWSS3ImageLoader(settings),
                new S3ImageSaver(settings));

            imageHelper.LoadImages(
                input.SourceImage, 
                input.PalletteImage);

            imageHelper.Resize();
            context.Logger.LogInformation("Finished resizing image(s)");

            bool _ = imageHelper.SaveImagesAsync().GetAwaiter().GetResult();
            context.Logger.LogInformation("Images Saved");

            imageHelper.CreateSortedImages().GetAwaiter().GetResult();
            string outputFileName = imageHelper.CreateOutputImage().GetAwaiter().GetResult();

            Tuple<string, string> originalFilenames = imageHelper.GetSourceAndPalletteImageFilenames();

            ProcessingResultsModel resultsModel = new ProcessingResultsModel
            {
                OutputImage = outputFileName,
                SourceImage = originalFilenames.Item1,
                PalletteImage = originalFilenames.Item2
            };

            context.Logger.LogInformation("Returning Model");
            return resultsModel;
        }
    }
}