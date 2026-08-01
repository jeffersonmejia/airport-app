using Microsoft.EntityFrameworkCore;

namespace Airport.Features.Flights.Infrastructure.Persistence;

public sealed class FlightsDbContext(DbContextOptions<FlightsDbContext> options)
    : DbContext(options)
{
    public DbSet<FlightRow> Flights => Set<FlightRow>();

    public DbSet<AirportRow> Airports => Set<AirportRow>();

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
            entity.Property(flight => flight.OriginAirportId)
                .HasColumnName("from")
                .HasConversion<short>()
                .HasColumnType("smallint");
            entity.Property(flight => flight.DestinationAirportId)
                .HasColumnName("to")
                .HasConversion<short>()
                .HasColumnType("smallint");
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

            entity.HasOne(flight => flight.OriginAirport)
                .WithMany()
                .HasForeignKey(flight => flight.OriginAirportId);
            entity.HasOne(flight => flight.DestinationAirport)
                .WithMany()
                .HasForeignKey(flight => flight.DestinationAirportId);
        });

        modelBuilder.Entity<AirportRow>(entity =>
        {
            entity.ToTable("airport", "airportdb");
            entity.HasKey(airport => airport.AirportId);
            entity.Property(airport => airport.AirportId).HasColumnName("airport_id");
            entity.Property(airport => airport.Iata).HasColumnName("iata").HasColumnType("character(3)");
            entity.Property(airport => airport.Icao).HasColumnName("icao").HasColumnType("character(4)");
            entity.Property(airport => airport.Name).HasColumnName("name").HasMaxLength(50);
        });

    }
}
