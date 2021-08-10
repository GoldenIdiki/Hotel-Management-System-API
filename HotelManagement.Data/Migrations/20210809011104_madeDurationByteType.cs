using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace HotelManagement.Data.Migrations
{
    public partial class madeDurationByteType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<byte>(
                name: "Duration",
                table: "BookingHistoryTbl",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(TimeSpan),
                oldType: "TEXT");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<TimeSpan>(
                name: "Duration",
                table: "BookingHistoryTbl",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "INTEGER");
        }
    }
}
