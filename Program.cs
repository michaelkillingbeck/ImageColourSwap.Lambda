using Amazon.Lambda.Core;

namespace ImageColourSwap.Lambda
{
    public class Program
    {
        public static void Main()
        {
            
        }

        [LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]
        public string Handler(FileInputModel input, ILambdaContext context)
        {
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
            imageHelper.CreateOutputImage().GetAwaiter().GetResult();

            return "Execution finished";
        }
    }
}