using FPTU_ProposalGuard.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FPTU_ProposalGuard.Infrastructure.Data.Configurations;

public class SemesterConfiguration : IEntityTypeConfiguration<Semester>
{
    public void Configure(EntityTypeBuilder<Semester> builder)
    {
        builder.HasKey(e => e.SemesterId).HasName("PK_Semester_SemesterId");

        builder.ToTable("Semester");

        builder.Property(e => e.SemesterId).HasColumnName("semester_id");
        builder.Property(e => e.SemesterCode)
            .HasMaxLength(155)
            .HasColumnName("semester_code");
        builder.Property(e => e.Term)
            .HasMaxLength(50)
            .HasConversion<string>()
            .HasColumnName("term");
        builder.Property(e => e.Year).HasColumnName("year");
    }
}