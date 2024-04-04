namespace ImageColourSwap.Lambda;

public class FileInputModel
{
    public string PalletteImage { get; set; }

    public string SourceImage { get; set; }

    public FileInputModel()
    {
        PalletteImage = string.Empty;
        SourceImage = string.Empty;
    }
}