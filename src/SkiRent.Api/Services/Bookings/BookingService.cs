using System.Globalization;

using FluentResults;

using Microsoft.Extensions.Options;

using SkiRent.Api.Configurations;
using SkiRent.Api.Data.Auth;
using SkiRent.Api.Data.Models;
using SkiRent.Api.Data.UnitOfWork;
using SkiRent.Api.Errors;
using SkiRent.Api.Extensions;
using SkiRent.Shared.Contracts.Bookings;
using SkiRent.Shared.Contracts.Common;
using SkiRent.Shared.Contracts.Invoices;
using SkiRent.Shared.Contracts.Payments;

using ZiggyCreatures.Caching.Fusion;

namespace SkiRent.Api.Services.Bookings;

public class BookingService : IBookingService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IHttpClientFactory _clientFactory;
    private readonly IFusionCache _cache;
    private readonly AppSettings _appSettings;
    private readonly PaymentGatewayOptions _paymentGatewayOptions;
    private readonly TimeProvider _timeProvider;

    public BookingService(
        IUnitOfWork unitOfWork,
        IOptions<AppSettings> appSettings,
        IOptions<PaymentGatewayOptions> paymentGatewayOptions,
        IHttpClientFactory clientFactory,
        [FromKeyedServices("SkiRent.Api.Cache")] IFusionCache cache,
        TimeProvider timeProvider)
    {
        _unitOfWork = unitOfWork;
        _clientFactory = clientFactory;
        _cache = cache;
        _appSettings = appSettings.Value;
        _paymentGatewayOptions = paymentGatewayOptions.Value;
        _timeProvider = timeProvider;
    }

    public async Task<Result<CreatedBookingResponse>> CreateAsync(int userId, CreateBookingRequest request)
    {
        if (!await _unitOfWork.Users.ExistsAsync(user => user.Id == userId))
        {
            return Result.Fail(new UserNotFoundError(userId));
        }

        var paymentItems = new List<PaymentItem>();
        var bookingItems = new List<BookingItem>();

        foreach (var equipmentBooking in request.Equipments)
        {
            var equipment = await _unitOfWork.Equipments.GetByIdAsync(equipmentBooking.EquipmentId);

            if (equipment is null)
            {
                return Result.Fail(new EquipmentNotFoundError(equipmentBooking.EquipmentId));
            }

            if (equipment.AvailableQuantity < equipmentBooking.Quantity)
            {
                return Result.Fail(new InsufficientQuantityError(equipmentBooking.EquipmentId));
            }

            equipment.AvailableQuantity -= equipmentBooking.Quantity;

            var days = (request.EndDate.DayNumber - request.StartDate.DayNumber) + 1;

            paymentItems.Add(new PaymentItem
            {
                Name = equipment.Name,
                SubText = $"{equipmentBooking.Quantity} db x {equipment.PricePerDay.ToString("C0", CultureInfo.CreateSpecificCulture("hu-HU"))} x {days} napra",
                TotalPrice = equipmentBooking.Quantity * equipment.PricePerDay * days,
            });

            bookingItems.Add(new BookingItem
            {
                EquipmentId = equipmentBooking.EquipmentId,
                NameAtBooking = equipment.Name,
                PriceAtBooking = equipment.PricePerDay,
                Quantity = equipmentBooking.Quantity
            });
        }

        var totalPrice = paymentItems.Sum(item => item.TotalPrice);

        var paymentId = await CreatePaymentAsync(paymentItems, totalPrice, request.SuccessUrl, request.CancelUrl);

        var booking = new Booking
        {
            UserId = userId,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            TotalPrice = totalPrice,
            PaymentId = paymentId,
            Status = BookingStatus.Pending,
            BookingItems = bookingItems,

            // Új mezők
            FullName = request.PersonalDetails.FullName,
            PhoneNumber = request.PersonalDetails.PhoneNumber,
            MobilePhoneNumber = request.PersonalDetails.MobilePhoneNumber,
            AddressCountry = request.PersonalDetails.Address.Country,
            AddressPostalCode = request.PersonalDetails.Address.PostalCode,
            AddressCity = request.PersonalDetails.Address.City,
            AddressStreet = request.PersonalDetails.Address.StreetAddress,
        };

        await _unitOfWork.Bookings.AddAsync(booking);
        await _unitOfWork.SaveChangesAsync();

        var invoiceRequest = new CreateInvoiceRequest
        {
            PaymentId = paymentId,
            PersonalDetails = request.PersonalDetails,
            Items = paymentItems,
            StartDate = booking.StartDate,
            EndDate = booking.EndDate,
            MerchantName = _appSettings.MerchantName,
            Culture = CultureInfo.CreateSpecificCulture("hu-HU")
        };

        await _cache.SetAsync(paymentId.ToString(), invoiceRequest);

        var result = new CreatedBookingResponse
        {
            Id = booking.Id,
            PaymentId = booking.PaymentId,
            PaymentUrl = new Uri($"{_paymentGatewayOptions.BaseUrl}payments/{paymentId}")
        };

        return Result.Ok(result);
    }

    public async Task<Result<GetBookingResponse>> GetAsync(int bookingId, int userId, Func<string, bool> isInRole)
    {
        var booking = await _unitOfWork.Bookings.GetBookingWithItemsAsync(bookingId);

        if (booking is null)
        {
            return Result.Fail(new BookingNotFoundError(bookingId));
        }

        if (!isInRole(Roles.Admin) && booking.UserId != userId)
        {
            return Result.Fail(new BookingAccessDeniedError(bookingId));
        }

        var days = (booking.EndDate.DayNumber - booking.StartDate.DayNumber) + 1;

        var result = new GetBookingResponse
        {
            Id = booking.Id,
            UserId = booking.UserId,
            StartDate = booking.StartDate,
            EndDate = booking.EndDate,
            TotalPrice = booking.TotalPrice,
            PaymentId = booking.PaymentId,
            Status = Enum.TryParse<BookingStatusTypes>(booking.Status, out var parsedStatus) ? parsedStatus : BookingStatusTypes.Pending,
            CreatedAt = booking.CreatedAt,
            UpdatedAt = booking.UpdatedAt,
            Items = booking.BookingItems.Select(item => new BookingItemSummary
            {
                Name = item.NameAtBooking,
                Quantity = item.Quantity,
                PricePerDay = item.PriceAtBooking,
                TotalPrice = item.Quantity * item.PriceAtBooking * days
            }),
            RentalDays = days,
            IsOverdue = IsOverdue(booking.EndDate, booking.Status),
            PersonalDetails = MapPersonalDetails(booking)   // Új metódus hívás
        };

        return Result.Ok(result);
    }

    public async Task<Result<IEnumerable<GetAllBookingResponse>>> GetAllAsync(int userId, Func<string, bool> isInRole)
    {
        var bookings = isInRole(Roles.Admin)
            ? await _unitOfWork.Bookings.GetAllAsync()
            : await _unitOfWork.Bookings.FindAllAsync(booking => booking.UserId == userId);

        var result = bookings.Select(booking =>
            new GetAllBookingResponse
            {
                Id = booking.Id,
                StartDate = booking.StartDate,
                EndDate = booking.EndDate,
                TotalPrice = booking.TotalPrice,
                PaymentId = booking.PaymentId,
                Status = Enum.TryParse<BookingStatusTypes>(booking.Status, out var parsedStatus) ? parsedStatus : BookingStatusTypes.Pending,
                CreatedAt = booking.CreatedAt,
                UpdatedAt = booking.UpdatedAt,
                IsOverdue = IsOverdue(booking.EndDate, booking.Status)
            });

        return Result.Ok(result);
    }

    public async Task<Result<GetBookingResponse>> UpdateAsync(int bookingId, UpdateBookingRequest request)
    {
        var booking = await _unitOfWork.Bookings.GetBookingWithItemsAsync(bookingId);

        if (booking is null)
        {
            return Result.Fail(new BookingNotFoundError(bookingId));
        }

        if (request.Status is not null)
        {
            if (!IsValidStatusTransition(booking.Status, request.Status))
            {
                return Result.Fail(new InvalidBookingStatusTransitionError(booking.Status, ((BookingStatusTypes)request.Status).ToBookingStatusString()));
            }
            booking.Status = request.Status?.ToBookingStatusString() ?? booking.Status;
        }

        if (request.Status == BookingStatusTypes.Returned)
        {
            foreach (var item in booking.BookingItems)
            {
                var equipment = await _unitOfWork.Equipments.GetByIdAsync(item.EquipmentId);

                if (equipment is not null)
                {
                    equipment.AvailableQuantity += item.Quantity;
                }
            }
        }

        await _unitOfWork.SaveChangesAsync();

        var days = (booking.EndDate.DayNumber - booking.StartDate.DayNumber) + 1;

        var result = new GetBookingResponse
        {
            Id = booking.Id,
            UserId = booking.UserId,
            StartDate = booking.StartDate,
            EndDate = booking.EndDate,
            TotalPrice = booking.TotalPrice,
            PaymentId = booking.PaymentId,
            Status = Enum.TryParse<BookingStatusTypes>(booking.Status, out var parsedStatus) ? parsedStatus : BookingStatusTypes.Pending,
            CreatedAt = booking.CreatedAt,
            UpdatedAt = booking.UpdatedAt,
            Items = booking.BookingItems.Select(item => new BookingItemSummary
            {
                Name = item.NameAtBooking,
                Quantity = item.Quantity,
                PricePerDay = item.PriceAtBooking,
                TotalPrice = item.Quantity * item.PriceAtBooking * days
            }),
            RentalDays = days,
            IsOverdue = IsOverdue(booking.EndDate, booking.Status),

            PersonalDetails = MapPersonalDetails(booking)   //új metódus hívás.


        };

        return Result.Ok(result);
    }

    public async Task<Result> DeleteAsync(int bookingId)
    {
        var booking = await _unitOfWork.Bookings.GetByIdAsync(bookingId);

        if (booking is null)
        {
            return Result.Fail(new BookingNotFoundError(bookingId));
        }

        if (booking.Status != BookingStatus.Cancelled && booking.Status != BookingStatus.Returned)
        {
            return Result.Fail(new BookingDeletionNotAllowedError(bookingId));
        }

        _unitOfWork.Bookings.Delete(booking);
        await _unitOfWork.SaveChangesAsync();

        return Result.Ok();
    }

    protected virtual async Task<Guid> CreatePaymentAsync(IEnumerable<PaymentItem> paymentItems, decimal totalPrice, Uri successUrl, Uri cancelUrl)
    {
        var client = _clientFactory.CreateClient();
        client.BaseAddress = _paymentGatewayOptions.BaseUrl;

        var response = await client.PostAsJsonAsync("/api/payments", new CreatePaymentRequest
        {
            MerchantName = _appSettings.MerchantName,
            Items = paymentItems,
            TotalPrice = totalPrice,
            TwoLetterISORegionName = "HU",
            CallbackUrl = new Uri($"{_appSettings.BaseUrl}api/payments/callback"),
            SuccessUrl = successUrl,
            CancelUrl = cancelUrl
        });

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<Guid>();
    }

    private bool IsOverdue(DateOnly endDate, string status)
    {
        var bookingStatus = Enum.TryParse<BookingStatusTypes>(status, out var parsedStatus) ? parsedStatus : BookingStatusTypes.Pending;

        return endDate < DateOnly.FromDateTime(_timeProvider.GetUtcNow().UtcDateTime)
            && bookingStatus == BookingStatusTypes.Paid;
    }

    private static bool IsValidStatusTransition(string currentStatus, BookingStatusTypes? newStatus)
    {
        return newStatus switch
        {
            BookingStatusTypes.InDelivery or BookingStatusTypes.Received
                when currentStatus == BookingStatus.Paid => true,

            BookingStatusTypes.Received
                when currentStatus == BookingStatus.InDelivery => true,

            BookingStatusTypes.Returned
                when currentStatus == BookingStatus.Received => true,

            _ => false
        };
    }


    //Új metódus:
    private static PersonalDetails MapPersonalDetails(Booking booking) => new()
    {
        FullName = booking.FullName ?? "",
        PhoneNumber = booking.PhoneNumber ?? "",
        MobilePhoneNumber = booking.MobilePhoneNumber ?? "",
        Address = new Address
        {
            Country = booking.AddressCountry ?? "",
            PostalCode = booking.AddressPostalCode ?? "",
            City = booking.AddressCity ?? "",
            StreetAddress = booking.AddressStreet ?? ""
        }
    };

}
