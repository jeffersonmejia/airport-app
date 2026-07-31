using Microsoft.EntityFrameworkCore;

namespace Airport.Features.Administration.Infrastructure.Persistence;

public sealed class AdministrationDbContext(DbContextOptions<AdministrationDbContext> options)
    : DbContext(options);
