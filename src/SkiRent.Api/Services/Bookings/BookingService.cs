using System.Globalization;

using FluentResults;

using Microsoft.Extensions.Options;

using SkiRent.Api.Configurations;
using SkiRent.Api.Data.Models;
using SkiRent.Api.Data.UnitOfWork;
using SkiRent.Api.Errors;
using SkiRent.Shared.Contracts.Bookings;
using SkiRent.Shared.Contracts.Common;
using SkiRent.Shared.Contracts.Payments;

namespace SkiRent.Api.Services.Bookings;

public class BookingService : IBookingService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IHttpClientFactory _clientFactory;
    private readonly AppSettings _appSettings;
    private readonly PaymentGatewayOptions _paymentGatewayOptions;

    public BookingService(
        IUnitOfWork unitOfWork,
        IOptions<AppSettings> appSettings,
        IOptions<PaymentGatewayOptions> paymentGatewayOptions,
        IHttpClientFactory clientFactory)
    {
        _unitOfWork = unitOfWork;
        _clientFactory = clientFactory;
        _appSettings = appSettings.Value;
        _paymentGatewayOptions = paymentGatewayOptions.Value;
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
            BookingItems = bookingItems
        };

        await _unitOfWork.Bookings.AddAsync(booking);
        await _unitOfWork.SaveChangesAsync();

        var result = new CreatedBookingResponse
        {
            Id = booking.Id,
            PaymentId = booking.PaymentId,
            PaymentUrl = new Uri($"{_paymentGatewayOptions.BaseUrl}payments/{paymentId}")
        };

        return Result.Ok(result);
    }

    private async Task<Guid> CreatePaymentAsync(IEnumerable<PaymentItem> paymentItems, decimal totalPrice, Uri successUrl, Uri cancelUrl)
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
}
