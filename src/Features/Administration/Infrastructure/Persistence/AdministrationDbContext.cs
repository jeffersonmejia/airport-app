using Microsoft.EntityFrameworkCore;

namespace Airport.Features.Administration.Infrastructure.Persistence;

public sealed class AdministrationDbContext(DbContextOptions<AdministrationDbContext> options)
    : DbContext(options)
{
    public DbSet<CommerceOrderRow> Orders => Set<CommerceOrderRow>();
    public DbSet<CommercePaymentRow> Payments => Set<CommercePaymentRow>();
    public DbSet<CommerceTicketRow> Tickets => Set<CommerceTicketRow>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CommerceOrderRow>(entity =>
        {
            entity.ToTable("orders", "airport_app", table => table.ExcludeFromMigrations());
            entity.HasKey(row => row.Id);
            entity.Property(row => row.Id).HasColumnName("order_id");
            entity.Property(row => row.UserId).HasColumnName("user_id");
            entity.Property(row => row.FlightNumber).HasColumnName("flight_number");
            entity.Property(row => row.OriginCode).HasColumnName("origin_code");
            entity.Property(row => row.DestinationCode).HasColumnName("destination_code");
            entity.Property(row => row.Total).HasColumnName("total");
            entity.Property(row => row.CurrencyCode).HasColumnName("currency_code");
            entity.Property(row => row.Status).HasColumnName("status");
            entity.Property(row => row.CreatedAt).HasColumnName("created_at");
        });
        modelBuilder.Entity<CommercePaymentRow>(entity =>
        {
            entity.ToTable("payments", "airport_app", table => table.ExcludeFromMigrations());
            entity.HasKey(row => row.Id);
            entity.Property(row => row.Id).HasColumnName("payment_id");
            entity.Property(row => row.OrderId).HasColumnName("order_id");
            entity.Property(row => row.ProviderOrderId).HasColumnName("provider_order_id");
            entity.Property(row => row.ProviderCaptureId).HasColumnName("provider_capture_id");
            entity.Property(row => row.Status).HasColumnName("status");
            entity.Property(row => row.Amount).HasColumnName("amount");
        });
        modelBuilder.Entity<CommerceTicketRow>(entity =>
        {
            entity.ToTable("purchased_tickets", "airport_app", table => table.ExcludeFromMigrations());
            entity.HasKey(row => row.Id);
            entity.Property(row => row.Id).HasColumnName("purchased_ticket_id");
            entity.Property(row => row.OrderId).HasColumnName("order_id");
            entity.Property(row => row.TicketNumber).HasColumnName("ticket_number");
        });
    }
}
