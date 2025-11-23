using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InventoryService.Migrations
{
    /// <inheritdoc />
    public partial class ChangeProductIdToGuid : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("PRAGMA foreign_keys=OFF;");

            // Create a new table with the correct schema (Id as TEXT for Guid).
            migrationBuilder.Sql(@"
        CREATE TABLE InventoryItems_new (
            Id TEXT PRIMARY KEY NOT NULL,
            ProductId INTEGER NOT NULL,
            QuantityAvailable INTEGER NOT NULL,
            LastUpdated TEXT NOT NULL
        );
    ");

            // Copy and convert existing data from old table to new table.
            // Generate a valid GUID string for each Id using lower(hex(randomblob(16))).
            migrationBuilder.Sql(@"
        INSERT INTO InventoryItems_new (Id, ProductId, QuantityAvailable, LastUpdated)
        SELECT lower(hex(randomblob(16))), ProductId, QuantityAvailable, LastUpdated FROM InventoryItems;
    ");

            // Drop the old table.
            migrationBuilder.Sql("DROP TABLE InventoryItems;");

            // Rename new table to the original name.
            migrationBuilder.Sql("ALTER TABLE InventoryItems_new RENAME TO InventoryItems;");

            migrationBuilder.Sql("PRAGMA foreign_keys=ON;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
