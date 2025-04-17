using System.Linq.Expressions;

using AutoFixture;

using NSubstitute;

using SkiRent.Api.Data.Models;
using SkiRent.Api.Data.UnitOfWork;
using SkiRent.Api.Errors;
using SkiRent.Api.Services.EquipmentCategories;

using SkiRent.Shared.Contracts.EquipmentCategories;

namespace SkiRent.UnitTests.Systems.Api.Services.EquipmentCategories;

public class TestUpdateAsync
{
    private IUnitOfWork _unitOfWork;
    private Fixture _fixture;
    private IEquipmentCategoryService _equipmentCategoryService;

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
        _equipmentCategoryService = new EquipmentCategoryService(_unitOfWork);
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
        var categoryId = _fixture.Create<int>();
        var request = _fixture.Create<UpdateEquipmentCategoryRequest>();

        _unitOfWork.EquipmentCategories
            .GetByIdAsync(categoryId)
            .Returns((EquipmentCategory?)null);

        // Act
        var result = await _equipmentCategoryService.UpdateAsync(categoryId, request);

        // Assert
        Assert.That(result.IsFailed, Is.True);
        Assert.That(result.Errors[0], Is.InstanceOf<EquipmentCategoryNotFoundError>());
        Assert.That(result.Errors[0].Metadata.GetValueOrDefault("categoryId"), Is.EqualTo(categoryId));
    }

    [Test]
    public async Task WhenEquipmentCategoryAlreadyExists_ReturnsFailedResult()
    {
        // Arrange
        var category = _fixture.Create<EquipmentCategory>();
        var request = _fixture.Create<UpdateEquipmentCategoryRequest>();

        _unitOfWork.EquipmentCategories
            .GetByIdAsync(category.Id)
            .Returns(category);
        _unitOfWork.EquipmentCategories
            .ExistsAsync(Arg.Any<Expression<Func<EquipmentCategory, bool>>>())
            .Returns(true);

        // Act
        var result = await _equipmentCategoryService.UpdateAsync(category.Id, request);

        // Assert
        Assert.That(result.IsFailed, Is.True);
        Assert.That(result.Errors[0], Is.InstanceOf<EquipmentCategoryAlreadyExistsError>());
        Assert.That(result.Errors[0].Metadata.GetValueOrDefault("categoryName"), Is.EqualTo(request.Name));
    }

    [Test]
    public async Task WhenUpdateIsValid_CallsSaveChangesAsync()
    {
        // Arrange
        var category = _fixture.Create<EquipmentCategory>();
        var request = _fixture.Create<UpdateEquipmentCategoryRequest>();

        _unitOfWork.EquipmentCategories
            .GetByIdAsync(category.Id)
            .Returns(category);

        _unitOfWork.EquipmentCategories
            .ExistsAsync(Arg.Any<Expression<Func<EquipmentCategory, bool>>>())
            .Returns(false);

        // Act
        var result = await _equipmentCategoryService.UpdateAsync(category.Id, request);

        // Assert
        Assert.That(result.IsFailed, Is.False);
        Assert.That(result.Value.Name, Is.EqualTo(request.Name));
        await _unitOfWork.Received(1).SaveChangesAsync();
    }
}
