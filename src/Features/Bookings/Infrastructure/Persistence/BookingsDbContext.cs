using Microsoft.EntityFrameworkCore;

namespace Airport.Features.Bookings.Infrastructure.Persistence;

public sealed class BookingsDbContext(DbContextOptions<BookingsDbContext> options) : DbContext(options)
{
    public DbSet<OrderRow> Orders => Set<OrderRow>();
    public DbSet<OrderDetailRow> OrderDetails => Set<OrderDetailRow>();
    public DbSet<PurchasedTicketRow> PurchasedTickets => Set<PurchasedTicketRow>();
    public DbSet<PaymentRow> Payments => Set<PaymentRow>();
    internal DbSet<FlightOfferRow> Flights => Set<FlightOfferRow>();
    internal DbSet<BookingAirportRow> Airports => Set<BookingAirportRow>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<OrderRow>(entity =>
        {
            entity.ToTable("orders", "airport_app");
            entity.HasKey(row => row.Id);
            entity.Property(row => row.Id).HasColumnName("order_id");
            entity.Property(row => row.UserId).HasColumnName("user_id").HasMaxLength(450).IsRequired();
            entity.Property(row => row.FlightId).HasColumnName("flight_id");
            entity.Property(row => row.FlightNumber).HasColumnName("flight_number").HasMaxLength(8);
            entity.Property(row => row.OriginCode).HasColumnName("origin_code").HasMaxLength(4);
            entity.Property(row => row.DestinationCode).HasColumnName("destination_code").HasMaxLength(4);
            entity.Property(row => row.Departure).HasColumnName("departure");
            entity.Property(row => row.FareCode).HasColumnName("fare_code").HasMaxLength(20);
            entity.Property(row => row.FareName).HasColumnName("fare_name").HasMaxLength(50);
            entity.Property(row => row.Total).HasColumnName("total").HasPrecision(12, 2);
            entity.Property(row => row.CurrencyCode).HasColumnName("currency_code").HasMaxLength(3);
            entity.Property(row => row.Status).HasColumnName("status").HasMaxLength(30);
            entity.Property(row => row.CreatedAt).HasColumnName("created_at");
            entity.Property(row => row.UpdatedAt).HasColumnName("updated_at");
            entity.HasIndex(row => new { row.UserId, row.CreatedAt });
        });

        modelBuilder.Entity<OrderDetailRow>(entity =>
        {
            entity.ToTable("order_details", "airport_app");
            entity.HasKey(row => row.Id);
            entity.Property(row => row.Id).HasColumnName("order_detail_id");
            entity.Property(row => row.OrderId).HasColumnName("order_id");
            entity.Property(row => row.PassengerFirstName).HasColumnName("passenger_first_name").HasMaxLength(100);
            entity.Property(row => row.PassengerLastName).HasColumnName("passenger_last_name").HasMaxLength(100);
            entity.Property(row => row.PassportNumber).HasColumnName("passport_number").HasMaxLength(20);
            entity.Property(row => row.Quantity).HasColumnName("quantity");
            entity.Property(row => row.UnitPrice).HasColumnName("unit_price").HasPrecision(12, 2);
            entity.HasIndex(row => row.OrderId).IsUnique();
            entity.HasOne(row => row.Order).WithOne(row => row.Detail)
                .HasForeignKey<OrderDetailRow>(row => row.OrderId);
        });

        modelBuilder.Entity<PurchasedTicketRow>(entity =>
        {
            entity.ToTable("purchased_tickets", "airport_app");
            entity.HasKey(row => row.Id);
            entity.Property(row => row.Id).HasColumnName("purchased_ticket_id");
            entity.Property(row => row.OrderId).HasColumnName("order_id");
            entity.Property(row => row.FlightId).HasColumnName("flight_id");
            entity.Property(row => row.TicketNumber).HasColumnName("ticket_number").HasMaxLength(30);
            entity.Property(row => row.FareCode).HasColumnName("fare_code").HasMaxLength(20);
            entity.Property(row => row.IssuedAt).HasColumnName("issued_at");
            entity.HasIndex(row => row.OrderId).IsUnique();
            entity.HasIndex(row => row.TicketNumber).IsUnique();
            entity.HasOne(row => row.Order).WithOne(row => row.Ticket)
                .HasForeignKey<PurchasedTicketRow>(row => row.OrderId);
        });

        modelBuilder.Entity<PaymentRow>(entity =>
        {
            entity.ToTable("payments", "airport_app");
            entity.HasKey(row => row.Id);
            entity.Property(row => row.Id).HasColumnName("payment_id");
            entity.Property(row => row.OrderId).HasColumnName("order_id");
            entity.Property(row => row.Provider).HasColumnName("provider").HasMaxLength(20);
            entity.Property(row => row.ProviderOrderId).HasColumnName("provider_order_id").HasMaxLength(100);
            entity.Property(row => row.ApprovalUrl).HasColumnName("approval_url").HasMaxLength(500);
            entity.Property(row => row.ProviderCaptureId).HasColumnName("provider_capture_id").HasMaxLength(100);
            entity.Property(row => row.IdempotencyKey).HasColumnName("idempotency_key").HasMaxLength(108);
            entity.Property(row => row.Status).HasColumnName("status").HasMaxLength(30);
            entity.Property(row => row.Amount).HasColumnName("amount").HasPrecision(12, 2);
            entity.Property(row => row.CurrencyCode).HasColumnName("currency_code").HasMaxLength(3);
            entity.Property(row => row.CreatedAt).HasColumnName("created_at");
            entity.Property(row => row.CompletedAt).HasColumnName("completed_at");
            entity.HasIndex(row => row.ProviderOrderId).IsUnique();
            entity.HasIndex(row => row.ProviderCaptureId).IsUnique();
            entity.HasIndex(row => row.IdempotencyKey).IsUnique();
            entity.HasOne(row => row.Order).WithMany(row => row.Payments)
                .HasForeignKey(row => row.OrderId);
        });

        modelBuilder.Entity<FlightOfferRow>(entity =>
        {
            entity.ToTable("flight", "airportdb", table => table.ExcludeFromMigrations());
            entity.HasKey(row => row.FlightId);
            entity.Property(row => row.FlightId).HasColumnName("flight_id");
            entity.Property(row => row.FlightNumber).HasColumnName("flightno").HasColumnType("character(8)");
            entity.Property(row => row.OriginAirportId).HasColumnName("from").HasConversion<short>().HasColumnType("smallint");
            entity.Property(row => row.DestinationAirportId).HasColumnName("to").HasConversion<short>().HasColumnType("smallint");
            entity.Property(row => row.Departure).HasColumnName("departure");
            entity.Property(row => row.Arrival).HasColumnName("arrival");
        });

        modelBuilder.Entity<BookingAirportRow>(entity =>
        {
            entity.ToTable("airport", "airportdb", table => table.ExcludeFromMigrations());
            entity.HasKey(row => row.AirportId);
            entity.Property(row => row.AirportId).HasColumnName("airport_id");
            entity.Property(row => row.Iata).HasColumnName("iata").HasColumnType("character(3)");
            entity.Property(row => row.Icao).HasColumnName("icao").HasColumnType("character(4)");
        });
    }
}
