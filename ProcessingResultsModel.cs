namespace ImageColourSwap.Lambda;

public class ProcessingResultsModel
{
    public string OutputImage { get; set; }
    public string PalletteImage { get; set; }
    public string SourceImage { get; set; }

    public ProcessingResultsModel()
    {
        OutputImage = String.Empty;
        PalletteImage = String.Empty;
        SourceImage = String.Empty;
    }
}