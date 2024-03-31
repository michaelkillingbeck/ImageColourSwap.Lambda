namespace ImageColourSwap.Lambda;

public class SettingsModel
{
    public string BucketName { get; set; }

    public SettingsModel()
    {
        BucketName = string.Empty;
    }
}