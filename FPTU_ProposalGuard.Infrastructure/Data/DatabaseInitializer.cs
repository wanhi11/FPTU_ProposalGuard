using System.ComponentModel;
using System.Reflection;
using FPTU_ProposalGuard.Domain.Common.Enums;
using FPTU_ProposalGuard.Domain.Entities;
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
            if (!await context.Users.AnyAsync()) await SeedUserRoleAsync();
            if(!await context.Semesters.AnyAsync()) await SeedSemesterAsync();
        }
        catch (Exception ex)
        {
            logger.Error(ex, "An error occurred while performing seed data.");
        }
    }

    private async Task SeedUserRoleAsync()
    {
        List<SystemRole> roles = new()
        {
            new()
            {
                RoleName = nameof(Role.Administration),
                NormalizedName = nameof(Role.Administration).ToUpper(),
                Description = Role.Administration.GetDescription()
            },
            new()
            {
                RoleName = nameof(Role.Reviewer),
                NormalizedName = nameof(Role.Reviewer).ToUpper(),
                Description = Role.Reviewer.GetDescription()
            },
            new()
            {
                RoleName = nameof(Role.Lecturer),
                NormalizedName = nameof(Role.Lecturer).ToUpper(),
                Description = Role.Lecturer.GetDescription()
            }
        };
        // Add range
        await context.AddRangeAsync(roles);
        // Save DB
        var isSaved = await context.SaveChangesAsync() > 0;
        if (isSaved) logger.Information($"[ROLE] Seed {roles} data successfully");
        else return;
        
        List<User> users = new()
        {
            new()
            {
                Email = "admin@gmail.com",
                FirstName = "Admin",
                PasswordHash = BC.EnhancedHashPassword("@Admin123", 13),
                IsActive = true,
                EmailConfirmed = true,
                IsDeleted = false,
                CreateDate = DateTime.UtcNow,
                TwoFactorEnabled = false,
                RoleId = roles.First(r => r.RoleName == nameof(Role.Administration)).RoleId
            },
            new()
            {
                Email = "doanvietthanhhs@gmail.com",
                FirstName = "Chu",
                LastName = "Be",
                PasswordHash = BC.EnhancedHashPassword("@Admin123", 13),
                IsActive = true,
                EmailConfirmed = true,
                IsDeleted = false,
                CreateDate = DateTime.UtcNow,
                TwoFactorEnabled = false,
                RoleId = roles.First(r => r.RoleName == nameof(Role.Administration)).RoleId
            }
        };
        
        // Add range
        await context.AddRangeAsync(users);
        // Save change
        isSaved = await context.SaveChangesAsync() > 0;
        if(isSaved) logger.Information($"[USER] Seed {users} data successfully");
    }
    private async Task SeedSemesterAsync()
    {
        List<Semester> semesters = new()
        {
            new()
            {
                SemesterId = 1,
                Term = Term.Spring,
                Year = 2025,
                SemesterCode = "SP25"
            },
            new()
            {
                SemesterId = 2,
                Term = Term.Summer,
                Year = 2025,
                SemesterCode = "SU25"
            }
        };
    
        // Add range
        await context.AddRangeAsync(semesters);
        // Save change
        var isSaved = await context.SaveChangesAsync() > 0;
        if(isSaved) logger.Information($"[SEMESTER] Seed {semesters} data successfully");
    }
}
public static class DatabaseInitializerExtensions
{
    public static string GetDescription(this System.Enum value)
    {
        var field = value.GetType().GetField(value.ToString());
        var attribute = field?.GetCustomAttribute<DescriptionAttribute>();

        return attribute?.Description ?? value.ToString();
    }
}

