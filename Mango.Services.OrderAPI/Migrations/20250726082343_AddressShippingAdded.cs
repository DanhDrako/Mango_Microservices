using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mango.Services.OrderAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddressShippingAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OrderTime",
                table: "OrderHeaders");

            migrationBuilder.AddColumn<double>(
                name: "DeliveryFee",
                table: "OrderHeaders",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "PaymentSummary_Brand",
                table: "OrderHeaders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PaymentSummary_ExpMonth",
                table: "OrderHeaders",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PaymentSummary_ExpYear",
                table: "OrderHeaders",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PaymentSummary_Last4",
                table: "OrderHeaders",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ShippingAddress_City",
                table: "OrderHeaders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ShippingAddress_Country",
                table: "OrderHeaders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ShippingAddress_Line1",
                table: "OrderHeaders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ShippingAddress_Line2",
                table: "OrderHeaders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ShippingAddress_Name",
                table: "OrderHeaders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ShippingAddress_PostalCode",
                table: "OrderHeaders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ShippingAddress_State",
                table: "OrderHeaders",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeliveryFee",
                table: "OrderHeaders");

            migrationBuilder.DropColumn(
                name: "PaymentSummary_Brand",
                table: "OrderHeaders");

            migrationBuilder.DropColumn(
                name: "PaymentSummary_ExpMonth",
                table: "OrderHeaders");

            migrationBuilder.DropColumn(
                name: "PaymentSummary_ExpYear",
                table: "OrderHeaders");

            migrationBuilder.DropColumn(
                name: "PaymentSummary_Last4",
                table: "OrderHeaders");

            migrationBuilder.DropColumn(
                name: "ShippingAddress_City",
                table: "OrderHeaders");

            migrationBuilder.DropColumn(
                name: "ShippingAddress_Country",
                table: "OrderHeaders");

            migrationBuilder.DropColumn(
                name: "ShippingAddress_Line1",
                table: "OrderHeaders");

            migrationBuilder.DropColumn(
                name: "ShippingAddress_Line2",
                table: "OrderHeaders");

            migrationBuilder.DropColumn(
                name: "ShippingAddress_Name",
                table: "OrderHeaders");

            migrationBuilder.DropColumn(
                name: "ShippingAddress_PostalCode",
                table: "OrderHeaders");

            migrationBuilder.DropColumn(
                name: "ShippingAddress_State",
                table: "OrderHeaders");

            migrationBuilder.AddColumn<DateTime>(
                name: "OrderTime",
                table: "OrderHeaders",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
