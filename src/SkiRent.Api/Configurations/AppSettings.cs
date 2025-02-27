namespace SkiRent.Api.Configurations;

public class AppSettings
{
    private string _dataDirectoryPath = string.Empty;

    public required string MerchantName { get; set; }

    public required Uri BaseUrl { get; set; }

    public required string DataDirectoryPath
    {
        get => _dataDirectoryPath;

        set => _dataDirectoryPath = string.IsNullOrEmpty(value)
            ? Path.GetFullPath(Path.GetTempPath())
            : Path.GetFullPath(value);
    }
}
