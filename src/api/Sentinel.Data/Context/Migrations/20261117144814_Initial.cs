using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sentinel.Data.Context.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "audit_records",
                columns: table => new
                {
                    id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    user_id = table.Column<string>(type: "TEXT", maxLength: 36, nullable: false),
                    timestamp = table.Column<DateTime>(type: "TEXT", nullable: false),
                    description = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    data = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_audit_records", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<string>(type: "TEXT", maxLength: 36, nullable: false),
                    remote_id = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    email = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    type = table.Column<int>(type: "INTEGER", nullable: false),
                    cursor = table.Column<long>(type: "INTEGER", nullable: false),
                    date_created = table.Column<DateTime>(type: "TEXT", nullable: false),
                    date_modified = table.Column<DateTime>(type: "TEXT", nullable: false),
                    date_deleted = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "user_scopes",
                columns: table => new
                {
                    user_id = table.Column<string>(type: "TEXT", maxLength: 36, nullable: false),
                    scope = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_scopes", x => new { x.user_id, x.scope });
                    table.ForeignKey(
                        name: "FK_user_scopes_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "verifications",
                columns: table => new
                {
                    id = table.Column<string>(type: "TEXT", maxLength: 36, nullable: false),
                    user_id = table.Column<string>(type: "TEXT", maxLength: 36, nullable: false),
                    age = table.Column<int>(type: "INTEGER", nullable: false),
                    status = table.Column<int>(type: "INTEGER", nullable: false),
                    timestamp = table.Column<DateTime>(type: "TEXT", nullable: true),
                    cursor = table.Column<long>(type: "INTEGER", nullable: false),
                    date_created = table.Column<DateTime>(type: "TEXT", nullable: false),
                    date_modified = table.Column<DateTime>(type: "TEXT", nullable: false),
                    date_deleted = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_verifications", x => x.id);
                    table.ForeignKey(
                        name: "FK_verifications_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_audit_records_user_id_timestamp",
                table: "audit_records",
                columns: new[] { "user_id", "timestamp" });

            migrationBuilder.CreateIndex(
                name: "IX_users_cursor",
                table: "users",
                column: "cursor",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_users_date_created",
                table: "users",
                column: "date_created");

            migrationBuilder.CreateIndex(
                name: "IX_users_date_deleted",
                table: "users",
                column: "date_deleted");

            migrationBuilder.CreateIndex(
                name: "IX_users_date_modified",
                table: "users",
                column: "date_modified");

            migrationBuilder.CreateIndex(
                name: "IX_users_email",
                table: "users",
                column: "email",
                unique: true,
                filter: "date_deleted IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_users_remote_id",
                table: "users",
                column: "remote_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_users_type",
                table: "users",
                column: "type");

            migrationBuilder.CreateIndex(
                name: "IX_verifications_cursor",
                table: "verifications",
                column: "cursor",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_verifications_date_created",
                table: "verifications",
                column: "date_created");

            migrationBuilder.CreateIndex(
                name: "IX_verifications_date_deleted",
                table: "verifications",
                column: "date_deleted");

            migrationBuilder.CreateIndex(
                name: "IX_verifications_date_modified",
                table: "verifications",
                column: "date_modified");

            migrationBuilder.CreateIndex(
                name: "IX_verifications_user_id_status_age",
                table: "verifications",
                columns: new[] { "user_id", "status", "age" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "audit_records");

            migrationBuilder.DropTable(
                name: "user_scopes");

            migrationBuilder.DropTable(
                name: "verifications");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}