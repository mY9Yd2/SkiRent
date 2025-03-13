using Refit;

using SkiRent.Shared.Contracts.Auth;
using SkiRent.Shared.Contracts.Bookings;
using SkiRent.Shared.Contracts.EquipmentCategories;
using SkiRent.Shared.Contracts.Equipments;
using SkiRent.Shared.Contracts.Invoices;
using SkiRent.Shared.Contracts.Payments;
using SkiRent.Shared.Contracts.Users;

namespace SkiRent.Shared.Clients
{
    public interface ISkiRentApi
    {
        public HttpClient Client { get; }

        // Workaround:
        // Refit currently does not support to compose multiple interfaces in one API interface
        [Get("/api")]
        public Task<IApiResponse> PingAsync();

        public IAuthApi Auth => RestService.For<IAuthApi>(Client);
        public IUsersApi Users => RestService.For<IUsersApi>(Client);
        public IEquipmentCategoriesApi EquipmentCategories => RestService.For<IEquipmentCategoriesApi>(Client);
        public IEquipmentsApi Equipments => RestService.For<IEquipmentsApi>(Client);
        public IBookingsApi Bookings => RestService.For<IBookingsApi>(Client);
        public IPaymentsApi Payments => RestService.For<IPaymentsApi>(Client);
        public IInvoicesApi Invoices => RestService.For<IInvoicesApi>(Client);
    }

    public interface IAuthApi
    {
        [Post("/api/auth/sign-in")]
        public Task<IApiResponse> SignInAsync(SignInRequest request);

        [Post("/api/auth/sign-out")]
        public Task<IApiResponse> SignOutAsync(object empty);

        [Get("/api/auth/me")]
        public Task<IApiResponse<GetUserResponse>> MeAsync();
    }

    public interface IUsersApi
    {
        [Post("/api/users")]
        public Task<IApiResponse<CreatedUserResponse>> CreateAsync(CreateUserRequest request);

        [Get("/api/users/{userId}")]
        public Task<IApiResponse<GetUserResponse>> GetAsync(int userId);

        [Get("/api/users")]
        public Task<IApiResponse<IEnumerable<GetAllUserResponse>>> GetAllAsync();

        [Put("/api/users/{userId}")]
        public Task<IApiResponse<GetUserResponse>> UpdateAsync(int userId, UpdateUserRequest request);
    }

    public interface IEquipmentCategoriesApi
    {
        [Post("/api/equipment-categories")]
        public Task<IApiResponse<CreatedEquipmentCategoryResponse>> CreateAsync(CreateEquipmentCategoryRequest request);

        [Get("/api/equipment-categories")]
        public Task<IApiResponse<IEnumerable<GetAllEquipmentCategoryResponse>>> GetAllAsync();

        [Put("/api/equipment-categories/{categoryId}")]
        public Task<IApiResponse<GetEquipmentCategoryResponse>> UpdateAsync(int categoryId, UpdateEquipmentCategoryRequest request);
    }

    public interface IEquipmentsApi
    {
        [Post("/api/equipments")]
        public Task<IApiResponse<CreatedEquipmentResponse>> CreateAsync(CreateEquipmentRequest request);

        [Get("/api/equipments/{equipmentId}")]
        public Task<IApiResponse<GetEquipmentResponse>> GetAsync(int equipmentId);

        [Get("/api/equipments")]
        public Task<IApiResponse<IEnumerable<GetAllEquipmentResponse>>> GetAllAsync();

        [Put("/api/equipments/{equipmentId}")]
        public Task<IApiResponse<GetEquipmentResponse>> UpdateAsync(int equipmentId, UpdateEquipmentRequest request);
    }

    public interface IBookingsApi
    {
        [Post("/api/bookings")]
        public Task<IApiResponse<CreatedBookingResponse>> CreateAsync(CreateBookingRequest request);

        [Get("/api/bookings/{bookingId}")]
        public Task<IApiResponse<GetBookingResponse>> GetAsync(int bookingId);

        [Get("/api/bookings")]
        public Task<IApiResponse<IEnumerable<GetAllBookingResponse>>> GetAllAsync();

        [Put("/api/bookings/{bookingId}")]
        public Task<IApiResponse<GetBookingResponse>> UpdateAsync(int bookingId, UpdateBookingRequest request);
    }

    public interface IPaymentsApi
    {
        [Post("/api/payments/callback")]
        public Task<IApiResponse> CallbackAsync([Header("X-Signature")] string signature, PaymentResult paymentResult);
    }

    public interface IInvoicesApi
    {
        [Get("/api/invoices/{invoiceId}")]
        public Task<IApiResponse<Stream>> GetAsync(Guid invoiceId);

        [Get("/api/invoices")]
        public Task<IApiResponse<IEnumerable<GetAllInvoicesResponse>>> GetAllAsync();
    }
}
