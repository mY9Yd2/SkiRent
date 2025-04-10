using AutoFixture;

using NSubstitute;

using SkiRent.Api.Data.Models;
using SkiRent.Api.Data.UnitOfWork;
using SkiRent.Api.Errors;
using SkiRent.Api.Services.Equipments;

namespace SkiRent.UnitTests.Systems.Services.Equipments;

public class TestDeleteAsync
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

        _unitOfWork.Equipments.GetEquipmentWithCategoryAsync(equipmentId).Returns((Equipment?)null);

        // Act
        var result = await _equipmentService.DeleteAsync(equipmentId);

        // Assert
        Assert.That(result.IsFailed, Is.True);
        Assert.That(result.Errors[0], Is.InstanceOf<EquipmentNotFoundError>());
        Assert.That(result.Errors[0].Metadata.GetValueOrDefault("equipmentId"), Is.EqualTo(equipmentId));
    }

    [Test]
    public async Task WhenEquipmentExists_DeletesEquipmentAndSavesChanges()
    {
        // Arrange
        var equipment = _fixture.Create<Equipment>();

        _unitOfWork.Equipments.GetEquipmentWithCategoryAsync(equipment.Id).Returns(equipment);

        // Act
        var result = await _equipmentService.DeleteAsync(equipment.Id);

        // Assert
        Assert.That(result.IsFailed, Is.False);
        _unitOfWork.Equipments.Received(1).Delete(equipment);
        await _unitOfWork.Received(1).SaveChangesAsync();
    }
}
