using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FPTU_ProposalGuard.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Semester",
                columns: table => new
                {
                    semester_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    semester_code = table.Column<string>(type: "nvarchar(155)", maxLength: 155, nullable: false),
                    year = table.Column<int>(type: "int", nullable: false),
                    term = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Semester_SemesterId", x => x.semester_id);
                });

            migrationBuilder.CreateTable(
                name: "System_Message",
                columns: table => new
                {
                    msg_id = table.Column<string>(type: "nvarchar(50)", nullable: false),
                    msg_content = table.Column<string>(type: "nvarchar(1500)", maxLength: 1500, nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: true),
                    created_by = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    updated_by = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemMessage_MsgId", x => x.msg_id);
                });

            migrationBuilder.CreateTable(
                name: "System_Role",
                columns: table => new
                {
                    role_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    role_name = table.Column<string>(type: "nvarchar(155)", maxLength: 155, nullable: false),
                    normalized_name = table.Column<string>(type: "nvarchar(155)", maxLength: 155, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemRole_RoleId", x => x.role_id);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newsequentialid())"),
                    email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    password_hash = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    first_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    last_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    dob = table.Column<DateTime>(type: "datetime", nullable: true),
                    phone = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    avatar = table.Column<string>(type: "varchar(2048)", unicode: false, maxLength: 2048, nullable: true),
                    gender = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    is_active = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    is_deleted = table.Column<bool>(type: "bit", nullable: false),
                    create_date = table.Column<DateTime>(type: "datetime", nullable: false),
                    modified_date = table.Column<DateTime>(type: "datetime", nullable: true),
                    modified_by = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    two_factor_enabled = table.Column<bool>(type: "bit", nullable: false),
                    phone_number_confirmed = table.Column<bool>(type: "bit", nullable: false),
                    email_confirmed = table.Column<bool>(type: "bit", nullable: false),
                    two_factor_secret_key = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    two_factor_backup_codes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    phone_verification_code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    phone_verification_expiry = table.Column<DateTime>(type: "datetime", nullable: true),
                    role_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User_UserId", x => x.user_id);
                    table.ForeignKey(
                        name: "FK_User_RoleId",
                        column: x => x.role_id,
                        principalTable: "System_Role",
                        principalColumn: "role_id");
                });

            migrationBuilder.CreateTable(
                name: "Notification",
                columns: table => new
                {
                    notification_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    recipient_id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    is_read = table.Column<bool>(type: "bit", nullable: false),
                    create_date = table.Column<DateTime>(type: "datetime", nullable: false),
                    created_by_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notification_NotificationId", x => x.notification_id);
                    table.ForeignKey(
                        name: "FK_Notification_CreatedBy",
                        column: x => x.created_by_id,
                        principalTable: "User",
                        principalColumn: "user_id");
                    table.ForeignKey(
                        name: "FK_Notification_Recipient",
                        column: x => x.recipient_id,
                        principalTable: "User",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "Project_Proposal",
                columns: table => new
                {
                    project_proposal_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    semester_id = table.Column<int>(type: "int", nullable: false),
                    vie_title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    eng_title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    abbreviation = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    supervisor_name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    duration_from = table.Column<DateOnly>(type: "date", nullable: false),
                    duration_to = table.Column<DateOnly>(type: "date", nullable: false),
                    profession = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    specialty_code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    kinds_of_person = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    context_text = table.Column<string>(type: "ntext", nullable: false),
                    solution_text = table.Column<string>(type: "ntext", nullable: false),
                    functional_requirements = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    main_proposal_content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    submitter_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    approver_id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: false),
                    created_by = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: true),
                    updated_by = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectProposal_ProjectProposalId", x => x.project_proposal_id);
                    table.ForeignKey(
                        name: "FK_ProjectProposal_ApproverId",
                        column: x => x.approver_id,
                        principalTable: "User",
                        principalColumn: "user_id");
                    table.ForeignKey(
                        name: "FK_ProjectProposal_SemesterId",
                        column: x => x.semester_id,
                        principalTable: "Semester",
                        principalColumn: "semester_id");
                    table.ForeignKey(
                        name: "FK_ProjectProposal_SubmitterId",
                        column: x => x.submitter_id,
                        principalTable: "User",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "Refresh_Token",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    refresh_token_id = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    token_id = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    refresh_count = table.Column<int>(type: "int", nullable: false),
                    create_date = table.Column<DateTime>(type: "datetime", nullable: false),
                    expiry_date = table.Column<DateTime>(type: "datetime", nullable: false),
                    user_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("Pk_RefreshToken_Id", x => x.id);
                    table.ForeignKey(
                        name: "FK_RefreshToken_UserId",
                        column: x => x.user_id,
                        principalTable: "User",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Proposal_History",
                columns: table => new
                {
                    history_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    project_proposal_id = table.Column<int>(type: "int", nullable: false),
                    old_status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    new_status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    process_by_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    process_date = table.Column<DateTime>(type: "datetime", nullable: false),
                    comment = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProposalHistory_HistoryId", x => x.history_id);
                    table.ForeignKey(
                        name: "FK_ProposalHistory_ProjectProposalId",
                        column: x => x.project_proposal_id,
                        principalTable: "Project_Proposal",
                        principalColumn: "project_proposal_id");
                    table.ForeignKey(
                        name: "PK_ProposalHistory_ProcessById",
                        column: x => x.process_by_id,
                        principalTable: "User",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "Proposal_Similarity",
                columns: table => new
                {
                    similarity_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    checked_proposal_id = table.Column<int>(type: "int", nullable: false),
                    existing_proposal_id = table.Column<int>(type: "int", nullable: false),
                    title_score = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    context_score = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    solution_score = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    overall_score = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    checked_on = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProposalSimilarity_SimilarityId", x => x.similarity_id);
                    table.ForeignKey(
                        name: "FK_ProposalSimilaryty_CheckedProposalId",
                        column: x => x.checked_proposal_id,
                        principalTable: "Project_Proposal",
                        principalColumn: "project_proposal_id");
                    table.ForeignKey(
                        name: "FK_ProposalSimilaryty_ExistingProposalId",
                        column: x => x.existing_proposal_id,
                        principalTable: "Project_Proposal",
                        principalColumn: "project_proposal_id");
                });

            migrationBuilder.CreateTable(
                name: "Proposal_Student",
                columns: table => new
                {
                    project_proposal_id = table.Column<int>(type: "int", nullable: false),
                    student_code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    full_name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    phone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    role_in_group = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProposalStudent_ProposalId", x => x.project_proposal_id);
                    table.ForeignKey(
                        name: "FK_ProposalStudent_ProposalId",
                        column: x => x.project_proposal_id,
                        principalTable: "Project_Proposal",
                        principalColumn: "project_proposal_id");
                });

            migrationBuilder.CreateTable(
                name: "Proposal_Supervisor",
                columns: table => new
                {
                    project_proposal_id = table.Column<int>(type: "int", nullable: false),
                    supervisor_no = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: true),
                    full_name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    phone = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    title_prefix = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProposalSupervisor_ProposalId", x => x.project_proposal_id);
                    table.ForeignKey(
                        name: "FK_ProposalSupervisor_ProposalId",
                        column: x => x.project_proposal_id,
                        principalTable: "Project_Proposal",
                        principalColumn: "project_proposal_id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Notification_created_by_id",
                table: "Notification",
                column: "created_by_id");

            migrationBuilder.CreateIndex(
                name: "IX_Notification_recipient_id",
                table: "Notification",
                column: "recipient_id");

            migrationBuilder.CreateIndex(
                name: "IX_Project_Proposal_approver_id",
                table: "Project_Proposal",
                column: "approver_id");

            migrationBuilder.CreateIndex(
                name: "IX_Project_Proposal_semester_id",
                table: "Project_Proposal",
                column: "semester_id");

            migrationBuilder.CreateIndex(
                name: "IX_Project_Proposal_submitter_id",
                table: "Project_Proposal",
                column: "submitter_id");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectProposal_EngTitle",
                table: "Project_Proposal",
                column: "eng_title");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectProposal_VieTitle",
                table: "Project_Proposal",
                column: "vie_title");

            migrationBuilder.CreateIndex(
                name: "IX_Proposal_History_process_by_id",
                table: "Proposal_History",
                column: "process_by_id");

            migrationBuilder.CreateIndex(
                name: "IX_Proposal_History_project_proposal_id",
                table: "Proposal_History",
                column: "project_proposal_id");

            migrationBuilder.CreateIndex(
                name: "IX_Proposal_Similarity_checked_proposal_id",
                table: "Proposal_Similarity",
                column: "checked_proposal_id");

            migrationBuilder.CreateIndex(
                name: "IX_Proposal_Similarity_existing_proposal_id",
                table: "Proposal_Similarity",
                column: "existing_proposal_id");

            migrationBuilder.CreateIndex(
                name: "IX_Refresh_Token_user_id",
                table: "Refresh_Token",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_User_role_id",
                table: "User",
                column: "role_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Notification");

            migrationBuilder.DropTable(
                name: "Proposal_History");

            migrationBuilder.DropTable(
                name: "Proposal_Similarity");

            migrationBuilder.DropTable(
                name: "Proposal_Student");

            migrationBuilder.DropTable(
                name: "Proposal_Supervisor");

            migrationBuilder.DropTable(
                name: "Refresh_Token");

            migrationBuilder.DropTable(
                name: "System_Message");

            migrationBuilder.DropTable(
                name: "Project_Proposal");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropTable(
                name: "Semester");

            migrationBuilder.DropTable(
                name: "System_Role");
        }
    }
}
