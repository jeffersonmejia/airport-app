using Microsoft.EntityFrameworkCore;

namespace Airport.Features.Bookings.Infrastructure.Persistence;

public sealed class BookingsDbContext(DbContextOptions<BookingsDbContext> options)
    : DbContext(options)
{
    public DbSet<BookingRow> Bookings => Set<BookingRow>();
    public DbSet<BookingFlightRow> Flights => Set<BookingFlightRow>();
    public DbSet<BookingPassengerRow> Passengers => Set<BookingPassengerRow>();
    public DbSet<BookingCancellationRow> Cancellations => Set<BookingCancellationRow>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BookingRow>(entity =>
        {
            entity.ToTable("booking", "airportdb");
            entity.HasKey(row => row.BookingId);
            entity.HasIndex(row => new { row.FlightId, row.Seat }).IsUnique();
            entity.Property(row => row.BookingId)
                .HasColumnName("booking_id")
                .HasDefaultValueSql("nextval('airportdb.booking_booking_id_seq')")
                .ValueGeneratedOnAdd();
            entity.Property(row => row.FlightId).HasColumnName("flight_id");
            entity.Property(row => row.Seat).HasColumnName("seat").HasColumnType("character(4)");
            entity.Property(row => row.PassengerId).HasColumnName("passenger_id");
            entity.Property(row => row.Price).HasColumnName("price").HasPrecision(10, 2);
            entity.Property(row => row.Version).IsRowVersion();
        });

        modelBuilder.Entity<BookingFlightRow>(entity =>
        {
            entity.ToTable("flight", "airportdb");
            entity.HasKey(row => row.FlightId);
            entity.Property(row => row.FlightId).HasColumnName("flight_id");
            entity.Property(row => row.Departure).HasColumnName("departure");
        });

        modelBuilder.Entity<BookingPassengerRow>(entity =>
        {
            entity.ToTable("passenger", "airportdb");
            entity.HasKey(row => row.PassengerId);
            entity.Property(row => row.PassengerId).HasColumnName("passenger_id");
        });

        modelBuilder.Entity<BookingCancellationRow>(entity =>
        {
            entity.ToTable("booking_cancellation", "airportdb");
            entity.HasKey(row => row.BookingId);
            entity.Property(row => row.BookingId).HasColumnName("booking_id");
            entity.Property(row => row.CancelledAt).HasColumnName("cancelled_at");
            entity.Property(row => row.CancelledBy).HasColumnName("cancelled_by");
            entity.Property(row => row.Reason).HasColumnName("reason").HasMaxLength(250);
        });
    }
}
