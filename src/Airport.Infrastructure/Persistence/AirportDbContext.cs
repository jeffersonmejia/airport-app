using Airport.Infrastructure.Features.Flights;
using Microsoft.EntityFrameworkCore;

namespace Airport.Infrastructure.Persistence;

public sealed class AirportDbContext(DbContextOptions<AirportDbContext> options)
    : DbContext(options)
{
    public DbSet<FlightRow> Flights => Set<FlightRow>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<FlightRow>(entity =>
        {
            entity.ToTable("flight", "airportdb");
            entity.HasKey(flight => flight.FlightId);

            entity.Property(flight => flight.FlightId).HasColumnName("flight_id");
            entity.Property(flight => flight.FlightNumber)
                .HasColumnName("flightno")
                .HasColumnType("character(8)")
                .IsRequired();
            entity.Property(flight => flight.Departure)
                .HasColumnName("departure")
                .IsRequired();
            entity.Property(flight => flight.Arrival)
                .HasColumnName("arrival")
                .IsRequired();
            entity.Property(flight => flight.AirlineId)
                .HasColumnName("airline_id")
                .IsRequired();
            entity.Property(flight => flight.AirplaneId)
                .HasColumnName("airplane_id")
                .IsRequired();
        });
    }
}
