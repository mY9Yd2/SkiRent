using System.Linq.Expressions;

using AutoFixture;

using NSubstitute;

using SkiRent.Api.Data.Models;
using SkiRent.Api.Data.UnitOfWork;
using SkiRent.Api.Errors;
using SkiRent.Api.Services.EquipmentCategories;
using SkiRent.Shared.Contracts.EquipmentCategories;

namespace SkiRent.UnitTests.Systems.Api.Services.EquipmentCategories;

public class TestCreateAsync
{
    private IUnitOfWork _unitOfWork;
    private Fixture _fixture;
    private IEquipmentCategoryService _equipmentCategoryService;

    [SetUp]
    public void Setup()
    {
        _fixture = new Fixture();
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _equipmentCategoryService = new EquipmentCategoryService(_unitOfWork);
    }

    [TearDown]
    public void TearDown()
    {
        _unitOfWork.Dispose();
    }

    [Test]
    public async Task WhenEquipmentCategoryAlreadyExists_ReturnsFailedResult()
    {
        // Arrange
        var request = _fixture.Create<CreateEquipmentCategoryRequest>();

        _unitOfWork.EquipmentCategories
            .ExistsAsync(Arg.Any<Expression<Func<EquipmentCategory, bool>>>())
            .Returns(true);

        // Act
        var result = await _equipmentCategoryService.CreateAsync(request);

        // Assert
        Assert.That(result.IsFailed, Is.True);
        Assert.That(result.Errors[0], Is.InstanceOf<EquipmentCategoryAlreadyExistsError>());
        Assert.That(result.Errors[0].Metadata.GetValueOrDefault("categoryName"), Is.EqualTo(request.Name));
    }
}
