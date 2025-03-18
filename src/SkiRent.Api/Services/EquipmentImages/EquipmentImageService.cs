using System.IO.Abstractions;

using FluentResults;

using Microsoft.Extensions.Options;

using SkiRent.Api.Configurations;
using SkiRent.Api.Data.Models;
using SkiRent.Api.Data.UnitOfWork;
using SkiRent.Api.Errors;
using SkiRent.Shared.Contracts.EquipmentImages;

namespace SkiRent.Api.Services.EquipmentImages;

public class EquipmentImageService : IEquipmentImageService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFileSystem _fileSystem;
    private readonly AppSettings _appSettings;

    public EquipmentImageService(IUnitOfWork unitOfWork, IFileSystem fileSystem, IOptions<AppSettings> appSettings)
    {
        _unitOfWork = unitOfWork;
        _fileSystem = fileSystem;
        _appSettings = appSettings.Value;
    }

    public async Task<Result<CreatedEquipmentImageResponse>> CreateAsync(IFormFile formFile)
    {
        string? createdFile = null;

        try
        {
            var image = new EquipmentImage
            {
                Id = Guid.NewGuid(),
                DisplayName = _fileSystem.Path.GetFileNameWithoutExtension(formFile.FileName)
            };

            var imagePath = _fileSystem.Path.Combine(_appSettings.DataDirectoryPath, "Images", $"{image.Id}.jpg");

            using var stream = _fileSystem.File.Create(imagePath);
            createdFile = imagePath;
            await formFile.CopyToAsync(stream);

            await _unitOfWork.EquipmentImages.AddAsync(image);

            var result = new CreatedEquipmentImageResponse
            {
                Id = image.Id,
                DisplayName = image.DisplayName
            };

            await _unitOfWork.SaveChangesAsync();

            return Result.Ok(result);
        }
        catch (Exception)
        {
            if (createdFile is not null)
            {
                _fileSystem.File.Delete(createdFile);
            }
            throw;
        }
    }

    public async Task<Result<IEnumerable<GetAllEquipmentImageResponse>>> GetAllAsync()
    {
        var equipmentImages = await _unitOfWork.EquipmentImages.GetAllAsync();

        var result = equipmentImages.Select(image =>
            new GetAllEquipmentImageResponse
            {
                Id = image.Id,
                DisplayName = image.DisplayName,
                CreatedAt = image.CreatedAt,
                UpdatedAt = image.UpdatedAt
            });

        return Result.Ok(result);
    }

    public async Task<Result<GetEquipmentImageResponse>> UpdateAsync(Guid imageId, UpdateEquipmentImageRequest request)
    {
        var imagePath = _fileSystem.Path.Combine(_appSettings.DataDirectoryPath, "Images", $"{imageId}.jpg");

        string? tempBackupFile = null;

        try
        {
            var image = await _unitOfWork.EquipmentImages.GetByIdAsync(imageId);

            if (image is null)
            {
                return Result.Fail("TODO");
            }

            if (request.Base64ImageData is not null)
            {
                byte[] newImage = Convert.FromBase64String(request.Base64ImageData);

                if (_fileSystem.File.Exists(imagePath))
                {
                    tempBackupFile = $"{imagePath}.bak";
                    _fileSystem.File.Copy(imagePath, tempBackupFile, overwrite: true);
                }

                using var stream = _fileSystem.File.Create(imagePath);
                using var memoryStream = new MemoryStream(newImage);
                await memoryStream.CopyToAsync(stream);
            }

            if (request.DisplayName is not null)
            {
                image.DisplayName = string.IsNullOrWhiteSpace(request.DisplayName)
                    ? null
                    : request.DisplayName;
            }

            await _unitOfWork.SaveChangesAsync();

            if (tempBackupFile is not null && _fileSystem.File.Exists(tempBackupFile))
            {
                _fileSystem.File.Delete(tempBackupFile);
            }

            var result = new GetEquipmentImageResponse
            {
                Id = image.Id,
                DisplayName = image.DisplayName,
                CreatedAt = image.CreatedAt,
                UpdatedAt = image.UpdatedAt
            };

            return Result.Ok(result);
        }
        catch (Exception)
        {
            if (tempBackupFile is not null && _fileSystem.File.Exists(tempBackupFile))
            {
                _fileSystem.File.Copy(tempBackupFile, imagePath, overwrite: true);
                _fileSystem.File.Delete(tempBackupFile);
            }
            throw;
        }
    }

    public async Task<Result> DeleteAsync(Guid imageId)
    {
        var image = await _unitOfWork.EquipmentImages.GetByIdAsync(imageId);

        if (image is null)
        {
            return Result.Fail(new EquipmentImageNotFound(imageId));
        }

        var hasEquipment = await _unitOfWork.Equipments.ExistsAsync(equipment => equipment.MainImageId == imageId);

        if (hasEquipment)
        {
            return Result.Fail(new EquipmentImageInUseError(imageId));
        }

        using var transaction = await _unitOfWork.BeginTransactionAsync();

        try
        {
            _unitOfWork.EquipmentImages.Delete(image);
            await _unitOfWork.SaveChangesAsync();

            var path = _fileSystem.Path.Combine(_appSettings.DataDirectoryPath, "Images", $"{image.Id}.jpg");
            _fileSystem.File.Delete(path);

            await transaction.CommitAsync();
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }

        return Result.Ok();
    }
}
