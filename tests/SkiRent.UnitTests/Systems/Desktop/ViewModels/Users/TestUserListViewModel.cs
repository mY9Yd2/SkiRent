using System.Net;
using System.Windows;

using AutoFixture;

using NSubstitute;

using Refit;

using SkiRent.Desktop.Models;
using SkiRent.Desktop.Services;
using SkiRent.Desktop.Utils;
using SkiRent.Desktop.ViewModels.Users;
using SkiRent.Shared.Clients;
using SkiRent.Shared.Contracts.Users;

namespace SkiRent.UnitTests.Systems.Desktop.ViewModels.Users;

public class TestUserListViewModel
{
    private Fixture _fixture;
    private ISkiRentApi _skiRentApi;
    private IUsersApi _usersApi;
    private IUserService _userService;
    private IMessageBoxService _messageBox;
    private INavigationService _navigationService;
    private CurrentUser _currentUser;

    [SetUp]
    public void Setup()
    {
        _fixture = new Fixture();

        _skiRentApi = Substitute.For<ISkiRentApi>();
        _usersApi = Substitute.For<IUsersApi>();
        _userService = Substitute.For<IUserService>();
        _messageBox = Substitute.For<IMessageBoxService>();
        _navigationService = Substitute.For<INavigationService>();

        Navigator.Initialize(_navigationService);

        _currentUser = _fixture.Create<CurrentUser>();

        _skiRentApi.Users.Returns(_usersApi);
        _userService.CurrentUser.Returns(_currentUser);
    }

    [Test]
    public async Task InitializeAsync_PopulatesUsers_WhenApiCallIsSuccessful()
    {
        // Arrange
        var userDtos = _fixture.CreateMany<GetAllUserResponse>(3);
        var response = new ApiResponse<IEnumerable<GetAllUserResponse>>(new(HttpStatusCode.OK), userDtos, null!);

        _skiRentApi.Users.GetAllAsync()
            .Returns(response);

        var viewModel = new UserListViewModel(_skiRentApi, _userService, _messageBox);

        viewModel.Users.Add(_fixture.Create<UserList>());

        var expectedUsers = userDtos.Select(dto => new UserList
        {
            Id = dto.Id,
            Email = dto.Email,
            Role = dto.Role,
        });

        // Act
        await viewModel.InitializeAsync();

        // Assert
        Assert.That(viewModel.Users, Has.Count.EqualTo(userDtos.Count()));
        Assert.That(expectedUsers.All(viewModel.Users.Contains));
    }

    [Test]
    public async Task RefreshCommand_CallsInitializeAsync()
    {
        // Arrange
        var userDtos = _fixture.CreateMany<GetAllUserResponse>(3);
        var response = new ApiResponse<IEnumerable<GetAllUserResponse>>(new(HttpStatusCode.OK), userDtos, null!);

        _skiRentApi.Users.GetAllAsync()
            .Returns(response);

        var viewModel = new UserListViewModel(_skiRentApi, _userService, _messageBox);

        // Act
        await viewModel.RefreshCommand.ExecuteAsync(null);

        // Assert
        Assert.That(viewModel.Users, Has.Count.EqualTo(userDtos.Count()));
    }

    [Test]
    public async Task ShowUserCreateCommand_NavigatesToUserCreateViewModel()
    {
        // Arrange
        var viewModel = new UserListViewModel(_skiRentApi, _userService, _messageBox);

        // Act
        await viewModel.ShowUserCreateCommand.ExecuteAsync(null);

        // Assert
        await Navigator.Instance.Received(1).NavigateToAsync<UserCreateViewModel>();
    }

    [Test]
    public async Task ShowUserEditCommand_NavigatesToUserEditViewModel_WhenUserIsSelected()
    {
        // Arrange
        var user = _fixture.Create<UserList>();
        var viewModel = new UserListViewModel(_skiRentApi, _userService, _messageBox)
        {
            SelectedUser = user
        };

        // Act
        await viewModel.ShowUserEditCommand.ExecuteAsync(null);

        // Assert
        await Navigator.Instance.Received(1).NavigateToAsync(Arg.Any<Func<UserEditViewModel, Task>>());
    }

    [Test]
    public void DeleteUserCommand_DoesNothing_WhenNoUserIsSelected()
    {
        // Arrange
        var viewModel = new UserListViewModel(_skiRentApi, _userService, _messageBox)
        {
            SelectedUser = null!
        };

        // Act & Assert
        Assert.DoesNotThrowAsync(async () => await viewModel.DeleteUserCommand.ExecuteAsync(null));
    }

    [Test]
    public async Task DeleteUserCommand_ShowsError_WhenDeletingSelf()
    {
        // Arrange
        var user = _fixture.Build<UserList>()
            .With(user => user.Id, _currentUser.Id)
            .Create();

        _messageBox.Show(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<MessageBoxButton>(), Arg.Any<MessageBoxImage>())
            .Returns(MessageBoxResult.OK);

        var viewModel = new UserListViewModel(_skiRentApi, _userService, _messageBox)
        {
            SelectedUser = user
        };

        // Act
        await viewModel.DeleteUserCommand.ExecuteAsync(null);

        // Assert
        _messageBox.Received(1).Show("Nem törölheted saját magadat.", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
        _messageBox.Received(1).Show(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<MessageBoxButton>(), Arg.Any<MessageBoxImage>());
        await _skiRentApi.Users.DidNotReceive().DeleteAsync(Arg.Any<int>());
    }

    [Test]
    public async Task DeleteUserCommand_DoesNothing_WhenUserCancelsDeletion()
    {
        // Arrange
        var user = _fixture.Create<UserList>();

        _messageBox.Show(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<MessageBoxButton>(), Arg.Any<MessageBoxImage>())
            .Returns(MessageBoxResult.No);

        var viewModel = new UserListViewModel(_skiRentApi, _userService, _messageBox)
        {
            SelectedUser = user
        };

        // Act
        await viewModel.DeleteUserCommand.ExecuteAsync(null);

        // Assert
        await _skiRentApi.Users.DidNotReceive().DeleteAsync(Arg.Any<int>());
        _messageBox.Received(1).Show("Biztosan törölni szeretné ezt a felhasználót?", "Törlés megerősítése",
            MessageBoxButton.YesNo, MessageBoxImage.Question);
    }

    [Test]
    public async Task DeleteUserCommand_DeletesUser_WhenConfirmed()
    {
        // Arrange
        var user = _fixture.Create<UserList>();
        var response = new ApiResponse<object>(new(HttpStatusCode.OK), null, null!);

        _messageBox.Show(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<MessageBoxButton>(), Arg.Any<MessageBoxImage>())
            .Returns(MessageBoxResult.Yes);

        _usersApi.DeleteAsync(user.Id)
            .Returns(response);

        var viewModel = new UserListViewModel(_skiRentApi, _userService, _messageBox)
        {
            SelectedUser = user
        };

        // Act
        await viewModel.DeleteUserCommand.ExecuteAsync(null);

        // Assert
        _messageBox.Received(1).Show("A felhasználó sikeresen törölve lett.", "Sikeres törlés", MessageBoxButton.OK, MessageBoxImage.Information);
        _messageBox.Received(2).Show(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<MessageBoxButton>(), Arg.Any<MessageBoxImage>());
        await _usersApi.Received(1).DeleteAsync(user.Id);
    }

    [Test]
    public async Task DeleteUserCommand_ShowsError_WhenDeletionFails()
    {
        // Arrange
        var user = _fixture.Create<UserList>();
        var response = new ApiResponse<object>(new(HttpStatusCode.Conflict), null, null!);

        _messageBox.Show(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<MessageBoxButton>(), Arg.Any<MessageBoxImage>())
            .Returns(MessageBoxResult.Yes);

        _usersApi.DeleteAsync(user.Id)
            .Returns(response);

        var viewModel = new UserListViewModel(_skiRentApi, _userService, _messageBox)
        {
            SelectedUser = user
        };

        // Act
        await viewModel.DeleteUserCommand.ExecuteAsync(null);

        // Assert
        await _usersApi.Received(1).DeleteAsync(user.Id);
        _messageBox.Received(1).Show("A felhasználót nem lehet törölni, mert aktív foglalások vannak hozzá rendelve.", "Hiba",
            MessageBoxButton.OK, MessageBoxImage.Error);
    }
}
