using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccess.Migrations
{
    public partial class AddHotelRoomSeed : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "HotelRooms",
                columns: new[] { "Id", "CreatedBy", "CreatedDate", "Details", "Name", "Occupancy", "RegularRate", "SqFt", "UpdatedBy", "UpdatedDate" },
                values: new object[] { 1, null, new DateTime(2021, 11, 9, 23, 1, 6, 967, DateTimeKind.Utc).AddTicks(8999), "Earth Federation Headquarters", "Jaburo", 1000, 200.0, null, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.InsertData(
                table: "HotelRooms",
                columns: new[] { "Id", "CreatedBy", "CreatedDate", "Details", "Name", "Occupancy", "RegularRate", "SqFt", "UpdatedBy", "UpdatedDate" },
                values: new object[] { 2, null, new DateTime(2021, 11, 9, 23, 1, 6, 967, DateTimeKind.Utc).AddTicks(9718), "The Mothership of 'Celestial Being'", "The Ptolemy", 30, 75.0, null, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.InsertData(
                table: "HotelRooms",
                columns: new[] { "Id", "CreatedBy", "CreatedDate", "Details", "Name", "Occupancy", "RegularRate", "SqFt", "UpdatedBy", "UpdatedDate" },
                values: new object[] { 3, null, new DateTime(2021, 11, 9, 23, 1, 6, 967, DateTimeKind.Utc).AddTicks(9721), "Lunar City built around Neil Armstrongs footprints on the moon", "Von Braun City", 2000, 125.0, null, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "HotelRooms",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "HotelRooms",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "HotelRooms",
                keyColumn: "Id",
                keyValue: 3);
        }
    }
}
