using Microsoft.EntityFrameworkCore.Migrations;

namespace HotelManagement.Data.Migrations
{
    public partial class addedDbSetForBookingHistory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookingHistory_AspNetUsers_AppUserId",
                table: "BookingHistory");

            migrationBuilder.DropForeignKey(
                name: "FK_BookingHistory_RoomTbl_RoomId",
                table: "BookingHistory");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BookingHistory",
                table: "BookingHistory");

            migrationBuilder.RenameTable(
                name: "BookingHistory",
                newName: "BookingHistoryTbl");

            migrationBuilder.RenameIndex(
                name: "IX_BookingHistory_RoomId",
                table: "BookingHistoryTbl",
                newName: "IX_BookingHistoryTbl_RoomId");

            migrationBuilder.RenameIndex(
                name: "IX_BookingHistory_AppUserId",
                table: "BookingHistoryTbl",
                newName: "IX_BookingHistoryTbl_AppUserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BookingHistoryTbl",
                table: "BookingHistoryTbl",
                column: "BookingHistoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_BookingHistoryTbl_AspNetUsers_AppUserId",
                table: "BookingHistoryTbl",
                column: "AppUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BookingHistoryTbl_RoomTbl_RoomId",
                table: "BookingHistoryTbl",
                column: "RoomId",
                principalTable: "RoomTbl",
                principalColumn: "RoomId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookingHistoryTbl_AspNetUsers_AppUserId",
                table: "BookingHistoryTbl");

            migrationBuilder.DropForeignKey(
                name: "FK_BookingHistoryTbl_RoomTbl_RoomId",
                table: "BookingHistoryTbl");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BookingHistoryTbl",
                table: "BookingHistoryTbl");

            migrationBuilder.RenameTable(
                name: "BookingHistoryTbl",
                newName: "BookingHistory");

            migrationBuilder.RenameIndex(
                name: "IX_BookingHistoryTbl_RoomId",
                table: "BookingHistory",
                newName: "IX_BookingHistory_RoomId");

            migrationBuilder.RenameIndex(
                name: "IX_BookingHistoryTbl_AppUserId",
                table: "BookingHistory",
                newName: "IX_BookingHistory_AppUserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BookingHistory",
                table: "BookingHistory",
                column: "BookingHistoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_BookingHistory_AspNetUsers_AppUserId",
                table: "BookingHistory",
                column: "AppUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BookingHistory_RoomTbl_RoomId",
                table: "BookingHistory",
                column: "RoomId",
                principalTable: "RoomTbl",
                principalColumn: "RoomId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
