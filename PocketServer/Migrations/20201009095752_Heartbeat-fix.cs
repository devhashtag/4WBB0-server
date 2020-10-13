using Microsoft.EntityFrameworkCore.Migrations;

namespace PocketServer.Migrations
{
    public partial class Heartbeatfix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Heartbeats_Devices_DeviceId1",
                table: "Heartbeats");

            migrationBuilder.DropIndex(
                name: "IX_Heartbeats_DeviceId1",
                table: "Heartbeats");

            migrationBuilder.DropColumn(
                name: "DeviceId1",
                table: "Heartbeats");

            migrationBuilder.AlterColumn<string>(
                name: "DeviceId",
                table: "Heartbeats",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_Heartbeats_DeviceId",
                table: "Heartbeats",
                column: "DeviceId");

            migrationBuilder.AddForeignKey(
                name: "FK_Heartbeats_Devices_DeviceId",
                table: "Heartbeats",
                column: "DeviceId",
                principalTable: "Devices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Heartbeats_Devices_DeviceId",
                table: "Heartbeats");

            migrationBuilder.DropIndex(
                name: "IX_Heartbeats_DeviceId",
                table: "Heartbeats");

            migrationBuilder.AlterColumn<int>(
                name: "DeviceId",
                table: "Heartbeats",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeviceId1",
                table: "Heartbeats",
                type: "varchar(255) CHARACTER SET utf8mb4",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Heartbeats_DeviceId1",
                table: "Heartbeats",
                column: "DeviceId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Heartbeats_Devices_DeviceId1",
                table: "Heartbeats",
                column: "DeviceId1",
                principalTable: "Devices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
