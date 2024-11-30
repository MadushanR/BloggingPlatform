using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Project1.Data.Migrations
{
    /// <inheritdoc />
    public partial class addedUserEmail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserEmail",
                table: "Blogs",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserEmail",
                table: "Blogs");
        }
    }
}
