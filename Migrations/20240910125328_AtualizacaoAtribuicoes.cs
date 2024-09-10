using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FotosAPI.Migrations
{
    /// <inheritdoc />
    public partial class AtualizacaoAtribuicoes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FilePath",
                table: "photo");

            migrationBuilder.DropColumn(
                name: "ThumbnailId",
                table: "photo");

            migrationBuilder.RenameColumn(
                name: "Picture",
                table: "photo",
                newName: "PicturePath");

            migrationBuilder.AddColumn<bool>(
                name: "thumbnail",
                table: "photo",
                type: "boolean",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "thumbnail",
                table: "photo");

            migrationBuilder.RenameColumn(
                name: "PicturePath",
                table: "photo",
                newName: "Picture");

            migrationBuilder.AddColumn<string>(
                name: "FilePath",
                table: "photo",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ThumbnailId",
                table: "photo",
                type: "integer",
                nullable: true);
        }
    }
}
