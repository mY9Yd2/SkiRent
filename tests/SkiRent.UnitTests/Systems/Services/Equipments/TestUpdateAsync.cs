using AutoFixture;

using NSubstitute;

using SkiRent.Api.Data.Models;
using SkiRent.Api.Data.UnitOfWork;
using SkiRent.Api.Errors;
using SkiRent.Api.Services.Equipments;
using SkiRent.Shared.Contracts.Equipments;

namespace SkiRent.UnitTests.Systems.Services.Equipments;

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
        var equipmentId = _fixture.Create<int>();
        var categoryId = _fixture.Create<int>();

        var request = _fixture.Build<UpdateEquipmentRequest>()
            .With(request => request.CategoryId, categoryId)
            .Create();
        var equipment = _fixture.Create<Equipment>();

        _unitOfWork.Equipments.GetEquipmentWithCategoryAsync(equipmentId)
            .Returns(equipment);
        _unitOfWork.EquipmentCategories.GetByIdAsync(request.CategoryIdAsNonNull)
            .Returns((EquipmentCategory?)null);

        // Act
        var result = await _equipmentService.UpdateAsync(equipmentId, request);

        // Assert
        Assert.That(result.IsFailed, Is.True);
        Assert.That(result.Errors[0], Is.InstanceOf<EquipmentCategoryNotFoundError>());
        Assert.That(result.Errors[0].Metadata.GetValueOrDefault("categoryId"), Is.EqualTo(categoryId));
    }

    [Test]
    public async Task WhenEquipmentImageNotFound_ReturnsFailedResult()
    {
        // Arrange
        var equipmentId = _fixture.Create<int>();
        var imageId = Guid.NewGuid();

        var request = _fixture.Build<UpdateEquipmentRequest>()
            .With(request => request.MainImageId, imageId)
            .Without(request => request.CategoryId)
            .Create();
        var equipment = _fixture.Build<Equipment>()
            .With(equipment => equipment.MainImageId, Guid.NewGuid())
            .Create();

        _unitOfWork.Equipments.GetEquipmentWithCategoryAsync(equipmentId)
            .Returns(equipment);
        _unitOfWork.EquipmentImages.GetByIdAsync(imageId)
            .Returns((EquipmentImage?)null);

        // Act
        var result = await _equipmentService.UpdateAsync(equipmentId, request);

        // Assert
        Assert.That(result.IsFailed, Is.True);
        Assert.That(result.Errors[0], Is.InstanceOf<EquipmentImageNotFoundError>());
        Assert.That(result.Errors[0].Metadata.GetValueOrDefault("imageId"), Is.EqualTo(imageId));
    }
}
