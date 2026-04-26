using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyCompany.Migrations
{
    /// <inheritdoc />
    public partial class AddTimeToService : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Time",
                table: "Services",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "CD37CD19-2F81-4737-A1D3-95A95869A21B",
                column: "ConcurrencyStamp",
                value: "dc3dd3b6-c5db-4e75-83b7-e5b7f4969dbe");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "ACCFA7D5-998D-42DE-ABB2-F94979E32468",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "85c09511-e2fd-4f47-a445-2df4b8264286", "AQAAAAIAAYagAAAAEHKxw64xpYmuB7Ago3OfkODTJlRlJV7jhexqb8dqKE0J+3H7d9cUkaFsKd7Qy9eeCg==" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Time",
                table: "Services");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "CD37CD19-2F81-4737-A1D3-95A95869A21B",
                column: "ConcurrencyStamp",
                value: "aac23195-d02a-4d12-a622-db53139468a9");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "ACCFA7D5-998D-42DE-ABB2-F94979E32468",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "24b834d9-032b-4e65-a5e3-4f8f8fcc4906", "AQAAAAIAAYagAAAAEP7f1NR9aiBBG1FM6/ksPdZoFG/GjwgWbTuH1mHIjQXVvYADc8Bdoya1fsiPXlxdIw==" });
        }
    }
}
