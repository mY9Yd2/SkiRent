using System.Net.Mime;

using FluentValidation;

using Microsoft.AspNetCore.Http;

using SixLabors.ImageSharp;

namespace SkiRent.Shared.Validators.Common.EquipmentImages
{
    public class EquipmentImageValidator : AbstractValidator<IFormFile>
    {
        private static readonly Dictionary<string, List<byte[]>> FileSignatures = new()
        {
            {
                ".jpeg", new List<byte[]>
                {
                    new byte[] { 0xFF, 0xD8, 0xFF }
                }
            },
            {
                ".jpg", new List<byte[]>
                {
                    new byte[] { 0xFF, 0xD8, 0xFF }
                }
            }
        };

        public EquipmentImageValidator()
        {
            RuleFor(imageFile => imageFile.FileName)
                .NotEmpty()
                .Must(HaveValidExtension)
                .Length(1, 255)
                .WithMessage("Only .jpg and .jpeg images are allowed, and the file name must not exceed 255 characters.");

            RuleFor(imageFile => imageFile.Length)
                .NotEmpty()
                // 100 Kilobyte
                .LessThanOrEqualTo(100_000)
                .WithMessage("File size must not exceed 100 kilobyte.");

            RuleFor(imageFile => imageFile.ContentType)
                .NotEmpty()
                .Equal(MediaTypeNames.Image.Jpeg)
                .WithMessage("Invalid MIME type. Only JPEG images are allowed.");

            RuleFor(imageFile => imageFile)
                .Must(VerifyFileSignature)
                .WithMessage("Invalid image file format.");

            RuleFor(imageFile => imageFile)
                .Must(HaveValidDimensions)
                .WithMessage("Image dimensions must be between 150x150 and 750x750 pixels (inclusive).");
        }

        public static bool HaveValidDimensions(IFormFile file)
        {
            int maxWidth = 750;
            int maxHeight = 750;
            int minWidth = 150;
            int minHeight = 150;

            try
            {
                using var image = Image.Load(file.OpenReadStream());
                return image.Width >= minWidth && image.Width <= maxWidth &&
                    image.Height >= minHeight && image.Height <= maxHeight;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static bool HaveValidExtension(string fileName)
        {
            var extension = Path.GetExtension(fileName).ToLower();
            return FileSignatures.ContainsKey(extension);
        }

        private static bool VerifyFileSignature(IFormFile file)
        {
            var extension = Path.GetExtension(file.FileName).ToLower();

            if (!FileSignatures.TryGetValue(extension, out var signatures))
            {
                return false;
            }

            using var reader = new BinaryReader(file.OpenReadStream());
            var headerBytes = reader.ReadBytes(signatures.Max(max => max.Length));

            return signatures.Any(signature =>
                headerBytes.Take(signature.Length).SequenceEqual(signature));
        }
    }
}
