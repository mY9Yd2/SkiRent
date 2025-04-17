using System.Linq.Expressions;

using AutoFixture;

using NSubstitute;

using SkiRent.Api.Data.Models;
using SkiRent.Api.Data.UnitOfWork;
using SkiRent.Api.Errors;
using SkiRent.Api.Services.EquipmentCategories;

namespace SkiRent.UnitTests.Systems.Api.Services.EquipmentCategories;

public class TestDeleteAsync
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

        _unitOfWork.EquipmentCategories
            .GetByIdAsync(categoryId)
            .Returns((EquipmentCategory?)null);

        // Act
        var result = await _equipmentCategoryService.DeleteAsync(categoryId);

        // Assert
        Assert.That(result.IsFailed, Is.True);
        Assert.That(result.Errors[0], Is.InstanceOf<EquipmentCategoryNotFoundError>());
        Assert.That(result.Errors[0].Metadata.GetValueOrDefault("categoryId"), Is.EqualTo(categoryId));
    }

    [Test]
    public async Task WhenEquipmentCategoryNotEmpty_ReturnsFailedResult()
    {
        // Arrange
        var category = _fixture.Create<EquipmentCategory>();

        _unitOfWork.EquipmentCategories
            .GetByIdAsync(category.Id)
            .Returns(category);
        _unitOfWork.Equipments
            .ExistsAsync(Arg.Any<Expression<Func<Equipment, bool>>>())
            .Returns(true);

        // Act
        var result = await _equipmentCategoryService.DeleteAsync(category.Id);

        // Assert
        Assert.That(result.IsFailed, Is.True);
        Assert.That(result.Errors[0], Is.InstanceOf<EquipmentCategoryNotEmptyError>());
        Assert.That(result.Errors[0].Metadata.GetValueOrDefault("categoryId"), Is.EqualTo(category.Id));
    }

    [Test]
    public async Task WhenEquipmentCategoryExists_DeletesEquipmentCategoryAndSavesChanges()
    {
        // Arrange
        var category = _fixture.Create<EquipmentCategory>();

        _unitOfWork.EquipmentCategories
            .GetByIdAsync(category.Id)
            .Returns(category);
        _unitOfWork.Equipments
            .ExistsAsync(Arg.Any<Expression<Func<Equipment, bool>>>())
            .Returns(false);

        // Act
        var result = await _equipmentCategoryService.DeleteAsync(category.Id);

        // Assert
        Assert.That(result.IsFailed, Is.False);
        _unitOfWork.EquipmentCategories.Received(1).Delete(category);
        await _unitOfWork.Received(1).SaveChangesAsync();
        await _unitOfWork.Equipments.Received(1).ExistsAsync(
            Arg.Is<Expression<Func<Equipment, bool>>>(expression =>
                expression.Compile().Invoke(new Equipment { CategoryId = category.Id }))
            );
    }
}
