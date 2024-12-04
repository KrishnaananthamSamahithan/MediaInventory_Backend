using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MediaInventory.Migrations
{
    /// <inheritdoc />
    public partial class AddTransactionsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "User",
                table: "Transactions",
                newName: "UserName");

            migrationBuilder.RenameColumn(
                name: "Timestamp",
                table: "Transactions",
                newName: "TransactionDate");

            migrationBuilder.RenameColumn(
                name: "Action",
                table: "Transactions",
                newName: "TransactionType");

            migrationBuilder.AddColumn<string>(
                name: "Role",
                table: "Users",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_InventoryId",
                table: "Transactions",
                column: "InventoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Inventories_InventoryId",
                table: "Transactions",
                column: "InventoryId",
                principalTable: "Inventories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Inventories_InventoryId",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_InventoryId",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "Role",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "UserName",
                table: "Transactions",
                newName: "User");

            migrationBuilder.RenameColumn(
                name: "TransactionType",
                table: "Transactions",
                newName: "Action");

            migrationBuilder.RenameColumn(
                name: "TransactionDate",
                table: "Transactions",
                newName: "Timestamp");
        }
    }
}
