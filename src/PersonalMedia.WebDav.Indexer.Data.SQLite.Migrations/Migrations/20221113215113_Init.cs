using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PersonalMedia.WebDav.Indexer.Data.SQLite.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Items",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ParentId = table.Column<int>(type: "INTEGER", nullable: false),
                    name = table.Column<string>(type: "TEXT", nullable: false),
                    created = table.Column<long>(type: "INTEGER", nullable: true),
                    modified = table.Column<long>(type: "INTEGER", nullable: true),
                    size = table.Column<long>(type: "INTEGER", nullable: true),
                    path = table.Column<string>(type: "TEXT", nullable: false),
                    etag = table.Column<string>(type: "TEXT", nullable: false),
                    itemtype = table.Column<string>(name: "item_type", type: "TEXT", nullable: false),
                    ItemId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Items", x => x.id);
                    table.ForeignKey(
                        name: "FK_Items_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Items_ItemId",
                table: "Items",
                column: "ItemId");

            // For SQLite FTS
            migrationBuilder.Sql(
                "CREATE VIRTUAL TABLE IF NOT EXISTS \"Items_FTS\" USING fts5(\"Text\", \"Title\", content=\"Items\", content_rowid=\"Id\");");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // For SQLite FTS
            migrationBuilder.Sql(
                "DROP TABLE \"Items_FTS\";");

            migrationBuilder.DropTable(
                name: "Items");
        }
    }
}
