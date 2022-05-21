using Amazon.Lambda.Core;
using Amazon.S3;
using Amazon.S3.Model;

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
            context.Logger.LogInformation("Testing version 1...");
            context.Logger.LogInformation($"Pallette Image : {input.PalletteImage}");
            context.Logger.LogInformation($"Source Image : {input.SourceImage}");

            var imageHelper = new ImageHelper(new AWSS3ImageLoader());

            imageHelper.LoadImages(
                input.SourceImage, 
                input.PalletteImage);

            imageHelper.Resize();
            context.Logger.LogInformation("Finished resizing image(s)");

            var result = imageHelper.SaveImagesAsync().GetAwaiter().GetResult();
            context.Logger.LogInformation("Images Saved");

            imageHelper.CreateSortedImages().GetAwaiter().GetResult();
            var outputFileName = imageHelper.CreateOutputImage().GetAwaiter().GetResult();

            var bucketName = "imagecolourswap";
            var originalFilenames = imageHelper.GetSourceAndPalletteImageFilenames();
            var urlGenerator = new AWSUrlGenerator();
            var resultsModel = new ProcessingResultsModel();
            context.Logger.LogInformation($"Getting url for {outputFileName}");
            resultsModel.OutputImage = urlGenerator.GetUrl(bucketName, outputFileName);
            context.Logger.LogInformation($"Getting url for {originalFilenames.Item1}");
            resultsModel.PalletteImage = urlGenerator.GetUrl(bucketName, originalFilenames.Item2);
            context.Logger.LogInformation($"Getting url for {originalFilenames.Item2}");
            resultsModel.SourceImage = urlGenerator.GetUrl(bucketName, originalFilenames.Item1);

            context.Logger.LogInformation("Returning Model");

            return resultsModel;
        }
    }
}