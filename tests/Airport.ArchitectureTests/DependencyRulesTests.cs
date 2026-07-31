using System.Reflection;
using Airport.Features.Auth.Application.Ports;
using Airport.Features.Auth.Domain;
using Airport.Features.Auth.Infrastructure.Security;
using Airport.Features.Auth.Presentation.Web;
using Airport.Features.Bookings.Application.SearchBookings;
using Airport.Features.Bookings.Domain;
using Airport.Features.Bookings.Infrastructure.Persistence;
using Airport.Features.Bookings.Presentation.Web;
using Airport.Features.Flights.Application.GetFlight;
using Airport.Features.Flights.Domain;
using Airport.Features.Flights.Infrastructure.Persistence;
using Airport.Features.Flights.Presentation.Api;
using Airport.Features.Flights.Presentation.Web;

namespace Airport.ArchitectureTests;

public sealed class DependencyRulesTests
{
    [Fact]
    public void Domain_DoesNotReferenceOuterLayers()
    {
        var references = ReferencesOf(typeof(Flight).Assembly);

        Assert.DoesNotContain(references, IsApplication);
        Assert.DoesNotContain(references, IsInfrastructure);
        Assert.DoesNotContain(references, IsPresentation);
        Assert.DoesNotContain(references, name => name.StartsWith("Microsoft.EntityFrameworkCore"));
        Assert.DoesNotContain(references, name => name.StartsWith("Npgsql"));
    }

    [Fact]
    public void Application_DoesNotReferenceAdapters()
    {
        var references = ReferencesOf(typeof(GetFlightHandler).Assembly);

        Assert.DoesNotContain(references, IsInfrastructure);
        Assert.DoesNotContain(references, IsPresentation);
        Assert.DoesNotContain(references, name => name.StartsWith("Microsoft.EntityFrameworkCore"));
        Assert.DoesNotContain(references, name => name.StartsWith("Npgsql"));
    }

    [Fact]
    public void Infrastructure_DoesNotReferencePresentation()
    {
        var references = ReferencesOf(typeof(FlightsDbContext).Assembly);

        Assert.DoesNotContain(references, IsPresentation);
    }

    [Fact]
    public void WebPresentation_DoesNotReferenceInfrastructure()
    {
        var references = ReferencesOf(typeof(FlightsPresentationAssembly).Assembly);

        Assert.DoesNotContain(references, IsInfrastructure);
    }

    [Fact]
    public void ApiPresentation_IsTheOnlyPresentationAllowedToComposeInfrastructure()
    {
        var references = ReferencesOf(typeof(FlightsModule).Assembly);

        Assert.Contains(references, IsInfrastructure);
    }

    [Fact]
    public void AuthDomain_DoesNotReferenceOuterLayers()
    {
        var references = ReferencesOf(typeof(AuthIdentity).Assembly);

        Assert.DoesNotContain(references, IsApplication);
        Assert.DoesNotContain(references, IsInfrastructure);
        Assert.DoesNotContain(references, IsPresentation);
    }

    [Fact]
    public void AuthApplication_DoesNotReferenceSecurityAdapters()
    {
        var references = ReferencesOf(typeof(IAccessTokenIssuer).Assembly);

        Assert.DoesNotContain(references, IsInfrastructure);
        Assert.DoesNotContain(references, IsPresentation);
    }

    [Fact]
    public void AuthInfrastructure_DoesNotReferencePresentation()
    {
        var references = ReferencesOf(typeof(JwtOptions).Assembly);

        Assert.DoesNotContain(references, IsPresentation);
    }

    [Fact]
    public void AuthWebPresentation_DoesNotReferenceInfrastructure()
    {
        var references = ReferencesOf(typeof(AuthPresentationAssembly).Assembly);

        Assert.DoesNotContain(references, IsInfrastructure);
    }

    [Fact]
    public void BookingsRespectsLayerBoundaries()
    {
        var domainReferences = ReferencesOf(typeof(Booking).Assembly);
        var applicationReferences = ReferencesOf(typeof(SearchBookingsHandler).Assembly);
        var infrastructureReferences = ReferencesOf(typeof(BookingsDbContext).Assembly);
        var webReferences = ReferencesOf(typeof(BookingsPresentationAssembly).Assembly);

        Assert.DoesNotContain(domainReferences, IsApplication);
        Assert.DoesNotContain(domainReferences, IsInfrastructure);
        Assert.DoesNotContain(domainReferences, IsPresentation);
        Assert.DoesNotContain(applicationReferences, IsInfrastructure);
        Assert.DoesNotContain(applicationReferences, IsPresentation);
        Assert.DoesNotContain(infrastructureReferences, IsPresentation);
        Assert.DoesNotContain(webReferences, IsInfrastructure);
    }

    private static string[] ReferencesOf(Assembly assembly) =>
        assembly.GetReferencedAssemblies().Select(reference => reference.Name ?? string.Empty).ToArray();

    private static bool IsApplication(string name) =>
        name.Contains(".Application", StringComparison.Ordinal);

    private static bool IsInfrastructure(string name) =>
        name.Contains(".Infrastructure", StringComparison.Ordinal);

    private static bool IsPresentation(string name) =>
        name.Contains(".Presentation", StringComparison.Ordinal);
}
