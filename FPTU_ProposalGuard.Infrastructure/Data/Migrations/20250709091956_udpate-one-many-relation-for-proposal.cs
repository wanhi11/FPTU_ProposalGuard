using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FPTU_ProposalGuard.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class udpateonemanyrelationforproposal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ProposalSupervisor_ProposalId",
                table: "Proposal_Supervisor");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProposalStudent_ProposalId",
                table: "Proposal_Student");

            migrationBuilder.AddColumn<int>(
                name: "proposal_supervisor_id",
                table: "Proposal_Supervisor",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<int>(
                name: "proposal_student_id",
                table: "Proposal_Student",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProposalSupervisor_ProposalSupervisorId",
                table: "Proposal_Supervisor",
                column: "proposal_supervisor_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProposalStudent_ProposalStudentId",
                table: "Proposal_Student",
                column: "proposal_student_id");

            migrationBuilder.CreateIndex(
                name: "IX_Proposal_Supervisor_project_proposal_id",
                table: "Proposal_Supervisor",
                column: "project_proposal_id");

            migrationBuilder.CreateIndex(
                name: "IX_Proposal_Student_project_proposal_id",
                table: "Proposal_Student",
                column: "project_proposal_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ProposalSupervisor_ProposalSupervisorId",
                table: "Proposal_Supervisor");

            migrationBuilder.DropIndex(
                name: "IX_Proposal_Supervisor_project_proposal_id",
                table: "Proposal_Supervisor");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProposalStudent_ProposalStudentId",
                table: "Proposal_Student");

            migrationBuilder.DropIndex(
                name: "IX_Proposal_Student_project_proposal_id",
                table: "Proposal_Student");

            migrationBuilder.DropColumn(
                name: "proposal_supervisor_id",
                table: "Proposal_Supervisor");

            migrationBuilder.DropColumn(
                name: "proposal_student_id",
                table: "Proposal_Student");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProposalSupervisor_ProposalId",
                table: "Proposal_Supervisor",
                column: "project_proposal_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProposalStudent_ProposalId",
                table: "Proposal_Student",
                column: "project_proposal_id");
        }
    }
}
