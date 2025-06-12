using FPTU_ProposalGuard.Domain.Interfaces;
using FPTU_ProposalGuard.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace FPTU_ProposalGuard.Infrastructure.Data;

public class DatabaseInitializer(FptuProposalGuardDbContext context, ILogger logger) : IDatabaseInitializer
{
    public async Task InitializeAsync()
    {
        try
        {
            // Check whether the database exists and can be connected to
            if (!await context.Database.CanConnectAsync())
            {
                // Check for applied migrations
                var appliedMigrations = await context.Database.GetAppliedMigrationsAsync();
                if (appliedMigrations.Any())
                {
                    logger.Information("Migrations have been applied.");
                    return;
                }

                // Perform migration if necessary
                await context.Database.MigrateAsync();
                logger.Information("Database initialized successfully.");
            }
            else
            {
                logger.Information("Database cannot be connected to.");
            }
        }
        catch (Exception ex)
        {
            logger.Error(ex, "An error occurred while initializing the database.");
        }
    }

    public async Task SeedAsync()
    {
        try
        {
            await TrySeedAsync();
        }
        catch (Exception ex)
        {
            logger.Error(ex, "An error occurred while seeding the database.");
        }
    }

    public async Task TrySeedAsync()
    {
        try
        {
            // Seed data here...
            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            logger.Error(ex, "An error occurred while performing seed data.");
        }
    }
}

