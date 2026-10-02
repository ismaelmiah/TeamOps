using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TeamOps.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddTaskProjectCascade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_tasks_ProjectId",
                table: "tasks",
                column: "ProjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_tasks_projects_ProjectId",
                table: "tasks",
                column: "ProjectId",
                principalTable: "projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tasks_projects_ProjectId",
                table: "tasks");

            migrationBuilder.DropIndex(
                name: "IX_tasks_ProjectId",
                table: "tasks");
        }
    }
}
