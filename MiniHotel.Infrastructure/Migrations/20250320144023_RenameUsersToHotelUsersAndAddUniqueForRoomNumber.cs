using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniHotel.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenameUsersToHotelUsersAndAddUniqueForRoomNumber : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Users_UserId",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_AspNetUsers_UserId",
                table: "Users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Users",
                table: "Users");

            migrationBuilder.RenameTable(
                name: "Users",
                newName: "HotelUsers");

            migrationBuilder.AddPrimaryKey(
                name: "PK_HotelUsers",
                table: "HotelUsers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_RoomNumber",
                table: "Rooms",
                column: "RoomNumber",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_HotelUsers_UserId",
                table: "Bookings",
                column: "UserId",
                principalTable: "HotelUsers",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_HotelUsers_AspNetUsers_UserId",
                table: "HotelUsers",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_HotelUsers_UserId",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_HotelUsers_AspNetUsers_UserId",
                table: "HotelUsers");

            migrationBuilder.DropIndex(
                name: "IX_Rooms_RoomNumber",
                table: "Rooms");

            migrationBuilder.DropPrimaryKey(
                name: "PK_HotelUsers",
                table: "HotelUsers");

            migrationBuilder.RenameTable(
                name: "HotelUsers",
                newName: "Users");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Users",
                table: "Users",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Users_UserId",
                table: "Bookings",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_AspNetUsers_UserId",
                table: "Users",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
