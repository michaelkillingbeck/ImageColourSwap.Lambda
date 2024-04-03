namespace ImageColourSwap.Lambda;

public class ProcessingResultsModel
{
    public string OutputImage { get; set; }
    public string PalletteImage { get; set; }
    public string SourceImage { get; set; }

    public ProcessingResultsModel()
    {
        OutputImage = string.Empty;
        PalletteImage = string.Empty;
        SourceImage = string.Empty;
    }
}