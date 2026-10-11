using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bison.Razor.Migrations
{
    /// <inheritdoc />
    public partial class InitialDBSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Authors",
                columns: table => new
                {
                    AuthorId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Email = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Authors", x => x.AuthorId);
                });

            migrationBuilder.CreateTable(
                name: "Taxa",
                columns: table => new
                {
                    TaxonId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    dwc_TaxonID = table.Column<string>(type: "TEXT", nullable: false),
                    VernacularName = table.Column<string>(type: "TEXT", nullable: false),
                    ParentTaxonId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Taxa", x => x.TaxonId);
                    table.ForeignKey(
                        name: "FK_Taxa_Taxa_ParentTaxonId",
                        column: x => x.ParentTaxonId,
                        principalTable: "Taxa",
                        principalColumn: "TaxonId");
                });

            migrationBuilder.CreateTable(
                name: "Posts",
                columns: table => new
                {
                    PostId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Text = table.Column<string>(type: "TEXT", nullable: false),
                    TimeStamp = table.Column<DateTime>(type: "TEXT", nullable: false),
                    AuthorId = table.Column<int>(type: "INTEGER", nullable: false),
                    Discriminator = table.Column<string>(type: "TEXT", maxLength: 13, nullable: false),
                    ObservationPostId = table.Column<int>(type: "INTEGER", nullable: true),
                    TaxonId = table.Column<int>(type: "INTEGER", nullable: true),
                    Proposal_ObservationPostId = table.Column<int>(type: "INTEGER", nullable: true),
                    Proposal_TaxonId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Posts", x => x.PostId);
                    table.ForeignKey(
                        name: "FK_Posts_Authors_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "Authors",
                        principalColumn: "AuthorId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Posts_Posts_ObservationPostId",
                        column: x => x.ObservationPostId,
                        principalTable: "Posts",
                        principalColumn: "PostId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Posts_Posts_Proposal_ObservationPostId",
                        column: x => x.Proposal_ObservationPostId,
                        principalTable: "Posts",
                        principalColumn: "PostId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Posts_Taxa_Proposal_TaxonId",
                        column: x => x.Proposal_TaxonId,
                        principalTable: "Taxa",
                        principalColumn: "TaxonId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Posts_Taxa_TaxonId",
                        column: x => x.TaxonId,
                        principalTable: "Taxa",
                        principalColumn: "TaxonId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Posts_AuthorId",
                table: "Posts",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_ObservationPostId",
                table: "Posts",
                column: "ObservationPostId");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_Proposal_ObservationPostId",
                table: "Posts",
                column: "Proposal_ObservationPostId");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_Proposal_TaxonId",
                table: "Posts",
                column: "Proposal_TaxonId");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_TaxonId",
                table: "Posts",
                column: "TaxonId");

            migrationBuilder.CreateIndex(
                name: "IX_Taxa_ParentTaxonId",
                table: "Taxa",
                column: "ParentTaxonId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Posts");

            migrationBuilder.DropTable(
                name: "Authors");

            migrationBuilder.DropTable(
                name: "Taxa");
        }
    }
}
