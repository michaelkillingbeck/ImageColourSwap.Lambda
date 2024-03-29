﻿using Amazon.Lambda.Core;
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

            var originalFilenames = imageHelper.GetSourceAndPalletteImageFilenames();

            var resultsModel = new ProcessingResultsModel();
            resultsModel.OutputImage = outputFileName;
            resultsModel.SourceImage = originalFilenames.Item1;
            resultsModel.PalletteImage = originalFilenames.Item2;

            context.Logger.LogInformation("Returning Model");
            return resultsModel;
        }
    }
}