using Microsoft.EntityFrameworkCore;

namespace Airport.Features.Flights.Infrastructure.Persistence;

public sealed class FlightsDbContext(DbContextOptions<FlightsDbContext> options)
    : DbContext(options)
{
    public DbSet<FlightRow> Flights => Set<FlightRow>();

    public DbSet<AirportRow> Airports => Set<AirportRow>();

    public DbSet<AirlineRow> Airlines => Set<AirlineRow>();

    public DbSet<AirplaneRow> Airplanes => Set<AirplaneRow>();

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
                .HasConversion<int>()
                .HasColumnType("integer")
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

        modelBuilder.Entity<AirlineRow>(entity =>
        {
            entity.ToTable("airline", "airportdb");
            entity.HasKey(airline => airline.AirlineId);
            entity.Property(airline => airline.AirlineId)
                .HasColumnName("airline_id")
                .HasConversion<int>()
                .HasColumnType("integer");
            entity.Property(airline => airline.Iata)
                .HasColumnName("iata")
                .HasColumnType("character(2)");
            entity.Property(airline => airline.Name)
                .HasColumnName("airlinename")
                .HasMaxLength(30);
        });

        modelBuilder.Entity<AirplaneRow>(entity =>
        {
            entity.ToTable("airplane", "airportdb");
            entity.HasKey(airplane => airplane.AirplaneId);
            entity.Property(airplane => airplane.AirplaneId).HasColumnName("airplane_id");
            entity.Property(airplane => airplane.Capacity).HasColumnName("capacity");
            entity.Property(airplane => airplane.TypeId).HasColumnName("type_id");
            entity.Property(airplane => airplane.AirlineId)
                .HasColumnName("airline_id")
                .HasConversion<int>()
                .HasColumnType("integer");
            entity.HasOne(airplane => airplane.Type)
                .WithMany()
                .HasForeignKey(airplane => airplane.TypeId);
        });

        modelBuilder.Entity<AirplaneTypeRow>(entity =>
        {
            entity.ToTable("airplane_type", "airportdb");
            entity.HasKey(type => type.TypeId);
            entity.Property(type => type.TypeId).HasColumnName("type_id");
            entity.Property(type => type.Identifier)
                .HasColumnName("identifier")
                .HasMaxLength(50);
            entity.Property(type => type.Description).HasColumnName("description");
        });

    }
}
