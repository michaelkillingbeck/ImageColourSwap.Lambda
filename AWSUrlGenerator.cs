using Amazon.S3;
using Amazon.S3.Model;

namespace ImageColourSwap.Lambda;

public class AWSUrlGenerator
{
    public string GetUrl(string bucketName, string objectName)
    {
        GetPreSignedUrlRequest request = new GetPreSignedUrlRequest
        {
            BucketName = bucketName,
            Expires = DateTime.UtcNow.AddMinutes(10),
            Key = objectName
        };

        AmazonS3Client client = new AmazonS3Client(Amazon.RegionEndpoint.EUWest2);
        
        return client.GetPreSignedURL(request);
    }
}