using FPTU_ProposalGuard.Domain.Interfaces;
using FPTU_ProposalGuard.Domain.Interfaces.Repositories;
using FPTU_ProposalGuard.Infrastructure.Data;
using FPTU_ProposalGuard.Infrastructure.Data.Context;
using FPTU_ProposalGuard.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FPTU_ProposalGuard.Infrastructure;

public static class DependencyInjection
{
    //	Summary:
    //		This class is to configure services for infrastructure layer
    public static IServiceCollection AddInfrastructure(this IServiceCollection services,
        IConfiguration configuration)
    {
        // Retrieve connectionStr from application configuration
        var connectionString = configuration.GetConnectionString("DefaultConnectionStr");
            
        // Add application DbContext 
        services.AddDbContext<FptuProposalGuardDbContext>(options => options.UseSqlServer(connectionString));

        // Register DI 
        services.AddScoped<IDatabaseInitializer, DatabaseInitializer>();
        services.AddScoped(typeof(IGenericRepository<,>), typeof(GenericRepository<,>));
        services.AddScoped<IUnitOfWork, UnitOfWork>();
 
        return services;
    }
}