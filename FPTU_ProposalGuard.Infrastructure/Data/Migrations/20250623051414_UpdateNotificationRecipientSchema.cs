using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FPTU_ProposalGuard.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateNotificationRecipientSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notification_Recipient",
                table: "Notification");

            migrationBuilder.DropIndex(
                name: "IX_Notification_recipient_id",
                table: "Notification");

            migrationBuilder.RenameColumn(
                name: "recipient_id",
                table: "Notification",
                newName: "RecipientId");

            migrationBuilder.RenameColumn(
                name: "is_read",
                table: "Notification",
                newName: "is_public");

            migrationBuilder.CreateTable(
                name: "Notification_Recipient",
                columns: table => new
                {
                    notification_recipient_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    notification_id = table.Column<int>(type: "int", nullable: false),
                    recipient_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    is_read = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationRecipient_NotificationRecipientId", x => x.notification_recipient_id);
                    table.ForeignKey(
                        name: "FK_NotificationRecipient_NotificationId",
                        column: x => x.notification_id,
                        principalTable: "Notification",
                        principalColumn: "notification_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NotificationRecipient_UserId",
                        column: x => x.recipient_id,
                        principalTable: "User",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Notification_Recipient_notification_id",
                table: "Notification_Recipient",
                column: "notification_id");

            migrationBuilder.CreateIndex(
                name: "IX_Notification_Recipient_recipient_id",
                table: "Notification_Recipient",
                column: "recipient_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Notification_Recipient");

            migrationBuilder.RenameColumn(
                name: "RecipientId",
                table: "Notification",
                newName: "recipient_id");

            migrationBuilder.RenameColumn(
                name: "is_public",
                table: "Notification",
                newName: "is_read");

            migrationBuilder.CreateIndex(
                name: "IX_Notification_recipient_id",
                table: "Notification",
                column: "recipient_id");

            migrationBuilder.AddForeignKey(
                name: "FK_Notification_Recipient",
                table: "Notification",
                column: "recipient_id",
                principalTable: "User",
                principalColumn: "user_id");
        }
    }
}
