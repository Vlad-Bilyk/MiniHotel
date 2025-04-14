using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniHotel.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MakeRoomCategoryInRoomTypeIsUnique : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_RoomTypes_RoomCategory",
                table: "RoomTypes",
                column: "RoomCategory",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_RoomTypes_RoomCategory",
                table: "RoomTypes");
        }
    }
}
