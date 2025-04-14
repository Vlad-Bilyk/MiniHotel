using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniHotel.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ReplaceRoomCategoryEnumInRoomType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Category",
                table: "RoomTypes",
                newName: "RoomCategory");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RoomCategory",
                table: "RoomTypes",
                newName: "Category");
        }
    }
}
