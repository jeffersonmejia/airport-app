using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Airport.Features.Bookings.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddTicketPurchaseFlow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "airport_app");

            migrationBuilder.CreateTable(
                name: "orders",
                schema: "airport_app",
                columns: table => new
                {
                    order_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: false),
                    flight_id = table.Column<int>(type: "integer", nullable: false),
                    flight_number = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: false),
                    origin_code = table.Column<string>(type: "character varying(4)", maxLength: 4, nullable: false),
                    destination_code = table.Column<string>(type: "character varying(4)", maxLength: 4, nullable: false),
                    departure = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    fare_code = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    fare_name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    total = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false),
                    currency_code = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_orders", x => x.order_id);
                });

            migrationBuilder.CreateTable(
                name: "order_details",
                schema: "airport_app",
                columns: table => new
                {
                    order_detail_id = table.Column<Guid>(type: "uuid", nullable: false),
                    order_id = table.Column<Guid>(type: "uuid", nullable: false),
                    passenger_first_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    passenger_last_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    passport_number = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    quantity = table.Column<int>(type: "integer", nullable: false),
                    unit_price = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_order_details", x => x.order_detail_id);
                    table.ForeignKey(
                        name: "FK_order_details_orders_order_id",
                        column: x => x.order_id,
                        principalSchema: "airport_app",
                        principalTable: "orders",
                        principalColumn: "order_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "payments",
                schema: "airport_app",
                columns: table => new
                {
                    payment_id = table.Column<Guid>(type: "uuid", nullable: false),
                    order_id = table.Column<Guid>(type: "uuid", nullable: false),
                    provider = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    provider_order_id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    approval_url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    provider_capture_id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    idempotency_key = table.Column<string>(type: "character varying(108)", maxLength: 108, nullable: false),
                    status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    amount = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false),
                    currency_code = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    completed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_payments", x => x.payment_id);
                    table.ForeignKey(
                        name: "FK_payments_orders_order_id",
                        column: x => x.order_id,
                        principalSchema: "airport_app",
                        principalTable: "orders",
                        principalColumn: "order_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "purchased_tickets",
                schema: "airport_app",
                columns: table => new
                {
                    purchased_ticket_id = table.Column<Guid>(type: "uuid", nullable: false),
                    order_id = table.Column<Guid>(type: "uuid", nullable: false),
                    flight_id = table.Column<int>(type: "integer", nullable: false),
                    ticket_number = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    fare_code = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    issued_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_purchased_tickets", x => x.purchased_ticket_id);
                    table.ForeignKey(
                        name: "FK_purchased_tickets_orders_order_id",
                        column: x => x.order_id,
                        principalSchema: "airport_app",
                        principalTable: "orders",
                        principalColumn: "order_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_order_details_order_id",
                schema: "airport_app",
                table: "order_details",
                column: "order_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_orders_user_id_created_at",
                schema: "airport_app",
                table: "orders",
                columns: new[] { "user_id", "created_at" });

            migrationBuilder.CreateIndex(
                name: "IX_payments_idempotency_key",
                schema: "airport_app",
                table: "payments",
                column: "idempotency_key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_payments_order_id",
                schema: "airport_app",
                table: "payments",
                column: "order_id");

            migrationBuilder.CreateIndex(
                name: "IX_payments_provider_capture_id",
                schema: "airport_app",
                table: "payments",
                column: "provider_capture_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_payments_provider_order_id",
                schema: "airport_app",
                table: "payments",
                column: "provider_order_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_purchased_tickets_order_id",
                schema: "airport_app",
                table: "purchased_tickets",
                column: "order_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_purchased_tickets_ticket_number",
                schema: "airport_app",
                table: "purchased_tickets",
                column: "ticket_number",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "order_details",
                schema: "airport_app");

            migrationBuilder.DropTable(
                name: "payments",
                schema: "airport_app");

            migrationBuilder.DropTable(
                name: "purchased_tickets",
                schema: "airport_app");

            migrationBuilder.DropTable(
                name: "orders",
                schema: "airport_app");
        }
    }
}
