using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace rapidCRUD.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class Update_Users_Items_With_PK : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Items",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid));
            
            migrationBuilder.AddPrimaryKey(
                name: "PK_Users",
                table: "Users",
                column: "Id");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Users",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid));
            
            migrationBuilder.AddForeignKey(
                name: "FK_Items_Users_NewFK_UserId",
                table: "Items",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
            

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
