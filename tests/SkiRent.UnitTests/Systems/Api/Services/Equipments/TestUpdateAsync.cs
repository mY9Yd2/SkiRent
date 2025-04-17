using AutoFixture;

using NSubstitute;

using SkiRent.Api.Data.Models;
using SkiRent.Api.Data.UnitOfWork;
using SkiRent.Api.Errors;
using SkiRent.Api.Services.Equipments;
using SkiRent.Shared.Contracts.Equipments;

namespace SkiRent.UnitTests.Systems.Api.Services.Equipments;

public class TestUpdateAsync
{
    private IUnitOfWork _unitOfWork;
    private Fixture _fixture;
    private IEquipmentService _equipmentService;

    [SetUp]
    public void Setup()
    {
        _fixture = new Fixture();

        _fixture.Behaviors
            .OfType<ThrowingRecursionBehavior>()
            .ToList()
            .ForEach(behavior => _fixture.Behaviors.Remove(behavior));
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        _unitOfWork = Substitute.For<IUnitOfWork>();
        _equipmentService = new EquipmentService(_unitOfWork);
    }

    [TearDown]
    public void TearDown()
    {
        _unitOfWork.Dispose();
    }

    [Test]
    public async Task WhenEquipmentNotFound_ReturnsFailedResult()
    {
        // Arrange
        var equipmentId = _fixture.Create<int>();

        var request = _fixture.Create<UpdateEquipmentRequest>();

        _unitOfWork.Equipments.GetEquipmentWithCategoryAsync(equipmentId).Returns((Equipment?)null);

        // Act
        var result = await _equipmentService.UpdateAsync(equipmentId, request);

        // Assert
        Assert.That(result.IsFailed, Is.True);
        Assert.That(result.Errors[0], Is.InstanceOf<EquipmentNotFoundError>());
        Assert.That(result.Errors[0].Metadata.GetValueOrDefault("equipmentId"), Is.EqualTo(equipmentId));
    }

    [Test]
    public async Task WhenEquipmentCategoryNotFound_ReturnsFailedResult()
    {
        // Arrange
        var categoryId = _fixture.Create<int>();

        var request = _fixture.Build<UpdateEquipmentRequest>()
            .With(request => request.CategoryId, categoryId)
            .Create();
        var equipment = _fixture.Create<Equipment>();

        _unitOfWork.Equipments.GetEquipmentWithCategoryAsync(equipment.Id)
            .Returns(equipment);
        _unitOfWork.EquipmentCategories.GetByIdAsync(request.CategoryIdAsNonNull)
            .Returns((EquipmentCategory?)null);

        // Act
        var result = await _equipmentService.UpdateAsync(equipment.Id, request);

        // Assert
        Assert.That(result.IsFailed, Is.True);
        Assert.That(result.Errors[0], Is.InstanceOf<EquipmentCategoryNotFoundError>());
        Assert.That(result.Errors[0].Metadata.GetValueOrDefault("categoryId"), Is.EqualTo(categoryId));
    }

    [Test]
    public async Task WhenEquipmentImageNotFound_ReturnsFailedResult()
    {
        // Arrange
        var imageId = _fixture.Create<Guid>();

        var request = _fixture.Build<UpdateEquipmentRequest>()
            .With(request => request.MainImageId, imageId)
            .Without(request => request.CategoryId)
            .Create();
        var equipment = _fixture.Build<Equipment>()
            .With(equipment => equipment.MainImageId, Guid.NewGuid())
            .Create();

        _unitOfWork.Equipments.GetEquipmentWithCategoryAsync(equipment.Id)
            .Returns(equipment);
        _unitOfWork.EquipmentImages.GetByIdAsync(imageId)
            .Returns((EquipmentImage?)null);

        // Act
        var result = await _equipmentService.UpdateAsync(equipment.Id, request);

        // Assert
        Assert.That(result.IsFailed, Is.True);
        Assert.That(result.Errors[0], Is.InstanceOf<EquipmentImageNotFoundError>());
        Assert.That(result.Errors[0].Metadata.GetValueOrDefault("imageId"), Is.EqualTo(imageId));
    }

    [Test]
    public async Task KeepsOriginalName_WhenNameNotProvided()
    {
        // Arrange
        var originalName = _fixture.Create<string>();
        var originalEquipment = _fixture.Build<Equipment>()
            .With(equipment => equipment.Name, originalName)
            .Create();

        var request = _fixture.Build<UpdateEquipmentRequest>()
            .With(request => request.Name, (string?)null)
            .Without(request => request.CategoryId)
            .Without(request => request.MainImageId)
            .Create();

        _unitOfWork.Equipments
            .GetEquipmentWithCategoryAsync(originalEquipment.Id)
            .Returns(originalEquipment);
        _unitOfWork.SaveChangesAsync()
            .Returns(Task.CompletedTask);

        // Act
        var result = await _equipmentService.UpdateAsync(originalEquipment.Id, request);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(originalEquipment.Name, Is.EqualTo(originalName));
    }

    [Test]
    public async Task SetsDescriptionToNull_WhenDescriptionIsEmptyString()
    {
        // Arrange
        var originalEquipment = _fixture.Create<Equipment>();

        var request = _fixture.Build<UpdateEquipmentRequest>()
            .With(request => request.Description, new string(' ', 4))
            .Without(request => request.CategoryId)
            .Without(request => request.MainImageId)
            .Create();

        _unitOfWork.Equipments
            .GetEquipmentWithCategoryAsync(originalEquipment.Id)
            .Returns(originalEquipment);
        _unitOfWork.SaveChangesAsync()
            .Returns(Task.CompletedTask);

        // Act
        var result = await _equipmentService.UpdateAsync(originalEquipment.Id, request);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(originalEquipment.Description, Is.Null);
    }

    [Test]
    public async Task RemovesMainImage_WhenMainImageIdIsNull()
    {
        // Arrange
        var image = _fixture.Create<EquipmentImage>();

        var originalEquipment = _fixture.Build<Equipment>()
            .With(equipment => equipment.MainImageId, image.Id)
            .With(equipment => equipment.MainImage, image)
            .Create();

        var request = _fixture.Build<UpdateEquipmentRequest>()
            .With(request => request.MainImageId, (Guid?)null)
            .Without(request => request.CategoryId)
            .Create();

        _unitOfWork.Equipments
            .GetEquipmentWithCategoryAsync(originalEquipment.Id)
            .Returns(originalEquipment);
        _unitOfWork.SaveChangesAsync()
            .Returns(Task.CompletedTask);

        // Act
        var result = await _equipmentService.UpdateAsync(originalEquipment.Id, request);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(originalEquipment.MainImage, Is.Null);
        Assert.That(originalEquipment.MainImageId, Is.Null);
    }
}
