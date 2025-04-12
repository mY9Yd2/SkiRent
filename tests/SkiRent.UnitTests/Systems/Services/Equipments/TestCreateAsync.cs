using System.Linq.Expressions;

using AutoFixture;

using NSubstitute;

using SkiRent.Api.Data.Models;
using SkiRent.Api.Data.UnitOfWork;
using SkiRent.Api.Errors;
using SkiRent.Api.Services.Equipments;
using SkiRent.Shared.Contracts.Equipments;

namespace SkiRent.UnitTests.Systems.Services.Equipments;

public class TestCreateAsync
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
    public async Task WhenEquipmentCategoryNotFound_ReturnsFailedResult()
    {
        // Arrange
        var request = _fixture.Create<CreateEquipmentRequest>();

        _unitOfWork.EquipmentCategories
            .ExistsAsync(Arg.Any<Expression<Func<EquipmentCategory, bool>>>())
            .Returns(false);

        // Act
        var result = await _equipmentService.CreateAsync(request);

        // Assert
        Assert.That(result.IsFailed, Is.True);
        Assert.That(result.Errors[0], Is.InstanceOf<EquipmentCategoryNotFoundError>());
        Assert.That(result.Errors[0].Metadata.GetValueOrDefault("categoryId"), Is.EqualTo(request.CategoryId));
    }

    [Test]
    public async Task WhenEquipmentImageNotFound_ReturnsFailedResult()
    {
        // Arrange
        var request = _fixture.Create<CreateEquipmentRequest>();

        _unitOfWork.EquipmentCategories
            .ExistsAsync(Arg.Any<Expression<Func<EquipmentCategory, bool>>>())
            .Returns(true);
        _unitOfWork.EquipmentImages
            .ExistsAsync(Arg.Any<Expression<Func<EquipmentImage, bool>>>())
            .Returns(false);

        // Act
        var result = await _equipmentService.CreateAsync(request);

        // Assert
        Assert.That(result.IsFailed, Is.True);
        Assert.That(result.Errors[0], Is.InstanceOf<EquipmentImageNotFoundError>());
        Assert.That(result.Errors[0].Metadata.GetValueOrDefault("imageId"), Is.EqualTo(request.MainImageId));
    }
}
