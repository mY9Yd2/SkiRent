using System.Net.Mime;

using FluentValidation;

using Microsoft.AspNetCore.Http;

using SkiRent.Shared.Contracts.EquipmentImages;
using SkiRent.Shared.Validators.Common.EquipmentImages;

namespace SkiRent.Shared.Validators.EquipmentImages
{
    public class UpdateEquipmentImageRequestValidator : AbstractValidator<UpdateEquipmentImageRequest>
    {
        public UpdateEquipmentImageRequestValidator()
        {
            RuleFor(request => request.DisplayName)
                .MaximumLength(255)
                .When(request => request.DisplayName is not null);

            When(request => request.Base64ImageData is not null, () =>
            {
                RuleFor(request => request.Base64ImageData)
                    .NotEmpty()
                    .WithMessage("Base64 image data is required.")
                    .Must(IsValidBase64)
                    .WithMessage("Invalid Base64 format.")
                    .Custom((base64, context) =>
                    {
                        ArgumentNullException.ThrowIfNull(base64);

                        IFormFile file = ConvertBase64ToFormFile(base64);

                        var equipmentImageValidator = new EquipmentImageValidator();

                        var validationResult = equipmentImageValidator.Validate(file);

                        if (!validationResult.IsValid)
                        {
                            foreach (var error in validationResult.Errors)
                            {
                                context.AddFailure(error.ErrorMessage);
                            }
                        }
                    });
            });
        }

        private static bool IsValidBase64(string? base64String)
        {
            if (string.IsNullOrWhiteSpace(base64String))
            {
                return false;
            }

            try
            {
                Convert.FromBase64String(base64String);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }

        private static IFormFile ConvertBase64ToFormFile(string base64)
        {
            byte[] bytes = Convert.FromBase64String(base64);
            var stream = new MemoryStream(bytes);

            return new FormFile(stream, 0, stream.Length, "formFile", "tempFileName.jpg")
            {
                Headers = new HeaderDictionary(),
                ContentType = MediaTypeNames.Image.Jpeg
            };
        }
    }
}
