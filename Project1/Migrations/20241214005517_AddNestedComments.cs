using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Project1.Migrations
{
    /// <inheritdoc />
    public partial class AddNestedComments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ParentCommentID",
                table: "Comments",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Comments_ParentCommentID",
                table: "Comments",
                column: "ParentCommentID");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Comments_ParentCommentID",
                table: "Comments",
                column: "ParentCommentID",
                principalTable: "Comments",
                principalColumn: "CommentID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Comments_ParentCommentID",
                table: "Comments");

            migrationBuilder.DropIndex(
                name: "IX_Comments_ParentCommentID",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "ParentCommentID",
                table: "Comments");
        }
    }
}
