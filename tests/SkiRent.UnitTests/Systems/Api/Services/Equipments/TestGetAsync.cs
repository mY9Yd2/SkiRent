using AutoFixture;

using NSubstitute;

using SkiRent.Api.Data.Models;
using SkiRent.Api.Data.UnitOfWork;
using SkiRent.Api.Errors;
using SkiRent.Api.Services.Equipments;

namespace SkiRent.UnitTests.Systems.Api.Services.Equipments;

public class TestGetAsync
{
    private IUnitOfWork _unitOfWork;
    private Fixture _fixture;
    private IEquipmentService _equipmentService;

    [SetUp]
    public void Setup()
    {
        _fixture = new Fixture();
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
        var result = await _equipmentService.GetAsync(equipmentId);

        // Assert
        Assert.That(result.IsFailed, Is.True);
        Assert.That(result.Errors[0], Is.InstanceOf<EquipmentNotFoundError>());
        Assert.That(result.Errors[0].Metadata.GetValueOrDefault("equipmentId"), Is.EqualTo(equipmentId));
    }
}
