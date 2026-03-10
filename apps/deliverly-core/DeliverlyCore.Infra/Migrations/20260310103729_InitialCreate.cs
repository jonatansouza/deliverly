using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DeliverlyCore.Infra.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tariff_tables",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    description = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    origin_prefix = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: false),
                    destination_prefix = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: false),
                    min_weight = table.Column<decimal>(type: "numeric(10,3)", nullable: false),
                    max_weight = table.Column<decimal>(type: "numeric(10,3)", nullable: false),
                    base_value_amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    base_value_currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tariff_tables", x => x.id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tariff_tables");
        }
    }
}
