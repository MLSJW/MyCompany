using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyCompany.Migrations
{
    /// <inheritdoc />
    public partial class AddPriceToService : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Price",
                table: "Services",
                type: "int",
                nullable: false,
                defaultValue: 0);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Price",
                table: "Services");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "CD37CD19-2F81-4737-A1D3-95A95869A21B",
                column: "ConcurrencyStamp",
                value: "dfc45cec-7a5f-494e-9009-07c91d9dd9fc");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "ACCFA7D5-998D-42DE-ABB2-F94979E32468",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "b4b4c41d-8b68-417d-bc52-b01aa32cdc70", "AQAAAAIAAYagAAAAELTA6NviWMHI4yaHfacnxliuK9GgRJ65lhKlMtKH1G9LXxpLgPiOhmivLc0Q8Vavrw==" });
        }
    }
}
