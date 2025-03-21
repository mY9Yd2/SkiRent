using System.IO.Abstractions;

using Microsoft.Extensions.FileProviders;

using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace SkiRent.Api.Configurations;

public static class StaticFileConfiguration
{
    public static StaticFileOptions Configure(AppSettings appSettings, IFileSystem fileSystem)
    {
        var options = new StaticFileOptions();

        string imagesPath = fileSystem.Path.Combine(appSettings.DataDirectoryPath, "Images");
        fileSystem.Directory.CreateDirectory(imagesPath);

        options.FileProvider = new PhysicalFileProvider(imagesPath);
        options.RequestPath = "/images";

        GeneratePlaceholderImage(appSettings.DataDirectoryPath, fileSystem);

        return options;
    }

    private static void GeneratePlaceholderImage(string path, IFileSystem fileSystem)
    {
        string placeholderImagePath = fileSystem.Path.Combine(path, "Images", "placeholder.jpg");

        if (!fileSystem.File.Exists(placeholderImagePath))
        {
            var image = CreatePlaceholderImage();
            image.SaveAsJpeg(placeholderImagePath);
        }
    }

    private static Image<Rgba32> CreatePlaceholderImage()
    {
        const int width = 750;
        const int height = 750;

        var image = new Image<Rgba32>(width, height);
        image.Mutate(ctx => ctx.Fill(Color.LightGray));

        var font = SystemFonts.CreateFont("Arial", 48);
        var textOptions = new RichTextOptions(font)
        {
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            Origin = new PointF(width / 2f, height / 2f)
        };

        image.Mutate(ctx => ctx.DrawText(textOptions, "Nincs kép", Color.Black));

        return image;
    }
}
