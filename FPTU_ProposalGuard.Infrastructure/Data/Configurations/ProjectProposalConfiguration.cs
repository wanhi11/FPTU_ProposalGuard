using FPTU_ProposalGuard.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FPTU_ProposalGuard.Infrastructure.Data.Configurations;

public class ProjectProposalConfiguration : IEntityTypeConfiguration<ProjectProposal>
{
    public void Configure(EntityTypeBuilder<ProjectProposal> builder)
    {
        builder.HasKey(e => e.ProjectProposalId).HasName("PK_ProjectProposal_ProjectProposalId");

        builder.ToTable("Project_Proposal");

        builder.HasIndex(e => e.EngTitle, "IX_ProjectProposal_EngTitle");

        builder.HasIndex(e => e.VieTitle, "IX_ProjectProposal_VieTitle");

        builder.Property(e => e.ProjectProposalId).HasColumnName("project_proposal_id");
        builder.Property(e => e.Abbreviation)
            .HasMaxLength(100)
            .HasColumnName("abbreviation");
        builder.Property(e => e.ApproverId).HasColumnName("approver_id");
        builder.Property(e => e.ContextText)
            .HasColumnType("ntext")
            .HasColumnName("context_text");
        builder.Property(e => e.CreatedAt)
            .HasColumnType("datetime")
            .HasColumnName("created_at");
        builder.Property(e => e.CreatedBy)
            .HasMaxLength(255)
            .HasColumnName("created_by");
        builder.Property(e => e.DurationFrom).HasColumnName("duration_from");
        builder.Property(e => e.DurationTo).HasColumnName("duration_to");
        builder.Property(e => e.EngTitle)
            .HasMaxLength(255)
            .HasColumnName("eng_title");
        builder.Property(e => e.FunctionalRequirements).HasColumnName("functional_requirements");
        builder.Property(e => e.NonFunctionalRequirements).HasColumnName("non_functional_requirements");
        builder.Property(e => e.TechnicalStack).HasColumnName("technical_stack");
        builder.Property(e => e.KindsOfPerson)
            .HasMaxLength(20)
            .HasColumnName("kinds_of_person");
        builder.Property(e => e.MainProposalContent).HasColumnName("main_proposal_content");
        builder.Property(e => e.Profession)
            .HasMaxLength(100)
            .HasColumnName("profession");
        builder.Property(e => e.SemesterId).HasColumnName("semester_id");
        builder.Property(e => e.SolutionText)
            .HasColumnType("ntext")
            .HasColumnName("solution_text");
        builder.Property(e => e.SpecialtyCode)
            .HasMaxLength(20)
            .HasColumnName("specialty_code");
        builder.Property(e => e.Status)
            .HasMaxLength(50)
            .HasConversion<string>()
            .HasColumnName("status");
        builder.Property(e => e.SubmitterId).HasColumnName("submitter_id");
        builder.Property(e => e.SupervisorName)
            .HasMaxLength(255)
            .HasColumnName("supervisor_name");
        builder.Property(e => e.UpdatedAt)
            .HasColumnType("datetime")
            .HasColumnName("updated_at");
        builder.Property(e => e.UpdatedBy)
            .HasMaxLength(255)
            .HasColumnName("updated_by");
        builder.Property(e => e.VieTitle)
            .HasMaxLength(255)
            .HasColumnName("vie_title");

        builder.HasOne(d => d.Approver).WithMany(p => p.ProjectProposalApprovers)
            .HasForeignKey(d => d.ApproverId)
            .HasConstraintName("FK_ProjectProposal_ApproverId");

        builder.HasOne(d => d.Semester).WithMany(p => p.ProjectProposals)
            .HasForeignKey(d => d.SemesterId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_ProjectProposal_SemesterId");

        builder.HasOne(d => d.Submitter).WithMany(p => p.ProjectProposalSubmitters)
            .HasForeignKey(d => d.SubmitterId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_ProjectProposal_SubmitterId");
    }
}