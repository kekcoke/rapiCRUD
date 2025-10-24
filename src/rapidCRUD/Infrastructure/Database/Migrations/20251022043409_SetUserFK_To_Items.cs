using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace rapidCRUD.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class SetUserFK_To_Items : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "Items",
                nullable: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Revert the changes in the Down method
            // migrationBuilder.DropForeignKey(
                // name: "FK_MyTable_OtherTable_NewForeignKeyColumnId",
                // table: "MyTable");

            // migrationBuilder.DropColumn(
                // name: "NewForeignKeyColumnId",
                // table: "MyTable");
        }
    }
}
