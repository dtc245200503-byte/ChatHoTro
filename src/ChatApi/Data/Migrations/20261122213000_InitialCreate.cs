using ChatApi.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChatApi.Data.Migrations;

[DbContext(typeof(ChatDbContext))]
[Migration("20261122213000_InitialCreate")]
public sealed class InitialCreate : Migration
{
    protected override void BuildTargetModel(ModelBuilder modelBuilder) =>
        ChatDbContextModelSnapshot.ConfigureModel(modelBuilder);

    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Conversations",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                CustomerExternalId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                CustomerName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                OrderCode = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                Subject = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                Status = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: false),
                CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                ClosedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
            },
            constraints: table => table.PrimaryKey("PK_Conversations", conversation => conversation.Id));

        migrationBuilder.CreateTable(
            name: "Participants",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                ConversationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                ExternalId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                Role = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: false),
                JoinedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Participants", participant => participant.Id);
                table.ForeignKey(
                    name: "FK_Participants_Conversations_ConversationId",
                    column: participant => participant.ConversationId,
                    principalTable: "Conversations",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "Messages",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                ConversationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                ParticipantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Content = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                SentAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Messages", message => message.Id);
                table.ForeignKey(
                    name: "FK_Messages_Conversations_ConversationId",
                    column: message => message.ConversationId,
                    principalTable: "Conversations",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_Messages_Participants_ParticipantId",
                    column: message => message.ParticipantId,
                    principalTable: "Participants",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateIndex(
            name: "IX_Conversations_CreatedAt",
            table: "Conversations",
            column: "CreatedAt");
        migrationBuilder.CreateIndex(
            name: "IX_Conversations_Status",
            table: "Conversations",
            column: "Status");
        migrationBuilder.CreateIndex(
            name: "IX_Messages_ConversationId_SentAt",
            table: "Messages",
            columns: new[] { "ConversationId", "SentAt" });
        migrationBuilder.CreateIndex(
            name: "IX_Messages_ParticipantId",
            table: "Messages",
            column: "ParticipantId");
        migrationBuilder.CreateIndex(
            name: "IX_Participants_ConversationId_ExternalId",
            table: "Participants",
            columns: new[] { "ConversationId", "ExternalId" });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "Messages");
        migrationBuilder.DropTable(name: "Participants");
        migrationBuilder.DropTable(name: "Conversations");
    }
}
