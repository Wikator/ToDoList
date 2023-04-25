using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ToDoList.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddedCommentsToDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_College_AspNetUsers_IdentityUserId",
                table: "College");

            migrationBuilder.DropForeignKey(
                name: "FK_College_Groups_GroupId",
                table: "College");

            migrationBuilder.DropForeignKey(
                name: "FK_College_Subjects_SubjectId",
                table: "College");

            migrationBuilder.DropForeignKey(
                name: "FK_CompletedCollege_AspNetUsers_ApplicationUserId",
                table: "CompletedCollege");

            migrationBuilder.DropForeignKey(
                name: "FK_CompletedCollege_College_CollegeId",
                table: "CompletedCollege");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CompletedCollege",
                table: "CompletedCollege");

            migrationBuilder.DropPrimaryKey(
                name: "PK_College",
                table: "College");

            migrationBuilder.RenameTable(
                name: "CompletedCollege",
                newName: "CompletedColleges");

            migrationBuilder.RenameTable(
                name: "College",
                newName: "Colleges");

            migrationBuilder.RenameIndex(
                name: "IX_CompletedCollege_CollegeId",
                table: "CompletedColleges",
                newName: "IX_CompletedColleges_CollegeId");

            migrationBuilder.RenameIndex(
                name: "IX_CompletedCollege_ApplicationUserId",
                table: "CompletedColleges",
                newName: "IX_CompletedColleges_ApplicationUserId");

            migrationBuilder.RenameIndex(
                name: "IX_College_SubjectId",
                table: "Colleges",
                newName: "IX_Colleges_SubjectId");

            migrationBuilder.RenameIndex(
                name: "IX_College_IdentityUserId",
                table: "Colleges",
                newName: "IX_Colleges_IdentityUserId");

            migrationBuilder.RenameIndex(
                name: "IX_College_GroupId",
                table: "Colleges",
                newName: "IX_Colleges_GroupId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CompletedColleges",
                table: "CompletedColleges",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Colleges",
                table: "Colleges",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Comments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CollegeId = table.Column<int>(type: "int", nullable: false),
                    ApplicationUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Text = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    FileUrl = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Comments_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Comments_Colleges_CollegeId",
                        column: x => x.CollegeId,
                        principalTable: "Colleges",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Comments_ApplicationUserId",
                table: "Comments",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_CollegeId",
                table: "Comments",
                column: "CollegeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Colleges_AspNetUsers_IdentityUserId",
                table: "Colleges",
                column: "IdentityUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_Colleges_Groups_GroupId",
                table: "Colleges",
                column: "GroupId",
                principalTable: "Groups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Colleges_Subjects_SubjectId",
                table: "Colleges",
                column: "SubjectId",
                principalTable: "Subjects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CompletedColleges_AspNetUsers_ApplicationUserId",
                table: "CompletedColleges",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CompletedColleges_Colleges_CollegeId",
                table: "CompletedColleges",
                column: "CollegeId",
                principalTable: "Colleges",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Colleges_AspNetUsers_IdentityUserId",
                table: "Colleges");

            migrationBuilder.DropForeignKey(
                name: "FK_Colleges_Groups_GroupId",
                table: "Colleges");

            migrationBuilder.DropForeignKey(
                name: "FK_Colleges_Subjects_SubjectId",
                table: "Colleges");

            migrationBuilder.DropForeignKey(
                name: "FK_CompletedColleges_AspNetUsers_ApplicationUserId",
                table: "CompletedColleges");

            migrationBuilder.DropForeignKey(
                name: "FK_CompletedColleges_Colleges_CollegeId",
                table: "CompletedColleges");

            migrationBuilder.DropTable(
                name: "Comments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CompletedColleges",
                table: "CompletedColleges");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Colleges",
                table: "Colleges");

            migrationBuilder.RenameTable(
                name: "CompletedColleges",
                newName: "CompletedCollege");

            migrationBuilder.RenameTable(
                name: "Colleges",
                newName: "College");

            migrationBuilder.RenameIndex(
                name: "IX_CompletedColleges_CollegeId",
                table: "CompletedCollege",
                newName: "IX_CompletedCollege_CollegeId");

            migrationBuilder.RenameIndex(
                name: "IX_CompletedColleges_ApplicationUserId",
                table: "CompletedCollege",
                newName: "IX_CompletedCollege_ApplicationUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Colleges_SubjectId",
                table: "College",
                newName: "IX_College_SubjectId");

            migrationBuilder.RenameIndex(
                name: "IX_Colleges_IdentityUserId",
                table: "College",
                newName: "IX_College_IdentityUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Colleges_GroupId",
                table: "College",
                newName: "IX_College_GroupId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CompletedCollege",
                table: "CompletedCollege",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_College",
                table: "College",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_College_AspNetUsers_IdentityUserId",
                table: "College",
                column: "IdentityUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_College_Groups_GroupId",
                table: "College",
                column: "GroupId",
                principalTable: "Groups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_College_Subjects_SubjectId",
                table: "College",
                column: "SubjectId",
                principalTable: "Subjects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CompletedCollege_AspNetUsers_ApplicationUserId",
                table: "CompletedCollege",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CompletedCollege_College_CollegeId",
                table: "CompletedCollege",
                column: "CollegeId",
                principalTable: "College",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
