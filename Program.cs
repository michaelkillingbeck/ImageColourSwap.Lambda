using Amazon.Lambda.Core;

namespace ImageColourSwap.Lambda
{
    public class Program
    {
        public static void Main()
        {

        }

        [LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]
        public string Handler(Stream request)
        {
            ImageColourSwapCore core = new ImageColourSwapCore();

            return "Testing...testing 1 2 3";
        }
    }
}