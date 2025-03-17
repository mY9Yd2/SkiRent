using Microsoft.EntityFrameworkCore.Storage;

using SkiRent.Api.Data.Repositories.Bookings;
using SkiRent.Api.Data.Repositories.EquipmentCategories;
using SkiRent.Api.Data.Repositories.EquipmentImages;
using SkiRent.Api.Data.Repositories.Equipments;
using SkiRent.Api.Data.Repositories.Invoices;
using SkiRent.Api.Data.Repositories.Users;

namespace SkiRent.Api.Data.UnitOfWork;

public interface IUnitOfWork : IDisposable
{
    public Task SaveChangesAsync();
    public Task<IDbContextTransaction> BeginTransactionAsync();
    public IUserRepository Users { get; }
    public IEquipmentRepository Equipments { get; }
    public IEquipmentCategoryRepository EquipmentCategories { get; }
    public IEquipmentImageRepository EquipmentImages { get; }
    public IBookingRepository Bookings { get; }
    public IInvoiceRepository Invoices { get; }
}
