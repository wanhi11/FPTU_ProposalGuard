using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FPTU_ProposalGuard.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updatehistorysimilarityrelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "PK_ProposalHistory_ProcessById",
                table: "Proposal_History");

            migrationBuilder.DropForeignKey(
                name: "FK_ProposalSimilaryty_CheckedProposalId",
                table: "Proposal_Similarity");

            migrationBuilder.DropForeignKey(
                name: "FK_ProposalSimilaryty_ExistingProposalId",
                table: "Proposal_Similarity");

            migrationBuilder.DropIndex(
                name: "IX_Proposal_Similarity_checked_proposal_id",
                table: "Proposal_Similarity");

            migrationBuilder.DropColumn(
                name: "checked_on",
                table: "Proposal_Similarity");

            migrationBuilder.DropColumn(
                name: "context_score",
                table: "Proposal_Similarity");

            migrationBuilder.DropColumn(
                name: "solution_score",
                table: "Proposal_Similarity");

            migrationBuilder.DropColumn(
                name: "new_status",
                table: "Proposal_History");

            migrationBuilder.RenameColumn(
                name: "title_score",
                table: "Proposal_Similarity",
                newName: "match_ratio");

            migrationBuilder.RenameColumn(
                name: "checked_proposal_id",
                table: "Proposal_Similarity",
                newName: "match_count");

            migrationBuilder.RenameColumn(
                name: "old_status",
                table: "Proposal_History",
                newName: "status");

            migrationBuilder.AddColumn<int>(
                name: "ProjectProposalId",
                table: "Proposal_Similarity",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "history_id",
                table: "Proposal_Similarity",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "longest_sequence",
                table: "Proposal_Similarity",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "url",
                table: "Proposal_History",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "version",
                table: "Proposal_History",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Proposal_Matched_Segment",
                columns: table => new
                {
                    segment_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    similarity_id = table.Column<int>(type: "int", nullable: false),
                    context = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    match_context = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    match_percentage = table.Column<decimal>(type: "decimal(5,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProposalMatchedSegment_SegmentId", x => x.segment_id);
                    table.ForeignKey(
                        name: "FK_ProposalMatchedSegment_SimilarityId",
                        column: x => x.similarity_id,
                        principalTable: "Proposal_Similarity",
                        principalColumn: "similarity_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Proposal_Similarity_history_id",
                table: "Proposal_Similarity",
                column: "history_id");

            migrationBuilder.CreateIndex(
                name: "IX_Proposal_Similarity_ProjectProposalId",
                table: "Proposal_Similarity",
                column: "ProjectProposalId");

            migrationBuilder.CreateIndex(
                name: "IX_Proposal_Matched_Segment_similarity_id",
                table: "Proposal_Matched_Segment",
                column: "similarity_id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProposalHistory_ProcessById",
                table: "Proposal_History",
                column: "process_by_id",
                principalTable: "User",
                principalColumn: "user_id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProposalSimilarity_ExistingProposalId",
                table: "Proposal_Similarity",
                column: "existing_proposal_id",
                principalTable: "Project_Proposal",
                principalColumn: "project_proposal_id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProposalSimilarity_HistoryId",
                table: "Proposal_Similarity",
                column: "history_id",
                principalTable: "Proposal_History",
                principalColumn: "history_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Proposal_Similarity_Project_Proposal_ProjectProposalId",
                table: "Proposal_Similarity",
                column: "ProjectProposalId",
                principalTable: "Project_Proposal",
                principalColumn: "project_proposal_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProposalHistory_ProcessById",
                table: "Proposal_History");

            migrationBuilder.DropForeignKey(
                name: "FK_ProposalSimilarity_ExistingProposalId",
                table: "Proposal_Similarity");

            migrationBuilder.DropForeignKey(
                name: "FK_ProposalSimilarity_HistoryId",
                table: "Proposal_Similarity");

            migrationBuilder.DropForeignKey(
                name: "FK_Proposal_Similarity_Project_Proposal_ProjectProposalId",
                table: "Proposal_Similarity");

            migrationBuilder.DropTable(
                name: "Proposal_Matched_Segment");

            migrationBuilder.DropIndex(
                name: "IX_Proposal_Similarity_history_id",
                table: "Proposal_Similarity");

            migrationBuilder.DropIndex(
                name: "IX_Proposal_Similarity_ProjectProposalId",
                table: "Proposal_Similarity");

            migrationBuilder.DropColumn(
                name: "ProjectProposalId",
                table: "Proposal_Similarity");

            migrationBuilder.DropColumn(
                name: "history_id",
                table: "Proposal_Similarity");

            migrationBuilder.DropColumn(
                name: "longest_sequence",
                table: "Proposal_Similarity");

            migrationBuilder.DropColumn(
                name: "url",
                table: "Proposal_History");

            migrationBuilder.DropColumn(
                name: "version",
                table: "Proposal_History");

            migrationBuilder.RenameColumn(
                name: "match_ratio",
                table: "Proposal_Similarity",
                newName: "title_score");

            migrationBuilder.RenameColumn(
                name: "match_count",
                table: "Proposal_Similarity",
                newName: "checked_proposal_id");

            migrationBuilder.RenameColumn(
                name: "status",
                table: "Proposal_History",
                newName: "old_status");

            migrationBuilder.AddColumn<DateTime>(
                name: "checked_on",
                table: "Proposal_Similarity",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<decimal>(
                name: "context_score",
                table: "Proposal_Similarity",
                type: "decimal(5,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "solution_score",
                table: "Proposal_Similarity",
                type: "decimal(5,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "new_status",
                table: "Proposal_History",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Proposal_Similarity_checked_proposal_id",
                table: "Proposal_Similarity",
                column: "checked_proposal_id");

            migrationBuilder.AddForeignKey(
                name: "PK_ProposalHistory_ProcessById",
                table: "Proposal_History",
                column: "process_by_id",
                principalTable: "User",
                principalColumn: "user_id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProposalSimilaryty_CheckedProposalId",
                table: "Proposal_Similarity",
                column: "checked_proposal_id",
                principalTable: "Project_Proposal",
                principalColumn: "project_proposal_id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProposalSimilaryty_ExistingProposalId",
                table: "Proposal_Similarity",
                column: "existing_proposal_id",
                principalTable: "Project_Proposal",
                principalColumn: "project_proposal_id");
        }
    }
}
