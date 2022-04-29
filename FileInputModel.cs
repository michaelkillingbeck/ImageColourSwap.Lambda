namespace ImageColourSwap.Lambda;

public class FileInputModel
{
    public string PalletteImage { get; set; }
    public string SourceImage { get; set; }

    public FileInputModel()
    {
        PalletteImage = String.Empty;
        SourceImage = String.Empty;
    }
}