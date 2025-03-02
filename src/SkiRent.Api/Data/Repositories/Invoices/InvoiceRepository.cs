using Microsoft.EntityFrameworkCore;

using SkiRent.Api.Data.Models;

namespace SkiRent.Api.Data.Repositories.Invoices;

public class InvoiceRepository : BaseRepository<Invoice, Guid>, IInvoiceRepository
{
    public InvoiceRepository(DbContext context) : base(context)
    { }
}
